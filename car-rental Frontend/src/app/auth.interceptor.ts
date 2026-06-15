import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  let token = '';

  if (typeof window !== 'undefined') {
    token = localStorage.getItem('token') || '';
  }

  if (!token) {
    return next(req); // no token, send request as-is
  }

  const authReq = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(authReq);
};