import { Card } from "primereact/card";
import { useEffect, useState } from "react";
import { InputText } from "primereact/inputtext";
import { Password } from "primereact/password";
import { Button } from "primereact/button";
import { useNavigate } from "react-router";
import { Message } from "primereact/message";
import { ApiException } from "../../../../api/nswag-api.g";
import { useAuthenticationStore } from "../../service/store/authentication.store";

type AuthError = {
  message: string;
  isUsername: boolean;
  isPassword: boolean;
};

export default function AuthenticationView() {
  const navigate = useNavigate();
  const login = useAuthenticationStore((s) => s.login);
  const logout = useAuthenticationStore((s) => s.logout);

  const [username, setUsername] = useState<string>("");
  const [password, setPassword] = useState<string>("");

  const [error, setError] = useState<AuthError | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  // Automatically logout on component mount to ensure a clean state
  // This is useful if the user navigates directly or is redirected to the authentication view
  useEffect(() => {
    logout();
  }, [logout]);

  async function handleSubmit(): Promise<void> {
    setLoading(true);

    if (username.trim() === "" || password.trim() === "") {
      // Empty field => error
      setError({
        message: "Veuillez remplir tous les champs.",
        isUsername: username.trim() === "",
        isPassword: password.trim() === "",
      });
    } else {
      // Valid form
      try {
        await login(
          username,
          password,
        );
        // Login Success
        navigate("/");
      } catch (error) {
        // Api error : 404 => ids not exits
        const e = error as ApiException;
        setError({
          message: e.message,
          isUsername: true,
          isPassword: true,
        });
      }
    }

    setLoading(false);
    return;
  }

  return (
    <div className="h-screen w-screen flex items-center justify-center">
      <Card
        title="YourProjectName - Connexion"
        style={{ minWidth: "25rem", padding: "1rem" }}
      >
        <form
          onSubmit={(e) => {
            e.preventDefault();
            handleSubmit();
          }}
          className="flex flex-col gap-5 mt-5"
        >
          {error && <Message severity="error" text={error?.message} />}

          <div className="p-inputgroup mt-5">
            <span className="p-inputgroup-addon">
              <i className="pi pi-user"></i>
            </span>
            <InputText
              id="username"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              placeholder="Identifiant"
              invalid={error?.isUsername}
              disabled={loading}
            />
          </div>

          <div className="p-inputgroup">
            <span className="p-inputgroup-addon">
              <i className="pi pi-lock"></i>
            </span>
            <Password
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              feedback={false}
              placeholder="Mot de passe"
              invalid={error?.isPassword}
              disabled={loading}
            />
          </div>

          <Button
            label="Connexion"
            onClick={handleSubmit}
            className="self-center"
            loading={loading}
          />
        </form>
      </Card>
    </div>
  );
}
