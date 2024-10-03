import {  EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { HeaderNavComponent } from '../header-nav/header-nav.component';
import 'rxjs/add/operator/map';
import { debug } from 'util';



@Injectable()
export class headernavService {

  isHeaderVisible: boolean = false;
  currentUser: string;
  currentCompany: string;
  credits: number;// =600;
  totalPages: number;
  isLoaded: boolean = false;
  userName: string;
  client_logo: any;
  userid: string;
  isXDS: boolean = false;
 
  constructor() {

  }


  @Output() change: EventEmitter<boolean> = new EventEmitter();
  @Output() changePoints: EventEmitter<number> = new EventEmitter();
  @Output() changeName: EventEmitter<string> = new EventEmitter();
  @Output() changeloader: EventEmitter<boolean> = new EventEmitter();
  @Output() changeLogo: EventEmitter<string> = new EventEmitter();
  @Output() changeisXDS: EventEmitter<boolean> = new EventEmitter();

  toggle(isVisible: boolean) {
    this.isHeaderVisible = isVisible;
    this.change.emit(this.isHeaderVisible);
  }

  updatePoints(points: number) {
   
    this.credits = points;
    this.changePoints.emit(this.credits);
  }

  updateUserName(name: any)
  {
      this.userName = name;
      this.changeName.emit(this.userName);
  }

  updateloader(load: boolean)
  {
      this.isLoaded = load;
      this.changeloader.emit(this.isLoaded);
  }

  updateClient_logo(client_logo: any) {
    this.client_logo = client_logo;
    this.changeLogo.emit(this.client_logo);
  }

  updateisXDS(isXDS: any) {
    this.isXDS = isXDS;
    this.changeisXDS.emit(this.isXDS);
  }

  

  
}
