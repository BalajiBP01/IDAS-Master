import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { LookupData, LookupDataService, CrudResponse, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { isNullOrUndefined } from 'util';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-lookupdetail',
  templateUrl: './lookupdetail.component.html'
})

export class lookupdetailComponent implements OnInit, OnDestroy {

  currentObj: LookupData = new LookupData();
  id: any;
  private sub: any;
  mode: string = "view";
  reponse: CrudResponse;
  readonly: boolean = false;
  userid: any;
  userper: UserPermission = new UserPermission();
  errormsg: any;
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, private lookupService: LookupDataService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    if (!isNullOrUndefined(this.userid)) {
      this.userService.getPermission(this.userid, "Lookup Data").subscribe(result => {
        this.userper = result;
        if (this.userper == null || this.userper.viewAction == false) {
          document.getElementById('nopermission').click();
        }
      });
    }
   
  }
  ngOnInit(): void {

    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    if (typeof this.id == 'undefined' || typeof this.id == null) {

      this.mode = "add";
      this.currentObj = new LookupData();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.lookupService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        this.readonly = true;

      });


    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  save() {
    this.loading = true;
    if (this.mode == "add") {
      if ((isNullOrUndefined(this.currentObj.type) || this.currentObj.type == "") || (isNullOrUndefined(this.currentObj.value) || this.currentObj.value == "")
        || (isNullOrUndefined(this.currentObj.text) || this.currentObj.text == "")) {
        this.errormsg = "All field's are mantatory";
        this.loading = false;
        document.getElementById('errormsg').click();
          return;
      }
      this.lookupService.insert(this.currentObj).subscribe((result) => {
        this.reponse = result;
        this.loading = false;
        if (!result.isSuccess) {
          if (result.message != null && result.message != "undefined") {
            this.errormsg = "All field's are mantatory";
            this.loading = false;
            document.getElementById('errormsg').click();
            return;
          }
        }else
        this.router.navigate(['lookuplist']);
      });

    }
    else if (this.mode == "edit") {
      {
       
        if ((isNullOrUndefined(this.currentObj.type) || this.currentObj.type == "") || (isNullOrUndefined(this.currentObj.value) || this.currentObj.value == "")
          || (isNullOrUndefined(this.currentObj.text) || this.currentObj.text == "") ) {
          this.errormsg = "All field's are mantatory";
          this.loading = false;
          document.getElementById('errormsg').click();
          return;
        }
        this.lookupService.update(this.currentObj).subscribe((result) => {
          this.reponse = result;
          this.loading = false;
          this.router.navigate(['lookuplist']);
        });
      }

    }
  }
  edit() {

    this.mode = "edit";
    this.loading = false;
    this.readonly = false;
    this.lookupService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.loading = false;
    });
  }
  delete() {
    this.lookupService.delete(this.id).subscribe((result) => {
      this.router.navigate(['lookuplist']);
    });
  }
  list() {
    this.router.navigate(['lookuplist']);
  }
}





