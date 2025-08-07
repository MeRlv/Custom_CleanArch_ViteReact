import type { AuthenticationActions } from './authentication.actions';
import type { AuthenticationState } from './authentication.state';

export interface AuthenticationStore extends AuthenticationState, AuthenticationActions {}
