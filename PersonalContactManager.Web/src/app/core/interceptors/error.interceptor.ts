import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      let detail = 'An unexpected error occurred.';

      if (err.status === 0) {
        detail = 'Cannot reach the server. Check your connection.';
      } else if (err.status === 400 && err.error?.errors) {
        // FluentValidation format: { errors: { Field: ["msg", ...] } }
        detail = (Object.values(err.error.errors) as string[][]).flat().join(' ');
      } else if (err.error?.error) {
        detail = err.error.error;
      } else if (err.error?.title) {
        detail = err.error.title;
      } else if (err.status === 404) {
        detail = 'Resource not found.';
      } else if (err.status === 409) {
        detail = 'This record already exists.';
      } else if (err.status >= 500) {
        detail = 'Server error. Please try again later.';
      }

      messageService.add({
        severity: 'error',
        summary: statusLabel(err.status),
        detail,
        life: 6000,
      });

      return throwError(() => err);
    })
  );
};

function statusLabel(status: number): string {
  const labels: Record<number, string> = {
    0: 'Connection Error',
    400: 'Validation Error',
    401: 'Unauthorized',
    403: 'Forbidden',
    404: 'Not Found',
    409: 'Conflict',
  };
  return labels[status] ?? (status >= 500 ? 'Server Error' : 'Error');
}
