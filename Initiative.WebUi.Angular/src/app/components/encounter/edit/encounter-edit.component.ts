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
    }

    ngOnDestroy() {
        this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();
        this.autoSaveSub?.unsubscribe();
    }

    
  drop(event: CdkDragDrop<any>) {
    console.log("moved");
  }

  onAddCreature() {
    var newCreature = new CreatureModel();
    newCreature.Name = "Creature "+this.encounterModel.Creatures.length;
    newCreature.ArmorClass = 10;
    newCreature.HitPoints = 10;

    this.encounterModel.Creatures.push(newCreature);

    this.encounterService.setCreaturesInEncounter(this.encounterId, this.encounterModel.Creatures).subscribe();

  }

  onUpdateTitle(input: string) {
    var newTitle = input.trim();
    this.encounterService.renameEncounter(this.encounterId, newTitle).subscribe();
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

}
