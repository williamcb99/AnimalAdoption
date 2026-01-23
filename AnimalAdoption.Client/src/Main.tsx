import ReactDOM from "react-dom/client";
import "./reset.css";
import "./index.css";
import "./variables.css";
import App from "./App.tsx";
import { BrowserRouter } from "react-router";


ReactDOM.createRoot(document.getElementById("app")!).render(
  <BrowserRouter>
    <App />
  </BrowserRouter>
);