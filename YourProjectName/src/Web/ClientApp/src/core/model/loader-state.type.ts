export type LoadingState = {
  isLoading: boolean;
  isLoaded: boolean;
};

export const LoadingStates = {
  INITIAL: Object.freeze({ isLoading: false, isLoaded: false }) as LoadingState,
  LOADING: Object.freeze({ isLoading: true, isLoaded: false }) as LoadingState,
  LOADED: Object.freeze({ isLoading: false, isLoaded: true }) as LoadingState,
};
