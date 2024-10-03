import { EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AsideNavComponent } from '../aside-nav/aside-nav.component';
import 'rxjs/add/operator/map';
import { debug } from 'util';


@Injectable()
export class AsideNavService {

    isSideNavVisible: boolean = false;

    @Output() change: EventEmitter<boolean> = new EventEmitter();
   
    toggle(isVisible: boolean) {
        this.change.emit(isVisible);
    }
}
