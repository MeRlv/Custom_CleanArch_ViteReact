import { useEffect } from 'react';
import { BrowserRouter, Route, Routes } from 'react-router';
import AuthenticationView from '../../feature/authentication/presentation/view/authentication.view';
import { ProgressSpinner } from 'primereact/progressspinner';
import { AuthenticationStatus } from '../../api/credentials-manager';
import { useAuthenticationStore } from '../../feature/authentication/service/store/authentication.store';
import { AuthenticationGuard } from './authentication-guard';
import { Navigate } from 'react-router';

export function AppRoutes() {
  const isAuthenticated = useAuthenticationStore((s) => s.isAuthenticated);
  const load = useAuthenticationStore((s) => s.load);

  useEffect(() => {
    if (isAuthenticated === AuthenticationStatus.UNKNOWN) {
      console.log('Initialization: Loading authentication state...');
      load();
    }
  }, [isAuthenticated, load]);

  if (isAuthenticated === AuthenticationStatus.UNKNOWN) {
    return (
      <div className="h-screen w-screen flex flex-col items-center justify-center gap-8 bg-zinc-800">
        <ProgressSpinner />
        <span className="text-2xl text-white tracking-wide">Chargement de l'application...</span>
      </div>
    );
  }

  return (
    <BrowserRouter>
        <Routes>
          {/* Public routes */}
          <Route path="/authenticate" element={<AuthenticationView />} />

          {/* Protected routes */}
          <Route path="/" element={<AuthenticationGuard />}>
            <Route index element={<div>Welcome :D</div>} />
          </Route>

          {/* Catch all - redirect to home */}
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
    </BrowserRouter>
  );
}
