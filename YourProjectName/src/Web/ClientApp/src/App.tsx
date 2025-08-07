import { PrimeReactProvider } from "primereact/api";
import "./App.css";
import { AppRoutes } from "./core/routes/app.routes";

function App() {
  return (
    <PrimeReactProvider>
      <AppRoutes />
    </PrimeReactProvider>
  );
}

export default App;
