import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.tsx";

// Import framework styles
import "primereact/resources/themes/md-dark-indigo/theme.css";
import "primeicons/primeicons.css";

// Import custom app style
import "./styles/custom-app.css";
import "./styles/custom-primereact.css";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>
);
