import { formatDate } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, input, LOCALE_ID, OnInit, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { catchError, Observable, of } from 'rxjs';

import { NotificationService } from '../../core/notifications/notification.service';
import { FieldError } from '../../shared/forms/field-error';
import { applyServerErrors } from '../../shared/forms/server-errors';
import { DepartmentsService } from '../departments/departments.service';
import { Employee, EmployeeRequest } from './employee.models';
import { EmployeesService } from './employees.service';

// Mirrors the domain invariants enforced by the API.
const NAME_MAX = 50;
const EMAIL_MAX = 150;
const PHONE_MAX = 30;
const SALARY_MAX = 1_000_000;

/** Create (`/employees/new`) and edit (`/employees/:id/edit`) share one form. */
@Component({
  selector: 'app-employee-form-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, FieldError],
  templateUrl: './employee-form.page.html',
})
export class EmployeeFormPage implements OnInit {
  private readonly employees = inject(EmployeesService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);
  private readonly locale = inject(LOCALE_ID);

  /** Route parameter; absent when creating. */
  readonly id = input<string>();

  protected readonly isEdit = computed(() => this.id() !== undefined);
  protected readonly saving = signal(false);
  protected readonly formError = signal<string | null>(null);

  protected readonly departments = toSignal(inject(DepartmentsService).getAll().pipe(catchError(() => of([]))), {
    initialValue: [],
  });

  protected readonly form = inject(NonNullableFormBuilder).group({
    firstName: ['', [Validators.required, Validators.maxLength(NAME_MAX)]],
    lastName: ['', [Validators.required, Validators.maxLength(NAME_MAX)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(EMAIL_MAX)]],
    phone: ['', [Validators.maxLength(PHONE_MAX), Validators.pattern(/^\+?[0-9\s\-()]*$/)]],
    salary: [0, [Validators.required, Validators.min(0), Validators.max(SALARY_MAX)]],
    hireDate: [this.toDateInput(new Date()), Validators.required],
    departmentId: [null as number | null, Validators.required],
    isActive: [true],
  });

  ngOnInit(): void {
    const id = this.id();

    if (id !== undefined) {
      this.employees.getById(Number(id)).subscribe((employee) => this.populate(employee));
    }
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving.set(true);
    this.formError.set(null);

    this.persist(this.toRequest()).subscribe({
      next: (employee) => {
        this.notifications.success(`Employee ${this.isEdit() ? 'updated' : 'created'} successfully.`);
        void this.router.navigate(['/employees', employee.id]);
      },
      error: (error: unknown) => {
        this.formError.set(
          applyServerErrors(this.form, error, {
            'Employee.DuplicateEmail': 'email',
            'Department.NotFound': 'departmentId',
          }),
        );
        this.saving.set(false);
      },
    });
  }

  private persist(request: EmployeeRequest): Observable<Employee> {
    const id = this.id();
    return id === undefined ? this.employees.create(request) : this.employees.update(Number(id), request);
  }

  private populate(employee: Employee): void {
    this.form.setValue({
      firstName: employee.firstName,
      lastName: employee.lastName,
      email: employee.email,
      phone: employee.phone ?? '',
      salary: employee.salary,
      hireDate: this.toDateInput(new Date(employee.hireDate)),
      departmentId: employee.departmentId,
      isActive: employee.isActive,
    });
  }

  private toRequest(): EmployeeRequest {
    const value = this.form.getRawValue();

    return {
      ...value,
      phone: value.phone.trim() || null,
      departmentId: value.departmentId!,
    };
  }

  private toDateInput(date: Date): string {
    return formatDate(date, 'yyyy-MM-dd', this.locale);
  }
}
