import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

import { InitialsPipe } from '../../shared/pipes/initials.pipe';
import { DEPARTMENT_VIEW_ROLES } from '../auth/auth.models';
import { AuthService } from '../auth/auth.service';

@Component({
  selector: 'app-shell',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, InitialsPipe],
  templateUrl: './shell.html',
})
export class Shell {
  protected readonly auth = inject(AuthService);
  protected readonly menuOpen = signal(false);
  protected readonly userMenuOpen = signal(false);
  protected readonly canViewDepartments = computed(() => this.auth.hasAnyRole(DEPARTMENT_VIEW_ROLES));
  protected readonly year = new Date().getFullYear();

  protected closeMenus(): void {
    this.menuOpen.set(false);
    this.userMenuOpen.set(false);
  }

  protected signOut(): void {
    this.closeMenus();
    this.auth.logout();
  }
}
