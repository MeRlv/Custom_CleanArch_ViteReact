import { Navigate, Outlet } from 'react-router';
import { AuthenticationStatus } from '../../api/credentials-manager';
import { useAuthenticationStore } from '../../feature/authentication/service/store/authentication.store';

export function AuthenticationGuard() {
  const isAuthenticated = useAuthenticationStore((s) => s.isAuthenticated);

  switch (isAuthenticated) {
    case AuthenticationStatus.AUTHENTICATED:
      return <Outlet />;

    default:
      return <Navigate to="/authenticate" replace />;
  }
}
