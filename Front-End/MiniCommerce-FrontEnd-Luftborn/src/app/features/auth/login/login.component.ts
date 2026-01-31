import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent implements OnInit {
  returnUrl = '/home';

  constructor(
    public auth: AuthService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/home';
    if (this.auth.isSsoEnabled && this.auth.isAuthenticated) {
      const url = sessionStorage.getItem('loginReturnUrl') || this.returnUrl;
      sessionStorage.removeItem('loginReturnUrl');
      this.router.navigateByUrl(url);
    }
  }

  loginWithGoogle(): void {
    sessionStorage.setItem('loginReturnUrl', this.returnUrl);
    this.auth.login();
  }
}
