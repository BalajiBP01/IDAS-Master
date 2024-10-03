import { Observable } from "rxjs/Observable";
import { Component, OnInit, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { HeaderService } from './headerService';

declare let mLayout: any;

@Component({

    selector: 'app-header-nav',
  templateUrl: './header-nav.component.html',
  encapsulation: ViewEncapsulation.None,
})
export class HeaderNavComponent implements OnInit, AfterViewInit {
 
    _visibleheader: Observable<boolean> = Observable.of(true);
    constructor(public headerService: HeaderService) {
    }
    ngOnInit() {
      this.headerService.changeheader.subscribe(isheadervisible => {
       
            this._visibleheader = Observable.of(isheadervisible);
        });
  }
  ngAfterViewInit() {

    mLayout.initHeader();

  }
}
