import { EventEmitter, Input, Output, Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { headernavService } from '../header-nav/header-nav.service';
import 'rxjs/add/operator/map';
import { debug } from 'util';


@Injectable()
export class PopupService {

    isSideNavVisible: boolean = false;

    @Output() buttonchange: EventEmitter<boolean> = new EventEmitter();

    updatenavigation(isVisible: boolean) {
        this.buttonchange.emit(isVisible);
    }
}
