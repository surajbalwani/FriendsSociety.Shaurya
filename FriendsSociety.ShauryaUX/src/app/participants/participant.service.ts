import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs'; 
import { applicationUser } from '../Infrastructure/participant.interface';

@Injectable({
  providedIn: 'root'
})
export class ParticipantService {
  private baseApiUri="https://localhost:7209/";
  constructor(private httpClient:HttpClient) {}
  getAllParticipants():Observable<any>  
  {
    return this.httpClient.get<any[]>(this.baseApiUri + '/api/ApplicationUser/')
  }

}
