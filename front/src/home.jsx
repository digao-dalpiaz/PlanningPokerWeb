import { useState } from "react";
import { Container, Button, Form, Card } from "react-bootstrap";
import { toast } from "react-toastify";
import { useNavigate } from "react-router";
import { api } from "./request";

const MODO_JOIN = 'J';

export default function Home() {

  const [modo, setModo] = useState(MODO_JOIN);
  const [nome, setNome] = useState('');
  const [idSala, setIdSala] = useState('');

  const navigate = useNavigate();

  return (
    <>
      <Container>
        <Card>
          <Card.Header>Criar ou juntar-se a uma sala</Card.Header>
          <Card.Body>

            <span>Seu nome</span>
            <Form.Control value={nome} onChange={ev => setNome(ev.target.value)} />

            <div style={{ height: 10 }} />

            <span>Modo</span>
            <Form.Control as="select"
              value={modo}
              onChange={ev => setModo(ev.target.value)}
            >
              <option value="N">Criar nova sala</option>
              <option value="J">Entrar em sala existente</option>
            </Form.Control>

            {modo === MODO_JOIN &&
              <>
                <div style={{ height: 10 }} />

                <span>Código da sala</span>
                <Form.Control value={idSala} onChange={ev => setIdSala(ev.target.value)} />
              </>}

            <div style={{ height: 20 }} />

            <Button onClick={go}><i className="fa-solid fa-right-to-bracket" /> Entrar</Button>

          </Card.Body>
        </Card>

      </Container>
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

    const dadosIngresso = (await api.post('geral/ingressar', {
      nome: xNome,
      idSala: modo === MODO_JOIN ? xIdSala : null
    })).data;

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