import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-lobby-join',
  standalone: true,
  templateUrl: './lobby-join.component.html',
  styleUrl: './lobby-join.component.css',
  imports: [CommonModule, FormsModule]
})
export class LobbyJoinComponent {
  roomCode: string = '';

  constructor(private router: Router) {}

  joinLobby() {
    if (this.roomCode && this.roomCode.length >= 4) {
      this.router.navigate(['/lobby', this.roomCode]);
    }
  }
}