import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UserSession } from '../models/session';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {}
  getToken(): string {
    return localStorage.getItem('token');
  }

  login(userName: string, password: string): Observable<UserSession> {
    return this.http.post<UserSession>(`${this.baseUrl}login`, {
      password: password,
      userid: userName
    });
  }
  getStatus(): Observable<UserSession> {
    return this.http.get<UserSession>(
      `${this.baseUrl}login?session=${localStorage.getItem('token')}`
    );
  }
}
