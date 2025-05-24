import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/authService';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  imports: [CommonModule, FormsModule]
})
export class LoginComponent {
  email = '';
  password = '';
  error = '';

  constructor(public auth: AuthService) {}

  login() {
    this.error = '';
    this.auth.login(this.email, this.password).subscribe({
      next: res => this.auth.setToken(res.token),
      error: () => this.error = 'Login failed'
    });
  }

  logout() {
    this.auth.logout();
  }
}