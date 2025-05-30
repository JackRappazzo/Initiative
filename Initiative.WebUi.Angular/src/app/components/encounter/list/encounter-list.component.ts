import { Component } from '@angular/core';
import { FormsModule, FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import { CommonModule, NgFor } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { Router, RouterModule } from '@angular/router';
import { EncounterListItemModel } from '../../../models/encounterListModel';
import { EncounterService } from '../../../services/encounterService';

@Component({
  selector: 'app-encounter-list',
  standalone: true,
  templateUrl: './encounter-list.component.html',
  styleUrl: './encounter-list.component.css',
  imports: 
  [
    CommonModule,
    FormsModule, 
    ReactiveFormsModule, 
    MatCardModule, 
    MatFormFieldModule, 
    MatInputModule,
    MatButtonModule,
    NgFor,
    RouterModule
  ]
})
export class EncounterListComponent {

  isSubmitting = false;
  encounterList: EncounterListItemModel[] = [];

  constructor(private encounterService:EncounterService, private router:Router) {
    this.router = router;

    this.encounterService.getEncounters().subscribe(encounters=> { this.encounterList = encounters; console.log(this.encounterList);});
  }

  
}