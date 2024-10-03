import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ApplicationMessage, ApplicationMessageService, CrudResponseMessage  ,UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { isNullOrUndefined } from "util";
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';

@Component({
    selector: 'app-applicationmessagedetail',
    templateUrl: './applicationmessagedetail.component.html',

})

export class ApplicationMessageDetailComponent implements OnInit, OnDestroy {

    currentObj: ApplicationMessage = new ApplicationMessage();
    id: any;
    private sub: any;
    mode: string = "view";
    reponse: CrudResponseMessage;
    readonly: boolean = false;
    errorMessageValue: string;
    userid: any;
  userper: UserPermission = new UserPermission();
  loading: boolean = false;


    constructor(public router: Router, private route: ActivatedRoute, private appmsgService: ApplicationMessageService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService ) {

        this.asideNavService.toggle(true);

        this.userid = localStorage.getItem('userid');
        this.userService.getPermission(this.userid, "Application Message").subscribe(result => {
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

        if (typeof this.id == 'undefined' || typeof this.id == null) {

            this.mode = "add";
            this.currentObj = new ApplicationMessage();
            this.readonly = false;
        }
        else {
          this.mode = "view";
          this.loading = true;
            this.appmsgService.view(this.id).subscribe((result) => {
              this.currentObj = result;
              this.loading = false;
                this.readonly = true;
            });


        }
    }
    onKey(e: any) {
        var str = e.target.value;
        var charCode = (e.which) ? e.which : e.keyCode;
        if (charCode == 8) return true;
        var keynum;
        var keychar;
        var charcheck = /^[A-Za-z0-9- ]+$/;
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

    ngOnDestroy() {
        this.sub.unsubscribe();
    }
 

  dash() {
    this.router.navigate(['dashboard']);
  }

  save() {
    this.loading = true;
        this.errorMessageValue = "";
        if ((isNullOrUndefined(this.currentObj.message) || this.currentObj.message == "")) {
            this.errorMessageValue =  "Message is required";
        }
        if ((isNullOrUndefined(this.currentObj.name) || this.currentObj.name == "")) {
            this.errorMessageValue = "Name is required";
        }
        if (this.errorMessageValue != "") {
          if (this.errorMessageValue != null && this.errorMessageValue != "undefined") {
              this.errorMessageValue = this.errorMessageValue;
              this.loading = false;
              document.getElementById('error').click();
              return;
            
          }

        }
     
            if (this.mode == "add") {
                this.appmsgService.insert(this.currentObj).subscribe((result) => {
                  this.reponse = result;
                  this.loading = false;
                    if (this.reponse.isSuccess) {
                        this.router.navigate(['applicationmessagelist']);
                    }
                    else {
                      if (this.reponse.message != null && this.reponse.message != "undefined") {
                        this.errorMessageValue = this.reponse.message;
                        this.loading = false;
                        document.getElementById('error').click();
                        return;
                      }
                    }
                });
            }
            else if (this.mode == "edit") {
                {
                    this.appmsgService.update(this.currentObj).subscribe((result) => {
                      this.reponse = result;
                      this.loading = false;
                        if (this.reponse.isSuccess) {
                            this.router.navigate(['applicationmessagelist']);
                        }
                        else {
                          if (this.reponse.message != null && this.reponse.message != "undefined") {
                            this.errorMessageValue = this.reponse.message;
                            this.loading = false;
                            document.getElementById('error').click();
                            return;
                          }
                        }

                    });
                }

            }

        
    }
    edit() {
        this.mode = "edit";
      this.readonly = false;
      this.loading = true;
      this.appmsgService.view(this.id).subscribe((result) => {
        this.loading = false;
            this.currentObj = result;
        });
    }
    delete() {
        this.appmsgService.delete(this.id).subscribe((result) => {
            this.router.navigate(['applicationmessagelist']);
        });
    }

    list() {

        this.router.navigate(['applicationmessagelist']);
    }

}





