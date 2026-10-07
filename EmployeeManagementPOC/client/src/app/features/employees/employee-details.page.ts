import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, input, numberAttribute } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { catchError, of, switchMap } from 'rxjs';

import { MANAGE_ROLES } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { NotificationService } from '../../core/notifications/notification.service';
import { Employee } from './employee.models';
import { EmployeesService } from './employees.service';

@Component({
  selector: 'app-employee-details-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, CurrencyPipe, DatePipe],
  templateUrl: './employee-details.page.html',
})
export class EmployeeDetailsPage {
  private readonly employees = inject(EmployeesService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);

  readonly id = input.required({ transform: numberAttribute });

  protected readonly canManage = inject(AuthService).hasAnyRole(MANAGE_ROLES);

  protected readonly employee = toSignal(
    toObservable(this.id).pipe(switchMap((id) => this.employees.getById(id).pipe(catchError(() => of(null))))),
  );

  protected remove(employee: Employee): void {
    if (!confirm(`Are you sure you want to delete ${employee.fullName}?`)) {
      return;
    }

    this.employees.delete(employee.id).subscribe(() => {
      this.notifications.success('Employee deleted successfully.');
      void this.router.navigate(['/employees']);
    });
  }
}
