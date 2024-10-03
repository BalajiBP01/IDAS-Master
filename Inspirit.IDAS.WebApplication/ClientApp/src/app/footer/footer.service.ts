import { EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import 'rxjs/add/operator/map';
import { debug } from 'util';
import { debounce } from 'rxjs/operator/debounce';



@Injectable()
export class FooterService {

  isFootervisible: boolean = false;
 

  constructor() {

  }


  @Output() changefooter: EventEmitter<boolean> = new EventEmitter();
  

  updatefooter(isVisible: boolean) {
    this.isFootervisible = isVisible;
    this.changefooter.emit(this.isFootervisible);
  }

}
