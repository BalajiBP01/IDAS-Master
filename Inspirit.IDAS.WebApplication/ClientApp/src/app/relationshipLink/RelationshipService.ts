import { EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import { debug } from 'util';
import { ProfileRequest } from './../services/services';
import { PersonProfileComponent } from './../personProfile/personProfile.component';
@Injectable()
export class RelationshipService {

  request: ProfileRequest = new ProfileRequest();
  idnumber: any;
  constructor() {

  }
  @Output() idno: EventEmitter<string> = new EventEmitter();

  gotoProfile(idno: any) {
    this.idnumber = idno;
    this.idno.emit(this.idnumber);
  }

}
