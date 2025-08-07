export interface AuthenticationActions {
  load: () => Promise<void>;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}
