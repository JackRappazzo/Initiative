import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule, NgFor } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { LobbyClient } from '../../../signalR/LobbyClient';
import { LobbyState } from '../../../signalR/LobbyState';

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
  lobbyMode: 'Waiting' | 'InProgress' = 'Waiting';
  turnNumber: number = 1;
  currentTurnIndex: number = 0;

  constructor(private route: ActivatedRoute) {}

  ngOnInit() {
    this.roomCode = this.route.snapshot.paramMap.get('roomCode') ?? '';
    this.lobbyClient = new LobbyClient('https://localhost:7034/lobby', this.roomCode);

    // Subscribe to state updates and update the creature list and lobby state
    this.lobbyClient.receivedLobbyState$.subscribe((state: LobbyState) => {
        console.log("Lobby state received:", state, typeof state);

        // Defensive: use lower-case property names if needed
        const creatures = state.Creatures ?? [];
        const currentIndex = state.CurrentCreatureIndex ?? 0;

        // Reorder the creature list so the current creature is first, followed by the rest in order
        if (creatures.length > 0 && currentIndex >= 0 && currentIndex < creatures.length) {
            this.creatureList = [
                ...creatures.slice(currentIndex),
                ...creatures.slice(0, currentIndex)
            ];
        } else {
            this.creatureList = creatures;
        }

        this.lobbyMode = state.CurrentMode ?? 'Waiting';
        this.turnNumber = state.CurrentTurn ?? 1;
        this.currentTurnIndex = state.CurrentCreatureIndex ?? 0;
    });

    this.lobbyClient.connect().then(() => {
      console.log("Connected to lobby");
      this.lobbyClient.joinLobby(this.roomCode).then(() => {
        console.log("Joined lobby:", this.roomCode);
        this.lobbyClient.getLobbyState();
      })
      .catch(err => {
        console.error("Failed to join lobby:", err);
      });
    });
  }

  ngOnDestroy() {
    this.lobbyClient.disconnect();
  }
}