export type ImmerSet<T> = (partial: Partial<T> | ((state: T) => void)) => void;
export type ImmerGet<T> = () => T;

export type ImmerSetProps<T> = Partial<T> | ((state: T) => void);
export type ImmerSetAsync<T> = (data: ImmerSetProps<T>, expectedToken: string) => void;
