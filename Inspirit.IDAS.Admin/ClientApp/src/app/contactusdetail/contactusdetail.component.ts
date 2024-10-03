
import { Component, OnInit, OnDestroy, NgModule } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { ContactusService,ContactUs,ContactusResponse, UserPermission, UserService } from '../services/services';
import { createElementCssSelector } from '@angular/compiler';
import { NgxEditorModule } from 'ngx-editor';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions} from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../popup/popup.component';
import { PopupService } from '../popup/popupService';



@Component({
  selector: 'app-contactusdetail',
  templateUrl: './contactusdetail.component.html',
})

export class ContactusDetailComponent implements OnInit, OnDestroy {
  currentObj: ContactUs = new ContactUs();
  id: any;
  private sub: any;
  mode: string = "view";
  readonly: boolean = false;
  datestring: string;
  isread: boolean;
  response: ContactusResponse = new ContactusResponse();
  userid: any;
  userper: UserPermission = new UserPermission();
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, private contactusService: ContactusService, private datePipe: DatePipe, public asideNavService: AsideNavService, public userService: UserService, private modalService: NgbModal, public popupservice: PopupService) {

      this.asideNavService.toggle(true);
      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Contact Us").subscribe(result => {
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
      this.loading = true;
      this.contactusService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        this.currentObj.isRead = true;
        this.contactusService.updateReadStatus(this.currentObj).subscribe((result) => {
            this.response = result; 
        });

        this.datestring = this.datePipe.transform(result.date, 'yyyy-MM-dd');
        this.readonly = true;
      });
    }
  }
  read()
  {
     
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  list() {
    this.router.navigate(['contactus']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }

}





