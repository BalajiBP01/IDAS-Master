import { EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { HeaderNavComponent } from './header-nav.component';
import 'rxjs/add/operator/map';
import { debug } from 'util';


@Injectable()
export class HeaderService {

   
    isHeaderVisible: boolean = false;

    @Output() changeheader: EventEmitter<boolean> = new EventEmitter();

  updateheader(isheadervisible: boolean) {
   
        this.isHeaderVisible = isheadervisible;
        this.changeheader.emit(this.isHeaderVisible);
    }
}















