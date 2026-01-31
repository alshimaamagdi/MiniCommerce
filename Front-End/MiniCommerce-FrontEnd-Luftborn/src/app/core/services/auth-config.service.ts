import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

export interface SsoConfig {
  enabled: boolean;
  authority: string | null;
  clientId: string | null;
  clientSecret: string | null;
  audience: string | null;
}

@Injectable({ providedIn: 'root' })
export class AuthConfigService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getSsoConfig(): Observable<SsoConfig> {
    return this.http.get<SsoConfig>(`${this.apiUrl}/api/auth/config`);
  }
}
