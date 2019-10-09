import { Injectable, Injector } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable()
export class AuthTokenInterceptor implements HttpInterceptor {
  private authService: AuthService;
  constructor(private injector: Injector) {}
  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    this.authService = this.injector.get(AuthService);
    const token = this.authService.getToken();
    const newreq = req.clone({
      withCredentials: true,
      setHeaders: {
        Authorization: token,
        'User-Agent': 'Anchorage-Viewer/0.1.0'
      }
    });

    return next.handle(newreq);
  }
}
