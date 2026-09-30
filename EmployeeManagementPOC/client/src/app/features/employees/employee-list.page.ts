import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, DestroyRef, effect, inject, signal } from '@angular/core';
import { takeUntilDestroyed, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { catchError, combineLatest, debounceTime, distinctUntilChanged, map, merge, of, switchMap, tap } from 'rxjs';

import { MANAGE_ROLES } from '../../core/auth/auth.models';
import { AuthService } from '../../core/auth/auth.service';
import { NotificationService } from '../../core/notifications/notification.service';
import { InitialsPipe } from '../../shared/pipes/initials.pipe';
import { DepartmentsService } from '../departments/departments.service';
import { Employee, EmployeeFilter, EmployeeSortField, EmployeeSuggestion } from './employee.models';
import { filterFromQuery, filterToQuery } from './employee-filter';
import { EmployeesService } from './employees.service';

const SEARCH_DEBOUNCE_MS = 400;
const SUGGEST_DEBOUNCE_MS = 150;
const MIN_SUGGEST_LENGTH = 2;

@Component({
  selector: 'app-employee-list-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, CurrencyPipe, DatePipe, InitialsPipe],
  templateUrl: './employee-list.page.html',
})
export class EmployeeListPage {
  private readonly employees = inject(EmployeesService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly canManage = inject(AuthService).hasAnyRole(MANAGE_ROLES);
  protected readonly sortOptions: { value: EmployeeSortField; label: string }[] = [
    { value: 'Name', label: 'Name' },
    { value: 'Salary', label: 'Salary' },
    { value: 'HireDate', label: 'Hire Date' },
  ];

  protected readonly departments = toSignal(inject(DepartmentsService).getAll().pipe(catchError(() => of([]))), {
    initialValue: [],
  });

  protected readonly filter = toSignal(this.route.queryParamMap.pipe(map(filterFromQuery)), { requireSync: true });

  private readonly reload = signal(0);
  protected readonly loading = signal(false);

  protected readonly result = toSignal(
    combineLatest([toObservable(this.filter), toObservable(this.reload)]).pipe(
      tap(() => this.loading.set(true)),
      switchMap(([filter]) => this.employees.search(filter).pipe(catchError(() => of(null)))),
      tap(() => this.loading.set(false)),
    ),
  );

  protected readonly form = inject(NonNullableFormBuilder).group({
    search: [''],
    departmentId: [null as number | null],
    isActive: [null as boolean | null],
    sortBy: ['Name' as EmployeeSortField],
    sortDescending: [false],
  });

  /** Autocomplete for the search box, served by the API's in-memory Trie index. */
  protected readonly suggestions = toSignal(
    this.form.controls.search.valueChanges.pipe(
      debounceTime(SUGGEST_DEBOUNCE_MS),
      map((value) => value.trim()),
      distinctUntilChanged(),
      switchMap((prefix) =>
        prefix.length < MIN_SUGGEST_LENGTH
          ? of([])
          : this.employees.suggest(prefix).pipe(catchError(() => of([]))),
      ),
    ),
    { initialValue: [] as EmployeeSuggestion[] },
  );

  constructor() {
    // URL → form (initial load and back/forward), without re-triggering navigation.
    effect(() => this.form.setValue(pickFormFields(this.filter()), { emitEvent: false }));

    // Form → URL. Typing is debounced; dropdowns apply immediately. Any change resets to page 1.
    const { search, ...selects } = this.form.controls;
    merge(
      search.valueChanges.pipe(debounceTime(SEARCH_DEBOUNCE_MS)),
      ...Object.values(selects).map((control) => control.valueChanges),
    )
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => this.applyFilter({ ...this.filter(), ...this.form.getRawValue(), page: 1 }));
  }

  protected goToPage(page: number): void {
    this.applyFilter({ ...this.filter(), page });
  }

  protected reset(): void {
    void this.router.navigate([], { relativeTo: this.route });
  }

  protected remove(employee: Employee): void {
    if (!confirm(`Are you sure you want to delete ${employee.fullName}?`)) {
      return;
    }

    this.employees.delete(employee.id).subscribe(() => {
      this.notifications.success('Employee deleted successfully.');
      this.reload.update((n) => n + 1);
    });
  }

  private applyFilter(filter: EmployeeFilter): void {
    void this.router.navigate([], { relativeTo: this.route, queryParams: filterToQuery(filter), replaceUrl: true });
  }
}

function pickFormFields({ search, departmentId, isActive, sortBy, sortDescending }: EmployeeFilter) {
  return { search, departmentId, isActive, sortBy, sortDescending };
}
