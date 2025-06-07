// client/src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, catchError, map, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7034/api/admin';
  private tokenKey = 'auth_token';

  isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  login(username: string, password: string) {
    return this.http.post<{ success:Boolean, jwt: string }>(`${this.apiUrl}/login`, { emailAddress: username, password: password }, { withCredentials: true })
        .pipe(
        map(response =>  ({
          success: response.success,
          token: response.jwt,
        })));
    }

  register(email: string, password: string) {
        return this.http.post(`${this.apiUrl}/register`, { emailAddress: email, password}, {observe: 'response'})
        .pipe(      
            map(response => response.status === 200),
            catchError(() => of(false)));
  }

  logout() {
    localStorage.removeItem(this.tokenKey);
    this.isLoggedIn$.next(false);
  }

  setToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
    this.isLoggedIn$.next(true);
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}