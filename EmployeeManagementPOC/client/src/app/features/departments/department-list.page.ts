import { DecimalPipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, of, switchMap } from 'rxjs';

import { MANAGE_ROLES } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { NotificationService } from '../../core/notifications/notification.service';
import { FilterByPipe } from '../../shared/pipes/filter-by.pipe';
import { InitialsPipe } from '../../shared/pipes/initials.pipe';
import { PluralizePipe } from '../../shared/pipes/pluralize.pipe';
import { Department, DepartmentsService } from './departments.service';

@Component({
  selector: 'app-department-list-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterLink, DecimalPipe, FilterByPipe, InitialsPipe, PluralizePipe],
  templateUrl: './department-list.page.html',
})
export class DepartmentListPage {
  private readonly departmentsService = inject(DepartmentsService);
  private readonly notifications = inject(NotificationService);

  protected readonly canManage = inject(AuthService).hasAnyRole(MANAGE_ROLES);
  protected readonly searchKeys: (keyof Department)[] = ['name', 'description'];
  protected readonly search = signal('');

  private readonly reload = signal(0);

  protected readonly departments = toSignal(
    toObservable(this.reload).pipe(
      switchMap(() => this.departmentsService.getAll().pipe(catchError(() => of([] as Department[])))),
    ),
    { initialValue: [] },
  );

  protected readonly totalEmployees = computed(() =>
    this.departments().reduce((sum, department) => sum + department.employeesCount, 0),
  );

  protected readonly averageEmployees = computed(() =>
    this.departments().length ? this.totalEmployees() / this.departments().length : 0,
  );

  protected remove(department: Department): void {
    if (!confirm(`Are you sure you want to delete the ${department.name} department?`)) {
      return;
    }

    // A 409 (department still has employees) is reported by the error interceptor.
    this.departmentsService.delete(department.id).subscribe(() => {
      this.notifications.success('Department deleted successfully.');
      this.reload.update((n) => n + 1);
    });
  }
}
