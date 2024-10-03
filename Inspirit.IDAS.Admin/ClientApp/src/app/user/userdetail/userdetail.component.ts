import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';
import { User, UserService, UserCrudResponse, UserPermission, SecurityService, Menu } from '../../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { isNullOrUndefined } from "util";
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-userdetail',
  templateUrl: './userdetail.component.html'
})

export class UserdetailComponent {
  emailPattern: RegExp;

  currentObj: User = new User();
  id: any;
  private sub: any;
  mode: string = "view";
  reponse: UserCrudResponse;
  readonly: boolean = false;
  usrper: UserPermission;
  addNew: number = 0;
  formnames: Menu[];
  usrpermissionlst: UserPermission[] = [];
  errorMessage: any;
  errorMessageValue: any;
  isvalid: boolean = false;
  userid: any;
  userper: UserPermission = new UserPermission();
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, private usrService: UserService, public securityServices: SecurityService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.usrService.getPermission(this.userid, "Manage Users").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
       
        document.getElementById('nopermission').click();
      }
    });
  }

  ngOnInit(): void {

    this.usrService.getUserMenu().subscribe((result) => {
      this.formnames = result;
      console.log(this.formnames[0].caption);
    });
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.currentObj = new User();
      this.currentObj.userPermissionslist = this.usrpermissionlst;
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.usrService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        console.log(this.currentObj.userPermissionslist);
        this.readonly = true;

      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  Addpermission() {
    this.usrper = new UserPermission();
    this.usrper.addAction = false;
    this.usrper.editAction = false;
    this.usrper.id = "00000000-0000-0000-0000-00000000000" + this.addNew++;
    this.usrper.privileged = false;
    this.usrper.viewAction = true;
    this.currentObj.userPermissionslist.push(this.usrper);
  }
  Removeproduct(id) {
    if (window.confirm("Are you sure want to delete ? \n Note: This will Impact permanently.")) {
      this.usrService.removeUserperission(id).subscribe((result) => {
        if (result.length == 0) {
          var product = this.currentObj.userPermissionslist.find(_id => _id.id == id);
          var index = this.currentObj.userPermissionslist.indexOf(product);
          if (index > -1) {
            this.currentObj.userPermissionslist.splice(index, 1);
          }
        }
        else {
          this.currentObj.userPermissionslist = result;
        }
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
  onKeyloginname(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;
    var keynum;
    var keychar;
    var charcheck = /^[A-Za-z0-9]+$/;
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
  save() {
    this.loading = true;
    this.errorMessageValue = "";
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/;
    if ((isNullOrUndefined(this.currentObj.emailid) || this.currentObj.emailid == "")) {
      this.errorMessageValue = "Enter Email Id";
    }
    else if (this.currentObj.emailid != undefined) {
      if (!this.currentObj.emailid.match(this.emailPattern)) {
        this.errorMessageValue = "Enter valid Email Id";
      }
    }
    if ((isNullOrUndefined(this.currentObj.password) || this.currentObj.password == "")) {
      this.errorMessageValue = "Password is required";
    }
    if ((isNullOrUndefined(this.currentObj.loginName) || this.currentObj.loginName == "")) {
      this.errorMessageValue = "Login Name is required";
    }
    if ((isNullOrUndefined(this.currentObj.lastName) || this.currentObj.lastName == "")) {
      this.errorMessageValue = "Last Name is required";
    }
    if ((isNullOrUndefined(this.currentObj.firstName) || this.currentObj.firstName == "")) {
      this.errorMessageValue = "First Name is required";
    }
    if (this.errorMessageValue != "") {
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    if (this.currentObj.userPermissionslist.length == 0) {
      this.errorMessageValue = "Atleast one form acess should be given.";
      this.loading = false;
      document.getElementById('errormsg').click();
      return;
    }
    else {
      this.currentObj.userPermissionslist.forEach(obj => {
        let formname = obj.formName;
        if (isNullOrUndefined(formname)) {
          this.errorMessageValue = "Form Name cannot be Empty";
          this.loading = false;
          document.getElementById('errormsg').click();
          this.isvalid = false;
          return;
        }
        else {
          this.isvalid = true;
        }
      });
    }
    if (this.isvalid) {
      if (this.mode == "add") {
        this.usrService.insert(this.currentObj).subscribe((result) => {
          this.reponse = result;
          this.loading = false;
          if (this.reponse.isSuccess)
            this.router.navigate(['userlist']);
          else {
            if (this.reponse.message != null && this.reponse.message != "undefined") {
              this.errorMessageValue = this.reponse.message;
              this.loading = false;
              document.getElementById('errormsg').click();
            }
          }
            
        });
      }
      else if (this.mode == "edit") {
        this.usrService.update(this.currentObj).subscribe((result) => {
          this.reponse = result;
          this.loading = false;
          if (this.reponse.isSuccess)
            this.router.navigate(['userlist']);
          else {
            if (this.reponse.message != null && this.reponse.message != "undefined") {
              this.errorMessageValue = this.reponse.message;
              this.loading = false;
              document.getElementById('errormsg').click();
            }
          }
        });
      }

    }
  }
  dash() {
    this.router.navigate(['dashboard']);
  }

  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.loading = true;
    this.usrService.view(this.id).subscribe((result) => {
      this.loading = false;
      this.currentObj = result;
    });
  }

  delete() {
    this.usrService.delete(this.id).subscribe((result) => {
      this.router.navigate(['userlist']);
    });
  }

  list() {
    this.router.navigate(['/userlist']);
  }
}
