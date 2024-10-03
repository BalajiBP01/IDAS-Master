import { Component, OnInit, ViewEncapsulation, AfterViewInit } from '@angular/core';
import { SecurityService, Menu } from '../services/services';
import { Observable } from "rxjs/Observable";
import { AsideNavService } from './aside-nav.service';
import {Router } from '@angular/router';
import { async } from "@angular/core/testing";
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../popup/popup.component';
import { PopupService } from '../popup/popupService';



declare let mLayout: any;
@Component({
    selector: "app-aside-nav",
    templateUrl: "./aside-nav.component.html",
    encapsulation: ViewEncapsulation.None,
})
export class AsideNavComponent implements OnInit {
    menus: Menu[];
    userId: any;
    valid: boolean = false;
    success: boolean;

    _visible: Observable<boolean> = Observable.of(true);
    _userID: Observable<string> = Observable.of("");

    constructor(public securityService: SecurityService, public sidenavservice: AsideNavService, public router: Router, private modalService: NgbModal, public popupservice: PopupService ) {
    }
    ngOnInit() {
      
        this.sidenavservice.change.subscribe(issidenavVisible => {
            this._visible = Observable.of(issidenavVisible);
            this.userId = localStorage.getItem('userid');
            if (this.userId) {
               this.securityService.getUserMenu(this.userId).subscribe((result) => {
                  this.menus = result;
               });
            }
      });

       

       
  }
  ngAfterViewInit() {

    mLayout.initAside();

  }
    logout() {
        let ngbModalOptions: NgbModalOptions = {
            backdrop: 'static',
            keyboard: false
        };
      
        const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
        modalRef.componentInstance.message = "Do you want to Log off?";
        modalRef.componentInstance.isconfirm = true;

        this.popupservice.buttonchange.subscribe((credits = this.success) => {
            this.success = credits;
            if (this.success) {
                localStorage.removeItem('userid');
              localStorage.removeItem('username');
              localStorage.removeItem('custsearch');
                this.router.navigate(['/adminlogin']);
            }
        });
    }
}

