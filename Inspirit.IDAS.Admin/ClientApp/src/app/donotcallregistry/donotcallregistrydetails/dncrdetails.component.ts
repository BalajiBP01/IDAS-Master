import { Component, OnInit, OnDestroy, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { DoNotCallRegistryService, DoNotCallRegistry ,UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { NgxEditorModule } from 'ngx-editor';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-dncrdetails',
  templateUrl: './dncrdetails.component.html',
})

export class donotcallregistrydetailsComponent {
  currentObj: DoNotCallRegistry = new DoNotCallRegistry();
  id: any;
  private sub: any;
  mode: string = "view";
  readonly: boolean = false;
  datestring: string;
  isread: boolean;
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private route: ActivatedRoute, private donotcallService: DoNotCallRegistryService, private datePipe: DatePipe, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {
      this.asideNavService.toggle(true);

      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Donot Call Registry").subscribe(result => {
          this.userper = result;
          if (this.userper == null || this.userper.viewAction == false) {
            document.getElementById('nopermission').click();
          }
      });
  }

  ngOnInit(): void {
    
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];

    });

    if (typeof this.id != 'undefined' || typeof this.id != null) {
      this.mode = "view";

      this.donotcallService.view(this.id).subscribe((result) => {
        
        if (this.currentObj.isApproved == true) {
          this.isread = true;
        }
        else {
          this.isread = false;
        }
        this.currentObj = result;
        this.datestring = this.datePipe.transform(result.currentDate, 'yyyy-MM-dd');
        this.readonly = true;
      });
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  list() {
    this.router.navigate(['dncrlist']);
  }
  

  dash() {
    this.router.navigate(['dashboard']);
  }

  activate() {
      this.currentObj.isApproved = true;
    this.donotcallService.update(this.currentObj).subscribe((result) => {
        this.router.navigate(['dncrlist']);
    });
  }
}
