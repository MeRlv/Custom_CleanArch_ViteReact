import { AuthenticationStatus } from '../../../../api/credentials-manager';
import type { User } from '../../../../api/nswag-api.g';

export interface AuthenticationState {
  isAuthenticated: AuthenticationStatus;
  user: User | null;
}
