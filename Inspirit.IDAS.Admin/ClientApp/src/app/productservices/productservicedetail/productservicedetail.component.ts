import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { Service, ProdservService, ServiceCrudResponse, UserService, UserPermission } from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { isNullOrUndefined } from "util";
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-productservicedetail',
  templateUrl: './productservicedetail.component.html'
})

export class ProductServiceDetailComponent {
  currentObj: Service = new Service();
  id: any;
  private sub: any;
  mode: string = "view";
  reponse: ServiceCrudResponse;
  readonly: boolean = false;
  errorMessageValue: any = "";
  userid: any;
  userper: UserPermission = new UserPermission();
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, private prodservService: ProdservService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


  }
  ngOnInit(): void {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Product Services").subscribe(result => {
      this.userper = result;

      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }

    });



    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });
    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.currentObj = new Service();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.prodservService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        this.readonly = true;
      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  onKey(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;
    var keynum;
    var keychar;
    var charcheck = /^[A-Za-z ]+$/;
    if (window.event) {
      keynum = e.keyCode;
    }
    else {
      if (e.which) {
        keynum = e.which;
      }
      else return true;
    }
    keychar = String.fromCharCode(keynum);
    return charcheck.test(keychar);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  save() {
    this.loading = true;
    this.errorMessageValue = "";
    if ((isNullOrUndefined(this.currentObj.code) || this.currentObj.code == "")) {
      this.errorMessageValue = "Code is required";
    }
    if ((isNullOrUndefined(this.currentObj.name) || this.currentObj.name == "")) {
      this.errorMessageValue = "Name is required";
    }

    if (this.errorMessageValue != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;

    }


    if (this.mode == "add") {
      this.prodservService.insert(this.currentObj).subscribe((result) => {
        this.loading = false;
        if (!result.isSuccess) {
          if (result.message != null && result.message != "undefined") {
            this.errorMessageValue = result.message;
            this.loading = false;
            document.getElementById('errormsg').click();
            return;
          }
          return;

        }
        this.router.navigate(['productservicelist']);
      });
    }
    else if (this.mode == "edit") {
      {
        this.prodservService.update(this.currentObj).subscribe((result) => {
          this.loading = false;
          this.reponse = result;
          if (this.reponse.isSuccess)
            this.router.navigate(['productservicelist']);
          else {
            if (result.message != null && result.message != "undefined") {
              this.errorMessageValue = result.message;
              this.loading = false;
              document.getElementById('errormsg').click();
              return;
            }
            return;
          }
        });
      }

    }
  }
  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.loading = true;
    this.prodservService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.loading = false;
    });
  }
  delete() {
    this.prodservService.delete(this.id).subscribe((result) => {
      this.router.navigate(['productservicelist']);
    });
  }
  list() {
    this.router.navigate(['/productservicelist']);
  }
}
