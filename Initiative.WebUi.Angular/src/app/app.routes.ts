import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/user/login.component';

export const routes: Routes = [
  { path: '', redirectTo: 'encounters', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', redirectTo: ''},
  { path: 'encounters', redirectTo: ''},
  { path: 'beastiary', redirectTo: ''},
  { path: '**', redirectTo: 'encounters' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}