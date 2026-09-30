import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { finalize } from 'rxjs';

import { LoadingService } from '../loading/loading.service';
import { SKIP_LOADING } from './api';

export const loadingInterceptor: HttpInterceptorFn = (request, next) => {
  if (request.context.get(SKIP_LOADING)) {
    return next(request);
  }

  const loading = inject(LoadingService);
  loading.begin();

  return next(request).pipe(finalize(() => loading.end()));
};
