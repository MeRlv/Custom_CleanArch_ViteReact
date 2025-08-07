import { AuthenticationStatus } from '../../../../api/credentials-manager';
import type { AuthenticationState } from '../../domain/store/authentication.state';

export const defaultAuthenticationState: AuthenticationState = {
  isAuthenticated: AuthenticationStatus.UNKNOWN,
  user: null,
};
