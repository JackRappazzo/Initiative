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
import { MatIcon, MatIconModule } from '@angular/material/icon'; // Add this to imports if not present


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
    RouterModule,
    MatIconModule // Ensure MatIconModule is imported
  ]
})
export class EncounterListComponent {

  isSubmitting = false;
  encounterList: EncounterListItemModel[] = [];

  constructor(private encounterService:EncounterService, private router:Router) {
    this.router = router;

    this.encounterService.getEncounters().subscribe(encounters=> { this.encounterList = encounters; console.log(this.encounterList);});
  }

  createEncounter() {
    this.isSubmitting = true;
    this.encounterService.createEncounter("New Encounter").subscribe({
      next: (encounter) => {
        this.encounterList.push(encounter);
        this.isSubmitting = false;
        this.router.navigate(['/encounter', encounter.EncounterId]);
      },
      error: (error) => {
        console.error('Error creating encounter:', error);
        this.isSubmitting = false;
      }
    });
  }

   confirmDelete(encounter: EncounterListItemModel) {
    if (confirm(`Are you sure you want to delete "${encounter.EncounterName}"?`)) {
      this.deleteEncounter(encounter);
    }
  }

  deleteEncounter(encounter: EncounterListItemModel) {
    this.encounterService.deleteEncounter(encounter.EncounterId).subscribe(() => {
      this.encounterList = this.encounterList.filter(e => e !== encounter);
    });
  }

}