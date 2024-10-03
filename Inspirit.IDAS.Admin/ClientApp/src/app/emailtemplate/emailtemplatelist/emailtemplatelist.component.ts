import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { EmailTemplateService, DataTableRequest, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-emailtemplatelist',
  templateUrl: './emailtemplatelist.component.html',

})

export class EmailTemplateListComponent implements OnInit {
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: DataTableRequest = new DataTableRequest();

  mode: string = "View";
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


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

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/EmailTemplate/EmailTemplateList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },

        data: function (data) {
          var req1 = JSON.parse(req);
          req1 = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },

      columns: [

        { data: 'subject', title: 'Subject', name: 'subject' },
        { data: 'type', title: 'Type', name: 'type' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px"  emailview-id=' + data + '> View </button>'
          }
        },
      ],

      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      scrollX: true,
      pageLength: 10,
    };
  }


  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
        if (event.target.hasAttribute("emailview-id")) {
            this.router.navigate(['emailtemplatelist/emailtemplatedetail', event.target.getAttribute("emailview-id")]);
      }
    });
  }
  emailtemplatedetailform() {
      this.router.navigate(['emailtemplatelist/emailtemplatedetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }

}
