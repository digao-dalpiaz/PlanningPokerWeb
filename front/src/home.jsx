import { useState } from "react";
import { Button, Form, Card, InputGroup, Spinner } from "react-bootstrap";
import { toast } from "react-toastify";
import { useLocation, useNavigate } from "react-router";
import { api } from "./request";

const MODO_NEW = 'N';
const MODO_JOIN = 'J';

export default function Home() {

  const navigate = useNavigate();
  const location = useLocation();

  const [entrando, setEntrando] = useState(false);

  const [modo, setModo] = useState(MODO_JOIN);
  const [nome, setNome] = useState('');
  const [idSala, setIdSala] = useState(new URLSearchParams(location.search).get('idSala') ?? '');
  const [descricao, setDescricao] = useState('');

  return (
    <>
      <Card>
        <Card.Header>😊 Bem-vindo :: Crie ou junte-se a uma sala</Card.Header>
        <Card.Body>

          <span>Seu nome</span>
          <InputGroup style={{ width: '400px' }}>
            <InputGroup.Text><i className="fa-solid fa-user" /></InputGroup.Text>
            <Form.Control value={nome} onChange={ev => setNome(ev.target.value)} maxLength="50"
              placeholder="Digão" />
          </InputGroup>

          <div style={{ height: 10 }} />

          <span>Modo</span>
          <Form.Check type="radio" id="modoN" label="Criar nova sala"
            checked={modo === MODO_NEW} onChange={() => setModo(MODO_NEW)} />
          <Form.Check type="radio" id="modoJ" label="Entrar em sala existente"
            checked={modo === MODO_JOIN} onChange={() => setModo(MODO_JOIN)} />

          <div style={{ height: 10 }} />

          {modo === MODO_JOIN ?
            <>
              <span>Código da sala</span>
              <InputGroup style={{ width: '250px' }}>
                <InputGroup.Text><i className="fa-solid fa-hashtag" /></InputGroup.Text>
                <Form.Control value={idSala} onChange={ev => setIdSala(ev.target.value)} maxLength="14"
                  placeholder="AAAA-AAAA-AAAA" />
              </InputGroup>
            </>
            :
            <>
              <span>Descrição da sala</span>
              <InputGroup>
                <InputGroup.Text><i className="fa-solid fa-tag" /></InputGroup.Text>
                <Form.Control value={descricao} onChange={ev => setDescricao(ev.target.value)} maxLength="50"
                  placeholder="Sprint 123 - Task 456 - Refatoração Cadastros" />
              </InputGroup>
            </>}

          <div style={{ height: 20 }} />

          <Button disabled={entrando} onClick={go}>
            <i className="fa-solid fa-right-to-bracket" /> {modo === MODO_JOIN ? 'Entrar' : 'Criar'}</Button>
          &nbsp;{entrando && <Spinner size="sm" />}

        </Card.Body>
      </Card>
    </>
  )

  async function go() {
    const xNome = nome.trim();
    const xIdSala = idSala.trim();
    const xDescricao = descricao.trim();

    if (!xNome) {
      toast.warn('Especifique seu nome');
      return false;
    }

    if (modo === MODO_JOIN) {
      if (!xIdSala) {
        toast.warn('Especifique o código da sala');
        return;
      }

      if (xIdSala.length !== 14) {
        toast.warn('Código da sala com formato inválido');
        return;
      }

      if (xIdSala.indexOf(' ') !== -1) {
        toast.warn("Código da sala não pode conter espaços");
        return;
      }
    }

    //

    let dadosIngresso;

    setEntrando(true);
    try {
      dadosIngresso = (await api.post('geral/ingressar', {
        nome: xNome,
        idSala: modo === MODO_JOIN ? xIdSala : null,
        descricao: modo === MODO_JOIN ? null : xDescricao
      })).data;
    } finally {
      setEntrando(false);
    }

    if (dadosIngresso.idSala === null) {
      toast.warn('Sala não existe');
      return;
    }

    navigate("/sala", {
      state: {
        idSala: dadosIngresso.idSala,
        token: dadosIngresso.token,
        descricao: dadosIngresso.descricao
      }
    });
  }

}