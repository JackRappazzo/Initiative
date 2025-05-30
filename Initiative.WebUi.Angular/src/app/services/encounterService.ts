// client/src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BehaviorSubject} from 'rxjs';
import { EncounterListItemModel } from '../models/encounterModel';
import { Observable,map } from 'rxjs';

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