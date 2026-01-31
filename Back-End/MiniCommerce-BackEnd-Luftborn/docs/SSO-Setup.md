# SSO (Single Sign-On) Setup

The MiniCommerce API supports optional SSO via **OpenID Connect** and **JWT Bearer** tokens. You can use **Azure AD (Microsoft Entra ID)** or **ADFS** (recommended in the task) as the identity provider.

## Enable or disable SSO

- **SSO disabled (default):** Set `SSO:Enabled` to `false` in `appsettings.json`. All API endpoints are then accessible without a token.
- **SSO enabled:** Set `SSO:Enabled` to `true` and configure your identity provider (see below). All API endpoints then require a valid JWT in the `Authorization: Bearer <token>` header.

## Option 1: Azure AD (Microsoft Entra ID)

1. In **Azure Portal** → **Microsoft Entra ID** → **App registrations** → **New registration**:
   - Name: e.g. `MiniCommerce API`
   - Supported account types: as needed (e.g. Single tenant)
   - Redirect URI: leave empty for API-only

2. After creation, note:
   - **Application (client) ID** → use as `ClientId` and in `Audience` as `api://<client-id>`
   - **Directory (tenant) ID** → use in `Authority`

3. Expose an API:
   - **App registration** → **Expose an API** → **Add a scope**: e.g. `access_as_user` (e.g. `api://<client-id>/access_as_user`).

4. In `appsettings.json` (or `appsettings.Development.json`):

```json
"SSO": {
  "Enabled": true,
  "Authority": "https://login.microsoftonline.com/{tenant-id}/v2.0",
  "Audience": "api://{client-id}",
  "ClientId": "{client-id}",
  "MetadataAddress": "",
  "RequireHttpsMetadata": true
}
```

Replace `{tenant-id}` and `{client-id}` with your Tenant ID and Application (client) ID.

5. To obtain a token for testing (e.g. Swagger), use a separate **client app registration** (public client or SPA) and sign in; use the returned access token in Swagger **Authorize** as `Bearer <token>`.

## Option 2: ADFS (Active Directory Federation Services)

1. Ensure ADFS exposes an **OpenID Connect** endpoint (ADFS 4.0 / Windows Server 2016+).

2. Create an **Application Group** (or Relying Party Trust):
   - Identifier (Audience): e.g. `https://api.minicommerce.local/` or your API’s resource identifier.
   - Configure to issue **OpenID Connect** tokens (e.g. access_token as JWT).

3. Get the OpenID Connect metadata URL, typically:
   - `https://adfs.yourdomain.com/adfs/.well-known/openid-configuration`

4. In `appsettings.json` (or use the provided `appsettings.ADFS.json` as a template):

```json
"SSO": {
  "Enabled": true,
  "Authority": "https://adfs.yourdomain.com/adfs",
  "Audience": "https://api.minicommerce.local/",
  "ClientId": "your-relying-party-identifier",
  "MetadataAddress": "https://adfs.yourdomain.com/adfs/.well-known/openid-configuration",
  "RequireHttpsMetadata": true
}
```

5. Replace:
   - `https://adfs.yourdomain.com` with your ADFS base URL.
   - `Audience` and `ClientId` with the identifier(s) configured in ADFS for this API.

6. For development with self-signed certs, you can set `RequireHttpsMetadata` to `false` only in a development environment (not in production).

## API endpoints for SSO

| Endpoint            | Method | Auth    | Description                                      |
|---------------------|--------|--------|--------------------------------------------------|
| `/api/auth/config`  | GET    | None   | Returns whether SSO is enabled and config (e.g. authority, clientId) for the frontend. |
| `/api/auth/me`      | GET    | Bearer | Returns current user claims when SSO is enabled and a valid token is sent. |

When SSO is enabled, all other API routes (e.g. `/api/categories`, `/api/products`) require a valid JWT in the `Authorization: Bearer <token>` header.

## Testing with Swagger

1. When SSO is enabled, open Swagger UI and click **Authorize**.
2. Enter: `Bearer <your-access-token>` (with a space after `Bearer`).
3. Call any protected endpoint; the token will be validated by the configured authority (Azure AD or ADFS).

## Running with SSO configuration

- To use a dedicated config file (e.g. for ADFS), add it when running:
  - `dotnet run` and ensure the file is loaded (e.g. copy settings into `appsettings.Development.json`), or
  - Set environment variable: `ASPNETCORE_ENVIRONMENT=SSO` and add an `appsettings.SSO.json` that is loaded for that environment (or merge the SSO section into your main appsettings).
