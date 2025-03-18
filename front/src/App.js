import { ToastContainer } from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import { HashRouter, Route, Routes } from "react-router";
import Home from "./home";
import { Sala } from "./sala";

function App() {
  return (
    <>
      <ToastContainer />
      <HashRouter>
        <Routes>
          <Route path="/" element={<Home />}/>
          <Route path="/sala" element={<Sala />}/>
          <Route path="*" element={<>Not found</>} />
        </Routes>
      </HashRouter>
    </>
  );
}

export default App;
