import { Component, OnInit, OnDestroy, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { EmailTemplateList, EmailTemplateService, CrudResponseemail, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { createElementCssSelector } from '@angular/compiler';
import { EventEmitter } from 'events';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-emailtemplatedetail',
  templateUrl: './emailtemplatedetail.component.html'
})

export class EmailTemplateDetailComponent implements OnInit, OnDestroy {

  currentObj: EmailTemplateList = new EmailTemplateList();
  id: any;
  private sub: any;
  mode: string = "view";
  message: any;
  reponse: CrudResponseemail;
  readonly: boolean = false;
  loading: boolean = false;
  public typearray = ["Email Template Type One",
    "Email Template Type Two",
    "Email Template Type Three"];
  public editor;
  public editorOptions = {
    placeholder: 'Compose an epic...',
    modules: {
      toolbar: [[{ 'font': [] }, 'bold', 'italic', 'underline', 'strike',
        'blockquote', 'code-block', { 'header': 1 }, { 'header': 2 },
      { 'list': 'ordered' }, { 'list': 'bullet' },
      { 'script': 'sub' }, { 'script': 'super' },
      { 'indent': '-1' }, { 'indent': '+1' },
      { 'direction': 'rtl' }, { 'size': ['small', false, 'large', 'huge'] },
      { 'header': [1, 2, 3, 4, 5, 6, false] }, { 'color': [] }, { 'background': [] },
      { 'align': [] }, 'clean', 'link', 'image']]
    }
  };
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private route: ActivatedRoute,
      private emiailtemplateService: EmailTemplateService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Email Templates").subscribe(result => {
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
      this.currentObj = new EmailTemplateList();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.emiailtemplateService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        this.readonly = true;
      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  save() {
    
    this.loading = true;
    if ((this.currentObj.subject == null || this.currentObj.subject == "undefined") || this.currentObj.subject == "") {
      this.message = "Subject is required";
      this.loading = false;
      document.getElementById('error').click();
      return;
    }
    if ((this.currentObj.mailContent == null || this.currentObj.mailContent == "undefined") || this.currentObj.mailContent == "") {
      this.message = "Mail Content is required";
      this.loading = false;
      document.getElementById('error').click();
      return;
    }
    if (this.mode == "add") {
      this.emiailtemplateService.insert(this.currentObj).subscribe((result) => {
        this.reponse = result;
        this.loading = false;
        this.router.navigate(['emailtemplatelist']);
      });
    }
    else if (this.mode == "edit") {
      this.emiailtemplateService.update(this.currentObj).subscribe((result) => {
        this.reponse = result;
        this.loading = false;
        this.router.navigate(['emailtemplatelist']);
      });
    }
  }
  edit() {
    this.mode = "edit";
    this.loading = true;
    this.readonly = false;
    this.emiailtemplateService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.loading = false;
    });
  }
  delete() {
    this.emiailtemplateService.delete(this.id).subscribe((result) => {
      this.router.navigate(['emailtemplatelist']);
    });
  }
  list() {
    this.router.navigate(['emailtemplatelist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}





