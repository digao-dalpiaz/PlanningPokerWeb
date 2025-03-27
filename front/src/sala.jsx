import { useEffect, useRef, useState } from "react";
import { useLocation } from "react-router";
import * as signalR from "@microsoft/signalr";
import { Badge, Button, Card, Col, Form, Row, Table } from "react-bootstrap";
import { toast } from "react-toastify";
import { IS_DEV_MODE, URL_BACKEND } from "./definicoes";

const STATUS_DESCONECTADO = 'D';
const STATUS_CONECTADO = 'C';
const STATUS_CONECTANDO = '.';

export function Sala() {

  const location = useLocation();
  const cred = location.state;

  const [statusCon, setStatusCon] = useState(STATUS_DESCONECTADO);
  const [infoUser, setInfoUser] = useState();
  const [posicao, setPosicao] = useState();
  const [voto, setVoto] = useState('');

  const conRef = useRef();
  const firstRun = useRef();

  useEffect(() => {
    if (IS_DEV_MODE && !firstRun.current) {
      firstRun.current = true;
      return;
    }

    conectar(); //promisse

    return () => {
      conRef.current?.stop();
    }
    // eslint-disable-next-line
  }, []);

  async function conectar() {
    if (!cred) {
      toast.error('Credenciais não informadas');
      return;
    }

    const con = new signalR.HubConnectionBuilder()
      .withUrl(URL_BACKEND + '/chatHub')
      .withAutomaticReconnect()
      .build();

    con.onreconnecting(() => {
      setStatusCon(STATUS_CONECTANDO);
      //toast.warn('Reconectando com o servidor...');
    });
    con.onreconnected(() => {
      setStatusCon(STATUS_CONECTADO);
      //toast.info('Reconectado com sucesso!');

      obterInforUser(); //promisse
    });
    con.onclose(() => {
      setStatusCon(STATUS_DESCONECTADO);
      //toast.error('Desconectado do servidor');
    });

    con.on('Posicao', data => setPosicao(data));

    conRef.current = con;

    setStatusCon(STATUS_CONECTANDO);
    try {
      await con.start();
    }
    catch (err) {
      setStatusCon(STATUS_DESCONECTADO);
      toast.error(/*err.message*/ 'Não foi possível se conectar ao servidor');
      return;
    }

    setStatusCon(STATUS_CONECTADO);

    obterInforUser(); //promisse
  }

  async function obterInforUser() {
    let data;

    try {
      data = await call('Entrar', cred.idSala, cred.token);
    } catch (err) {
      conRef.current.stop();
      return;
    }

    setInfoUser(data);
  }

  async function call(method, ...args) {
    try {
      if (conRef.current.state !== 'Connected') throw new Error('Conexão foi perdida com o servidor');
      return await conRef.current.invoke(method, ...args);
    } catch (err) {
      toast.error(getSignalRException(err));
      throw err;
    }
  };

  function getSignalRException(err) {
    const msg = err.message;
    const ident = 'Exception: ';
    const x = msg.indexOf(ident);
    if (x !== -1) return msg.substring(x + ident.length);
    return msg;
  }

  return (
    <>
      <table width="100%" style={{ marginBottom: 10 }}>
        <tbody>
          <tr>
            <td>
              {cred &&
                <>
                  <i className="fa-solid fa-layer-group" /> Sala: {cred.idSala}
                  &nbsp;<Button size="sm" variant="light" title="Compartilhar" onClick={shareSala}><i className="fa-solid fa-share-nodes" /></Button>
                </>}
            </td>
            <td align="right">
              {statusCon === STATUS_DESCONECTADO ?
                <span className="text-secondary"><i className="fa-solid fa-ban" /> Desconectado</span>
                : statusCon === STATUS_CONECTADO ?
                  <span className="text-success"><i className="fa-solid fa-wifi" /> Conectado</span>
                  :
                  <span className="text-warning"><i className="fa-solid fa-spinner" /> Conectando...</span>}
            </td>
          </tr>
        </tbody>
      </table>

      {infoUser && posicao && getCard()}
    </>
  )

  function getCard() {
    return (
      <>
        <Card hidden={!infoUser.admin} style={{ marginBottom: 10 }}>
          <Card.Header><i className="fa-solid fa-shield-halved" /> Administração</Card.Header>
          <Card.Body>
            <Button onClick={definirStatusSala}>{posicao.emVotacao ?
              <><i className="fa-solid fa-stop" /> Encerrar votação</>
              :
              <><i className="fa-solid fa-play" /> Iniciar votação</>}</Button>
          </Card.Body>
        </Card>

        <Card style={{ backgroundColor: posicao.emVotacao ? '#b1e0f3' : '' }}>
          <Card.Header><i className="fa-solid fa-head-side-virus" /> Voto {posicao.emVotacao ? <span> :: <b>VOTAÇÃO EM ANDAMENTO</b></span> : null}</Card.Header>
          <Card.Body>

            <Row>
              <Col>
                <Form.Control type="number" min="0" value={voto} onChange={ev => setVoto(ev.target.value)} />
              </Col>
              <Col>
                <Button onClick={votar} variant="success" disabled={!posicao.emVotacao}>Votar</Button>
                &nbsp;&nbsp;&nbsp;
                <Button onClick={abster} variant="danger" disabled={!posicao.emVotacao}>Abster</Button>
              </Col>
            </Row>

          </Card.Body>
        </Card>

        <Table striped hover>
          <thead>
            <tr>
              <th width="55%">Nome</th>
              <th width="25%">Status</th>
              <th>Voto</th>
            </tr>
          </thead>
          <tbody>
            {posicao.users.map(x =>
              <tr key={x.uuid} className={x.uuid === infoUser.uuid ? 'table-warning' : ''}>
                <td><i className="fa-solid fa-user" /> {x.nome} {x.admin && <Badge>ADMIN</Badge>} {!x.conectado && <i className="fa-solid fa-ban" />}</td>
                <td>
                  {x.votou ?
                    <span className="text-success"><i className="fa-solid fa-thumbs-up" /> Votou</span>
                    :
                    (posicao.emVotacao ?
                      <span className="text-secondary"><i className="fa-solid fa-hourglass-start" /> Esperando</span>
                      :
                      <span className="text-danger"><i className="fa-solid fa-circle-exclamation" /> Não votou</span>)}
                </td>
                <td className="text-end">{x.voto === 0 ? <><i className="fa-solid fa-hands" /> Absteve</> : x.voto}</td>
              </tr>)}
          </tbody>
        </Table>

        {!posicao.emVotacao && getSumario()}
      </>
    )
  }

  async function votar() {
    if (!voto) {
      toast.warn('Especifique o valor do voto');
      return;
    }

    const valor = parseInt(voto);
    if (valor <= 0) {
      toast.warn('Valor deve ser maior que zero');
      return;
    }

    await call('Votar', parseInt(voto));
    toast.info('Voto enviado: ' + voto);
    setVoto('');
  }

  async function abster() {
    await call('Votar', 0);
    toast.info('Abstenção enviada');
  }

  async function definirStatusSala() {
    await call('DefinirStatusSala', !posicao.emVotacao);
  }

  function getSumario() {
    const numeros = posicao.users.filter(x => x.voto > 0).map(x => x.voto);

    return (
      <div style={{ textAlign: 'right' }}>
        <b>Média:</b> {numeros.length > 0 ? calcularMedia(numeros).toFixed(1) : null} (Votos: {numeros.length})
      </div>
    )
  }

  function calcularMedia(numeros) {
    let soma = 0;
    numeros.forEach(x => soma += x);
    return soma / numeros.length;
  }

  async function shareSala() {
    await navigator.clipboard.writeText(window.location.origin + '/#/?idSala=' + cred.idSala);
    toast.info('Link copiado para a área de transferência');
  }

}