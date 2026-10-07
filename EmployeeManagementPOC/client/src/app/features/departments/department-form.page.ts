import { ChangeDetectionStrategy, Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { Observable } from 'rxjs';

import { NotificationService } from '../../core/notifications/notification.service';
import { FieldError } from '../../shared/forms/field-error';
import { applyServerErrors } from '../../shared/forms/server-errors';
import { Department, DepartmentRequest, DepartmentsService } from './departments.service';

// Mirrors the domain invariants enforced by the API.
const NAME_MAX = 100;
const DESCRIPTION_MAX = 500;

@Component({
  selector: 'app-department-form-page',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule, RouterLink, FieldError],
  templateUrl: './department-form.page.html',
})
export class DepartmentFormPage implements OnInit {
  private readonly departments = inject(DepartmentsService);
  private readonly notifications = inject(NotificationService);
  private readonly router = inject(Router);

  /** Route parameter; absent when creating. */
  readonly id = input<string>();

  protected readonly isEdit = computed(() => this.id() !== undefined);
  protected readonly saving = signal(false);
  protected readonly formError = signal<string | null>(null);

  protected readonly form = inject(NonNullableFormBuilder).group({
    name: ['', [Validators.required, Validators.maxLength(NAME_MAX)]],
    description: ['', Validators.maxLength(DESCRIPTION_MAX)],
  });

  ngOnInit(): void {
    const id = this.id();

    if (id !== undefined) {
      this.departments
        .getById(Number(id))
        .subscribe(({ name, description }) => this.form.setValue({ name, description: description ?? '' }));
    }
  }

  protected save(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, description } = this.form.getRawValue();

    this.saving.set(true);
    this.formError.set(null);

    this.persist({ name, description: description.trim() || null }).subscribe({
      next: () => {
        this.notifications.success(`Department ${this.isEdit() ? 'updated' : 'created'} successfully.`);
        void this.router.navigate(['/departments']);
      },
      error: (error: unknown) => {
        this.formError.set(applyServerErrors(this.form, error, { 'Department.DuplicateName': 'name' }));
        this.saving.set(false);
      },
    });
  }

  private persist(request: DepartmentRequest): Observable<Department> {
    const id = this.id();
    return id === undefined ? this.departments.create(request) : this.departments.update(Number(id), request);
  }
}
