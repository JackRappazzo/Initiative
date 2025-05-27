import { Component } from '@angular/core';
import { FormsModule, FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import { AuthService } from '../../services/authService';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  imports: 
  [
    CommonModule,
    FormsModule, 
    ReactiveFormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule,
    MatButtonModule,
  ]
})
export class LoginComponent {

  loginForm: FormGroup;
  isSubmitting = false;

  constructor(public auth: AuthService, private fb: FormBuilder, private router:Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
    this.router = router;
  }

  login() {
    if(this.loginForm.invalid) return;

    this.isSubmitting = true;
    const { email, password} = this.loginForm.value;

    this.auth.login(email, password).subscribe({
      next: res => { 
        this.auth.setToken(res.token); 
        this.router.navigate(["/"]);
      },
      error: () => alert("Login failed")
    });
  }

  logout() {
    this.auth.logout();
  }
}