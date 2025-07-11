import { Injectable } from '@angular/core';
import { HttpEvent, HttpInterceptor, HttpHandler, HttpRequest, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { Observable, throwError, from } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { AuthService } from '../authService';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;

  constructor(private authService: AuthService, private http: HttpClient) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const token = this.authService.getToken();
    let authReq = req;
    if (token) {
      authReq = req.clone({
        setHeaders: { Authorization: `Bearer ${token}` }
      });
    }

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && !this.isRefreshing) {
          this.isRefreshing = true;
          // Call the refresh endpoint
          return this.http.post<{ accessToken: string }>('https://localhost:7034/api/admin/refresh', {}, {withCredentials: true}).pipe(
            switchMap((response: any) => {
              this.isRefreshing = false;
              this.authService.setToken(response.accessToken);
              // Retry the original request with the new token
              const newReq = req.clone({
                setHeaders: { Authorization: `Bearer ${response.accessToken}` }
              });
              this.authService.setToken(response.accessToken);
              return next.handle(newReq);
            }),
            catchError(err => {
              this.isRefreshing = false;
              this.authService.logout();
              return throwError(() => err);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
}