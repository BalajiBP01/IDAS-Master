import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ApplicationSetting, AppSettingService, SettingCrudResponse, UserPermission, UserService } from '../../services/services';
import { AsideNavService} from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { isNullOrUndefined } from 'util';


@Component({
  selector: 'app-appsettingdetail',
  templateUrl: './appsettingdetail.component.html'
})

export class appsettingdetailComponent implements OnInit, OnDestroy {

  currentObj: ApplicationSetting = new ApplicationSetting();
  id: any;
  private sub: any;
  mode: string = "view";
  reponse: SettingCrudResponse;
  readonly: boolean = false;
  userid: any;
  message: any; 
  userper: UserPermission = new UserPermission();
  loading: boolean = false;
  constructor(public router: Router, private route: ActivatedRoute, private appService: AppSettingService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

      this.asideNavService.toggle(true);

      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Application Settings").subscribe(result => {
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
      this.currentObj = new ApplicationSetting();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.appService.view(this.id).subscribe((result) => {
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
      if ((isNullOrUndefined(this.currentObj.settingName) || this.currentObj.settingName == "") || (isNullOrUndefined(this.currentObj.settingValue) || this.currentObj.settingValue == "")) {
        this.message = "All mandatory fields are required";
        this.loading = false;
        document.getElementById('error').click();
        return;
      }
      this.appService.insert(this.currentObj).subscribe((result) => {
        this.reponse = result;
        this.loading = false;
          this.router.navigate(['appsettinglist']);
      });
      
    }
    else if (this.mode == "edit") {
      {
        if ((isNullOrUndefined(this.currentObj.settingName) || this.currentObj.settingName == "") || (isNullOrUndefined(this.currentObj.settingValue) || this.currentObj.settingValue == "")) {
          this.message = "All mandatory fields are required";
          this.loading = false;
          document.getElementById('error').click();
          return;
        }
        this.appService.update(this.currentObj).subscribe((result) => {
          this.reponse = result;
          this.loading = false;
            this.router.navigate(['appsettinglist']);
        });
  
      }

    }
  }

  edit() {

    this.mode = "edit";
    this.readonly = false;
    this.loading = true;
    this.appService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.loading = false;
    });
  }

  list() {
    this.router.navigate(['/appsettinglist']);
  }
}
