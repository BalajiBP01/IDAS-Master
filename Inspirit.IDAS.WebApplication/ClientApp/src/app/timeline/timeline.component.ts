import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
    selector: 'timeline',
    templateUrl: './timeline.component.html'
})

export class TimeLineComponent {

   
    @Input() contacts: any;
    
  constructor(public headerservice: headernavService) {
    this.headerservice.toggle(true);
        let c = this.contacts;
    }
}

