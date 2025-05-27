import { Component } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { NgIf, AsyncPipe } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../services/authService';

@Component({
  standalone: true,
  selector: 'app-main-layout',
  imports: [
    RouterOutlet,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatListModule,
    NgIf,
    AsyncPipe

  ],
  templateUrl: './main-layout.component.html',
  styleUrls: ['./main-layout.component.css']
})


export class MainLayoutComponent {
    isLoggedIn = false;

      constructor(public auth: AuthService, private router: Router) {
           this.auth.isLoggedIn$.subscribe(value => this.isLoggedIn = value);
      }
     logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}