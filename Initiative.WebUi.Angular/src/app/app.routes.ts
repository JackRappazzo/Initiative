import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/user/login.component';
import { EncounterListComponent } from './components/encounter/list/encounter-list.component';
import { EncounterEditComponent } from './components/encounter/edit/encounter-edit.component';
import { LobbyJoinComponent } from './components/lobby/join/lobby-join.component';
import { LobbyViewComponent } from './components/lobby/view/lobby-view.component';

export const routes: Routes = [
  { path: '', redirectTo: 'encounters', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'signup', redirectTo: ''},
  { path: 'encounters', component: EncounterListComponent},
  { path: 'encounters/:encounterId', component: EncounterEditComponent},
  { path: 'beastiary', redirectTo: ''},
  { path: 'lobby', component: LobbyJoinComponent },
  { path: 'lobby/:roomCode', component: LobbyViewComponent },
  { path: '**', redirectTo: 'encounters' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}