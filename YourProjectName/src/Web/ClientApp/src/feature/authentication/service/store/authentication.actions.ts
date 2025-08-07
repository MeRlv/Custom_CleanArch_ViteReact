import { ApiClient } from '../../../../api/api-client';
import { AuthenticationStatus } from '../../../../api/credentials-manager';
import type { User } from '../../../../api/nswag-api.g';
import type { ImmerSet, ImmerGet } from '../../../../core/model/immer.types';
import DIContainer from '../../../../core/services/dependancy-injection';
import type { AuthenticationActions } from '../../domain/store/authentication.actions';
import type { AuthenticationStore } from '../../domain/store/authentication.store';
import inflight from 'promise-inflight';

const apiClient = DIContainer.get(ApiClient);

async function loginImpl(username: string, password: string, set: ImmerSet<AuthenticationStore>): Promise<void> {
  const user = await apiClient.loginAndSaveCredentials(username, password);

  set({
    isAuthenticated: user ? AuthenticationStatus.AUTHENTICATED : AuthenticationStatus.UNAUTHENTICATED,
    user: user ?? null,
  });
}

function logoutImpl(set: ImmerSet<AuthenticationStore>): void {
  apiClient.clear();
  set({
    isAuthenticated: AuthenticationStatus.UNAUTHENTICATED,
    user: null,
  });
}

async function loadImpl(set: ImmerSet<AuthenticationStore>): Promise<void> {
  const user: User | null = await apiClient.loadCredentialsFromStorage();
  set({
    isAuthenticated: user ? AuthenticationStatus.AUTHENTICATED : AuthenticationStatus.UNAUTHENTICATED,
    user: user,
  });
}

export const createAuthenticationActions = (
  set: ImmerSet<AuthenticationStore>,
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  _get: ImmerGet<AuthenticationStore>
): AuthenticationActions => ({
  load: () => inflight(`authentication-loading`, () => loadImpl(set)),
  login: (username: string, password: string) => inflight(`authentication-login-${username}`, () => loginImpl(username, password, set)),
  logout: () => logoutImpl(set),
});
