import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LobbyClient } from '../../../signalR/LobbyClient';

@Component({
  selector: 'app-lobby-view',
  standalone: true,
  templateUrl: './lobby-view.component.html',
  styleUrl: './lobby-view.component.css',
  imports: [CommonModule, FormsModule, NgFor]
})
export class LobbyViewComponent implements OnInit, OnDestroy {
  creatureList: string[] = [];
  lobbyClient!: LobbyClient;
  roomCode: string = '';

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.roomCode = this.route.snapshot.paramMap.get('roomCode') ?? '';
    this.lobbyClient = new LobbyClient('https://localhost:7034/lobbyHub', this.roomCode);

    (this.lobbyClient as any).connection.off('CreatureList');
    this.lobbyClient['connection'].on('CreatureList', (creatures: string[]) => {
      this.creatureList = creatures;
    });

    this.lobbyClient.connect();
  }

  ngOnDestroy() {
    this.lobbyClient.disconnect();
  }
}