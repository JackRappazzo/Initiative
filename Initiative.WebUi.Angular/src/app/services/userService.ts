import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface UserInformation {
  displayName: string;
  roomCode: string;
}

@Injectable({ providedIn: 'root' })
export class UserService {
  private apiUrl = 'https://localhost:7034/api/user';
  private tokenKey = 'auth_token';

  constructor(private http: HttpClient) {}

  getUserInformation(): Observable<UserInformation> {
    const headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return this.http.get<UserInformation>(`${this.apiUrl}/information`, { headers });
  }

  private getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
}