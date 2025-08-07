import type { User, TokenModel } from "./nswag-api.g";
import store from "store2";

export enum AuthenticationStatus {
  AUTHENTICATED = "authenticated",
  UNAUTHENTICATED = "unauthenticated",
  UNKNOWN = "unknown",
}

export class CredentialsManager {
  private authenticationStatus: AuthenticationStatus =
    AuthenticationStatus.UNKNOWN;
  public get status(): AuthenticationStatus {
    return this.authenticationStatus;
  }

  private static readonly cacheKeyUser: string = "lynred:monitorix:user";
  private _user: User | null = null;
  public get user(): User | null {
    return this._user;
  }

  private static readonly accessTokenExpirationOffset = 0 * 1 * 60 * 1000; // 1 minute
  private static readonly cacheKeyAccessToken: string =
    "lynred:monitorix:access-token";
  private _accessToken: TokenModel | null = null;
  public get accessToken(): TokenModel | null {
    return this._accessToken;
  }

  private static readonly refreshTokenExpirationOffset = 0 * 5 * 60 * 1000; // 5 minutes
  private static readonly cacheKeyRefreshToken: string =
    "lynred:monitorix:refresh-token";
  private _refreshToken: TokenModel | null = null;
  public get refreshToken(): TokenModel | null {
    return this._refreshToken;
  }

  constructor() {}

  public isDataAvailable(): boolean {
    return (
      this.user != null &&
      this.accessToken != null &&
      this.accessToken.value != null &&
      this.refreshToken != null &&
      this.refreshToken.value != null
    );
  }

  public isAccessTokenValid(): boolean {
    return (
      this.accessToken != null &&
      this.accessToken.value != null &&
      this.accessToken.expiration != null &&
      Date.now() <
        new Date(this.accessToken.expiration).getTime() +
          CredentialsManager.accessTokenExpirationOffset
    );
  }

  public isRefreshTokenValid(): boolean {
    return (
      this.refreshToken != null &&
      this.refreshToken.value != null &&
      this.refreshToken.expiration != null &&
      Date.now() <
        new Date(this.refreshToken.expiration).getTime() +
          CredentialsManager.refreshTokenExpirationOffset
    );
  }

  /**
   * Saves the user credentials.
   * User undefined means we use the current user.
   */
  public saveCredentials({
    user,
    accessToken,
    refreshToken,
  }: {
    user?: User;
    accessToken: TokenModel;
    refreshToken: TokenModel;
  }): void {
    if (user !== undefined) {
      this._user = user;
      store.set(CredentialsManager.cacheKeyUser, JSON.stringify(user));
    }

    this.authenticationStatus = this._user
      ? AuthenticationStatus.AUTHENTICATED
      : AuthenticationStatus.UNAUTHENTICATED;

    this._accessToken = accessToken;
    store.set(
      CredentialsManager.cacheKeyAccessToken,
      JSON.stringify(accessToken)
    );

    this._refreshToken = refreshToken;
    store.set(
      CredentialsManager.cacheKeyRefreshToken,
      JSON.stringify(refreshToken)
    );
  }

  /**
   * Loads the user credentials from storage.
   * This method should be called on application startup to restore the authentication state.
   */
  public loadCredentialsFromStorage(): void {
    const rawUser = store.get(CredentialsManager.cacheKeyUser);
    const rawAccessToken = store.get(CredentialsManager.cacheKeyAccessToken);
    const rawRefreshToken = store.get(CredentialsManager.cacheKeyRefreshToken);

    this.saveCredentials({
      user: rawUser ? JSON.parse(rawUser) : null,
      accessToken: rawAccessToken ? JSON.parse(rawAccessToken) : null,
      refreshToken: rawRefreshToken ? JSON.parse(rawRefreshToken) : null,
    });
  }

  /**
   * Clears the user credentials and resets the authentication state.
   * This method should be called on logout or when the user is no longer authenticated.
   */
  public clear(): void {
    this.authenticationStatus = AuthenticationStatus.UNAUTHENTICATED;
    this._user = null;
    this._accessToken = null;
    this._refreshToken = null;

    store.remove(CredentialsManager.cacheKeyUser);
    store.remove(CredentialsManager.cacheKeyAccessToken);
    store.remove(CredentialsManager.cacheKeyRefreshToken);
  }
}
