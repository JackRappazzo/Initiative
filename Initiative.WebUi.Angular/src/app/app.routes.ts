import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/user/login.component';
import { EncounterListComponent } from './components/encounter/list/encounter-list.component';

export const routes: Routes = [
  { path: '', redirectTo: 'encounters', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', redirectTo: ''},
  { path: 'encounters', component: EncounterListComponent},
  { path: 'beastiary', redirectTo: ''},
  { path: '**', redirectTo: 'encounters' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}