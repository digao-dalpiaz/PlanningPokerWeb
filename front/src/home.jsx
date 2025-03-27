import { useState } from "react";
import { Button, Form, Card, InputGroup } from "react-bootstrap";
import { toast } from "react-toastify";
import { useLocation, useNavigate } from "react-router";
import { api } from "./request";

const MODO_JOIN = 'J';

export default function Home() {

  const navigate = useNavigate();
  const location = useLocation();

  const [entrando, setEntrando] = useState(false);

  const [modo, setModo] = useState(MODO_JOIN);
  const [nome, setNome] = useState('');
  const [idSala, setIdSala] = useState(new URLSearchParams(location.search).get('idSala') ?? '');

  return (
    <>
      <Card>
        <Card.Header>Criar ou juntar-se a uma sala</Card.Header>
        <Card.Body>

          <span>Seu nome</span>
          <InputGroup>
            <InputGroup.Text><i className="fa-solid fa-user" /></InputGroup.Text>
            <Form.Control value={nome} onChange={ev => setNome(ev.target.value)} />
          </InputGroup>

          <div style={{ height: 10 }} />

          <span>Modo</span>
          <InputGroup>
            <InputGroup.Text><i className="fa-solid fa-sliders" /></InputGroup.Text>
            <Form.Control as="select"
              value={modo}
              onChange={ev => setModo(ev.target.value)}
            >
              <option value="N">Criar nova sala</option>
              <option value="J">Entrar em sala existente</option>
            </Form.Control>
          </InputGroup>

          {modo === MODO_JOIN &&
            <>
              <div style={{ height: 10 }} />

              <span>Código da sala</span>
              <InputGroup>
                <InputGroup.Text><i className="fa-solid fa-hashtag" /></InputGroup.Text>
                <Form.Control value={idSala} onChange={ev => setIdSala(ev.target.value)} />
              </InputGroup>
            </>}

          <div style={{ height: 20 }} />

          <Button disabled={entrando} onClick={go}><i className="fa-solid fa-right-to-bracket" /> Entrar</Button>

        </Card.Body>
      </Card>
    </>
  )

  async function go() {
    const xNome = nome.trim();
    const xIdSala = idSala.trim();

    if (!xNome) {
      toast.warn('Especifique seu nome');
      return false;
    }

    if (modo === MODO_JOIN) {
      if (!xIdSala) {
        toast.warn('Especifique o código da sala');
        return;
      }

      if (xIdSala.length !== 36) {
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
        idSala: modo === MODO_JOIN ? xIdSala : null
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
        token: dadosIngresso.token
      }
    });
  }

}