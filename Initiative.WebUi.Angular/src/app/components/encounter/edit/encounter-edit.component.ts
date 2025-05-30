import { Component } from "@angular/core";
import { EncounterService } from "../../../services/encounterService";
import { ActivatedRoute } from "@angular/router";
import { EncounterModel } from "../../../models/encounterModel";
import { MaterialModule } from "../../../modules/material.module";
import { CdkDragDrop, DragDropModule, moveItemInArray } from '@angular/cdk/drag-drop';
import { CreatureModel } from "../../../models/CreatureModel";
import { NgFor } from "@angular/common";

@Component(
    {
        selector: "app-encounter-edit",
        standalone: true,
        styleUrl: "./encounter-edit.component.css",     
        templateUrl: "./encounter-edit.component.html",
        imports: [MaterialModule, DragDropModule, NgFor]
    }
)
export class EncounterEditComponent{
    encounterId!:string;

    encounterModel:EncounterModel = new EncounterModel();

    constructor(private encounterService:EncounterService, private route:ActivatedRoute) {
        this.encounterId = this.route.snapshot.paramMap.get('encounterId') ?? "";
        this.encounterService.getEncounter(this.encounterId)
            .subscribe(encounter=> { console.log(encounter); return this.encounterModel = encounter; });
    }

    
  drop(event: CdkDragDrop<any>) {
    console.log("moved");
  }


}
