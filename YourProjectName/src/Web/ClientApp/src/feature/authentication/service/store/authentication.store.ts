import { create } from 'zustand';
import type { AuthenticationStore } from '../../domain/store/authentication.store';
import { immer } from 'zustand/middleware/immer';
import { defaultAuthenticationState } from './authentication.state';
import { createAuthenticationActions } from './authentication.actions';

export const useAuthenticationStore = create<AuthenticationStore>()(
  immer<AuthenticationStore>((set, get) => ({
    ...defaultAuthenticationState,
    ...createAuthenticationActions(set, get),

    clear: () => set({ ...defaultAuthenticationState }),
  }))
);
