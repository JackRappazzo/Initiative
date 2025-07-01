import { Component } from "@angular/core";
import { EncounterService } from "../../../services/encounterService";
import { ActivatedRoute } from "@angular/router";
import { EncounterModel } from "../../../models/encounterModel";
import { MaterialModule } from "../../../modules/material.module";
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { CreatureModel } from "../../../models/CreatureModel";
import { NgFor, NgIf } from "@angular/common";
import { FormsModule, NgModel } from "@angular/forms";
import { interval, Subscription } from "rxjs";
import { LobbyClient } from "../../../signalR/LobbyClient";


@Component(
    {
        selector: "app-encounter-edit",
        standalone: true,
        styleUrl: "./encounter-edit.component.css",     
        templateUrl: "./encounter-edit.component.html",
        imports: [MaterialModule, DragDropModule, NgFor, NgIf, FormsModule]
    }
)
export class EncounterEditComponent{
    encounterId!:string;
    editingName:boolean = false;

    encounterModel:EncounterModel = new EncounterModel();
    private autoSaveSub?: Subscription;

    lobbyClient!: LobbyClient;
    lobbyMode: 'Waiting' | 'InProgress' = 'Waiting';
    currentTurnIndex = 0;
    turnNumber = 1;

    constructor(private encounterService:EncounterService, private route:ActivatedRoute) {
        this.encounterId = this.route.snapshot.paramMap.get('encounterId') ?? "";
        this.encounterService.getEncounter(this.encounterId)
            .subscribe(encounter=> { console.log(encounter); return this.encounterModel = encounter; });

        // Start periodic auto-save every 10 seconds
        this.autoSaveSub = interval(10000).subscribe(() => {
            if (this.encounterModel.Creatures.length > 0) {
                this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
            }
        });

        // Initialize LobbyClient (replace with your actual hub URL and room code logic)
        const hubUrl = "https://your-api-url/lobbyhub";
        const roomCode = this.encounterId; // Or however you map encounter to lobby
        this.lobbyClient = new LobbyClient(hubUrl, roomCode);
        this.lobbyClient.connect();
    }

    ngOnDestroy() {
        this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
        this.autoSaveSub?.unsubscribe();
        this.lobbyClient.disconnect();
    }

    // Call this after every API update
    private sendLobbyState() {
        const state = {
            ...this.encounterModel,
            lobbyMode: this.lobbyMode,
            currentTurnIndex: this.currentTurnIndex,
            turnNumber: this.turnNumber
        };
        this.lobbyClient.setEncounterState(state);
    }

    
  drop(event: CdkDragDrop<any>) {
    moveItemInArray(
        this.encounterModel.Creatures,
        event.previousIndex,
        event.currentIndex
    );
    this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe(() => {
        this.sendLobbyState();
    });
  }

  onAddCreature() {
    var newCreature = new CreatureModel();
    newCreature.Name = "Creature "+this.encounterModel.Creatures.length;
    newCreature.ArmorClass = 10;
    newCreature.HitPoints = 10;
    newCreature.MaxHitPoints = 10;

    this.encounterModel.Creatures.push(newCreature);

    this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe(() => {
        this.sendLobbyState();
    });

  }

  onUpdateTitle(input: string) {
    var newTitle = input.trim();
    this.encounterService.renameEncounter(this.encounterId, newTitle).subscribe();
    this.encounterModel.Name = newTitle;
  }


  updateHitpoints(creature:CreatureModel, inputField: HTMLInputElement)
  {
    var input = inputField.value;

     if (!input) return;

      const trimmed = input.trim();
      let newHp = creature.HitPoints;

      //Check for signs for simple addition or subtraction
      if (/^[+-]\d+$/.test(trimmed)) {
        newHp += parseInt(trimmed, 10);
      }
      //Set raw value
       else if (/^\d+$/.test(trimmed)) {
        newHp = parseInt(trimmed, 10);
      } 
      //xdx formulae. Needs improvement, this isn't quite expressive enough
      else if (/^\d+d\d+$/i.test(trimmed)) {
        const [count, sides] = trimmed.toLowerCase().split('d').map(Number);
        const roll = Array.from({ length: count })
          .map(() => Math.floor(Math.random() * sides) + 1)
          .reduce((a, b) => a + b, 0);
        newHp += roll;
      } else {
        console.warn('Invalid HP input:', input);
        return;
      }

      creature.HitPoints = Math.max(0, newHp);

      inputField.value = creature.HitPoints.toString();
      this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
  }

  selectInput(input: HTMLInputElement) {
    setTimeout(() => input.select(), 0);
  }

  setCreaturesOnService() {
    console.log("Setting creatures on service");
    this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
  }

  deleteCreature(creature: CreatureModel) {
    const index = this.encounterModel.Creatures.indexOf(creature);
    if (index > -1) {
        this.encounterModel.Creatures.splice(index, 1);
        this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
    }
  }

  sortByInitiative() {
    this.encounterModel.Creatures.sort((a, b) => {
      // Sort descending by Initiative + InitiativeModifier
      const aInit = (a.Initiative ?? 0) + (a.InitiativeModifier ?? 0);
      const bInit = (b.Initiative ?? 0) + (b.InitiativeModifier ?? 0);
      return bInit - aInit;
    });
    this.setCreaturesOnService();
  }

  // Start/Stop Encounter
  startEncounter() {
    this.lobbyMode = 'InProgress';
    this.currentTurnIndex = 0;
    this.turnNumber = 1;
    this.lobbyClient.startEncounter(this.encounterModel.Creatures.map(c => c.Name));
    this.sendLobbyState();
  }

  endEncounter() {
    this.lobbyMode = 'Waiting';
    this.lobbyClient.endEncounter();
    this.sendLobbyState();
  }

  // Advance turn logic
  nextTurn() {
    if (this.encounterModel.Creatures.length === 0) return;
    this.currentTurnIndex++;
    if (this.currentTurnIndex >= this.encounterModel.Creatures.length) {
        this.currentTurnIndex = 0;
        this.turnNumber++;
    }
    this.sendLobbyState();
    this.lobbyClient.sendNextTurn();
  }

}
