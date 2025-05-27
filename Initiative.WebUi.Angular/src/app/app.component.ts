import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { LoginComponent } from "./components/user/login.component";
import { MainLayoutComponent } from './layouts/main-layout.component';

@Component({
  selector: 'app-root',
  imports: [MainLayoutComponent],
  standalone: true,
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Initiative';
}
