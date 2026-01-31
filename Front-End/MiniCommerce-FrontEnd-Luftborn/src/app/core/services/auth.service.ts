import { Injectable } from '@angular/core';
import { OAuthService, AuthConfig } from 'angular-oauth2-oidc';
import { BehaviorSubject, firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthConfigService } from './auth-config.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly ssoEnabled$ = new BehaviorSubject<boolean>(false);
  private initialized = false;

  constructor(
    private oauthService: OAuthService,
    private authConfigService: AuthConfigService
  ) {}

  async init(): Promise<void> {
    if (this.initialized) return;

    const config = await firstValueFrom(
      this.authConfigService.getSsoConfig()
    );

    this.ssoEnabled$.next(config.enabled);

    if (config.enabled && config.authority && config.clientId) {

      const authConfig: AuthConfig = {
        issuer: config.authority,
        clientId: config.clientId,

        redirectUri: window.location.origin + '/',
        postLogoutRedirectUri: window.location.origin + '/',

        responseType: 'code',
        scope: 'openid profile email',

        strictDiscoveryDocumentValidation: false,
        showDebugInformation: !environment.production,
        disablePKCE: false,

        // Google token endpoint requires client_secret for Web application type (even with PKCE).
        dummyClientSecret: config.clientSecret ?? undefined,
      };

      if (config.audience) {
        authConfig.customQueryParams = { audience: config.audience };
      }

      this.oauthService.configure(authConfig);

      // Load discovery document from our API proxy to avoid CORS when calling Google directly
      const discoveryUrl = `${environment.apiUrl}/api/auth/openid-configuration`;
      await this.oauthService.loadDiscoveryDocument(discoveryUrl);
      await this.oauthService.tryLogin();

      // Silent refresh
      this.oauthService.setupAutomaticSilentRefresh();
    }

    this.initialized = true;
  }

  get isSsoEnabled(): boolean {
    return this.ssoEnabled$.value;
  }

  get isAuthenticated(): boolean {
    return this.oauthService.hasValidAccessToken();
  }

  login(): void {
    this.oauthService.initCodeFlow();
  }

  logout(): void {
    this.oauthService.logOut();
  }

  getAccessToken(): string | null {
    return this.oauthService.getAccessToken();
  }

  get claims(): Record<string, unknown> {
    return (this.oauthService.getIdentityClaims() as any) ?? {};
  }

  get name(): string {
    return (
      (this.claims['name'] as string) ??
      (this.claims['preferred_username'] as string) ??
      ''
    );
  }
}
