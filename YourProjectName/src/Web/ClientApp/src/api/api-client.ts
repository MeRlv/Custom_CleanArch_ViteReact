import { CredentialsManager } from "./credentials-manager";
import { ApiException, Client, type User } from "./nswag-api.g";
import inflight from "promise-inflight";

const API_URL = import.meta.env.VITE_API_URL;

export class ApiClient extends Client {
  private _credsManager: CredentialsManager;

  constructor() {
    super(API_URL, {
      fetch: async (
        url: RequestInfo,
        init?: RequestInit
      ): Promise<Response> => {
        // Security requests short-circuit the fetch protocol
        if (typeof url === "string" && url.includes("/security")) {
          return window.fetch(url, init);
        }

        // PREVENTION : Refresh token if expired
        const token = await this.getValidAccessToken();
        const headers = {
          ...init?.headers,
          ...(token ? { Authorization: `Bearer ${token}` } : {}),
        };

        return await window.fetch(url, {
          ...init,
          headers,
        });
      },
    });

    this._credsManager = new CredentialsManager();
  }

  /**
   * Logs in the user and saves its credentials.
   * This method overrides the default login method to store the access and refresh tokens.
   */
  public async loginAndSaveCredentials(
    username: string,
    password: string
  ): Promise<User | null> {
    const response = await super.loginUser({
      username: username,
      password: password,
    });

    if (!response.user || !response.accessToken || !response.refreshToken) {
      throw new ApiException(
        "Login failed: Invalid response from server",
        400,
        "",
        {},
        null
      );
    }

    this._credsManager.saveCredentials({
      user: response.user,
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
    });

    return response.user;
  }

  /**
   * Ensures a valid access token is available, refreshing it if necessary.
   * @throws Throws an error if the token refresh process fails.
   */
  private async getValidAccessToken(
    forcedRefresh = false
  ): Promise<string | null> {
    const username = this._credsManager.user?.username ?? "anonymous";
    const inflightKey = `api-client-getValidAccessToken-${username}`;

    return inflight(inflightKey, async () => {
      // Check if the token is expired
      if (forcedRefresh || !this._credsManager.isAccessTokenValid()) {
        await this.refreshCredentials();
      }

      return this._credsManager.accessToken?.value ?? null;
    });
  }

  /**
   * Clears the credentials.
   * Navigate automatically to the login page after clearing credentials.
   */
  public clear(): void {
    this._credsManager.clear();
    if (window.location.pathname !== "/authenticate") {
      window.location.href = "/authenticate";
    }
  }

  /**
   * Refreshes the access token using the refresh token.
   * This method overrides the default refresh method to handle token storage.
   * Retry logic is implemented to handle transient errors.
   */
  public async refreshCredentials(): Promise<void> {
    const maxTry = 1;
    const refreshWithRetries = async (tryCount: number): Promise<void> => {
      try {
        const response = await super.refreshToken({
          user: this._credsManager.user ?? undefined,
          refreshToken: this._credsManager.refreshToken?.value ?? undefined,
        });

        if (!response.accessToken || !response.refreshToken) {
          throw new ApiException(
            "Refresh failed: Invalid response from server",
            400,
            "",
            {},
            null
          );
        }

        this._credsManager.saveCredentials({
          accessToken: response.accessToken,
          refreshToken: response.refreshToken,
        });
      } catch (error) {
        // Unauthorized error handling
        if (error instanceof ApiException && (error as ApiException).status === 401) {
          console.error(`Failed to refresh tokens:`, error);
          this.clear();
          throw error;
        }

        // Retry logic for others errors
        if (tryCount < maxTry) {
          console.warn(
            `Failed to refresh tokens (attempt ${tryCount + 1}):`,
            error
          );
          await new Promise((resolve) =>
            setTimeout(resolve, 1000 * (tryCount + 1))
          ); // Exponential delay
          return refreshWithRetries(tryCount + 1);
        }

        // Failed to refresh tokens
        console.error("Failed to refresh tokens:", error);
        this.clear();
        throw error;
      }
    };

    console.log(
      `Refreshing credentials for user ${
        this._credsManager.user?.username ?? "anonymous"
      }...`
    );
    return refreshWithRetries(0);
  }

  /**
   * Loads the user credentials from storage.
   */
  public async loadCredentialsFromStorage(): Promise<User | null> {
    this._credsManager.loadCredentialsFromStorage();

    // 1. Check data availability
    if (!this._credsManager.isDataAvailable()) return null;

    // 2. Check refresh token validity
    if (!this._credsManager.isRefreshTokenValid()) {
      this.clear();
      return null;
    }

    // 3. Refresh token unconditionally
    try {
      await this.refreshCredentials();
    } catch {
      return null;
    }

    // 4. Return user if everything is valid
    return this._credsManager.user;
  }
}
