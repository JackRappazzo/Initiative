// client/src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject} from 'rxjs';
import { EncounterListItemModel } from '../models/encounterListModel';
import { Observable,map } from 'rxjs';
import { EncounterModel } from '../models/encounterModel';
import { CreatureModel } from '../models/CreatureModel';

@Injectable({ providedIn: 'root' })
export class EncounterService {
  private apiUrl = 'https://localhost:7034/api/encounter';
  private tokenKey = 'auth_token';

  isLoggedIn$ = new BehaviorSubject<boolean>(this.hasToken());

  constructor(private http: HttpClient) {}

  public getEncounters() : Observable<EncounterListItemModel[]> {
    var headers = this.getHeaders();
    var response =  this.http.get<any[]>(`${this.apiUrl}`, { headers });

    var result = response.pipe(
        map(r=>r.map(
            e=> {
                var item = new EncounterListItemModel();
                item.EncounterId = e.encounterId;
                item.NumberOfCreatures = e.numberOfCreatures;
                item.EncounterName = e.encounterName;
                return item;
            }
        ))
    );
    return result;
  }

  public getEncounter(encounterId:string) : Observable<EncounterModel> {
    var headers = this.getHeaders();
    var response = this.http.get<any>(`${this.apiUrl}/${encounterId}`, {headers});

    var result = response.pipe(
      map(r=>{
        console.log(r);
        var encounter = new EncounterModel();   
        encounter.Id = r.id;
        encounter.Name = r.displayName ?? r.name; // fallback if displayName is missing

        var creatures:CreatureModel[] = r.creatures?.map((c: any) => {
          console.log(c);
          var creature = new CreatureModel();
          creature.ArmorClass = c.armorClass;
          creature.Name = c.name;
          creature.HitPoints = c.hitPoints;
          // Map other properties if needed
          return creature;
        }) ?? new Array<CreatureModel>();

        encounter.Creatures = creatures;
        
        return encounter;
      })
    );

    return result;

  }

  public setCreaturesInEncounter(encounterId:string, creatures:CreatureModel[])
  {
     const headers = this.getHeaders();
      // Map CreatureModel to the DTO expected by your API, if needed
      const body = creatures.map(c => ({
        armorClass: c.ArmorClass,
        name: c.Name,
        hitPoints: c.HitPoints
      }));

      // Adjust the URL and HTTP method if your API differs
      return this.http.post<any>(
        `${this.apiUrl}/${encounterId}/creatures`,
        body,
        { headers }
      );
  }

  public renameEncounter(encounterId:string, newName:string) : Observable<any> {
    const headers = this.getHeaders();
    const body = { newName: newName };
    return this.http.put<any>(
      `${this.apiUrl}/${encounterId}/setName`,
      body,
      { headers }
    );
  }

  private getHeaders() : HttpHeaders {
    var headers = new HttpHeaders().set('Authorization', `Bearer ${this.getToken()}`);
    return headers;
  }

private getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }
private hasToken(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
}