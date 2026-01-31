import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'MiniCommerce';
  isLoginPage = false;

  constructor(
    public auth: AuthService,
    private router: Router
  ) {
    this.updateLoginPage();
    this.router.events.subscribe(() => this.updateLoginPage());
  }

  private updateLoginPage(): void {
    const url = this.router.url;
    this.isLoginPage = url === '/' || url === '' || url.startsWith('/?');
  }

  logout(): void {
    this.auth.logout();
  }
}
