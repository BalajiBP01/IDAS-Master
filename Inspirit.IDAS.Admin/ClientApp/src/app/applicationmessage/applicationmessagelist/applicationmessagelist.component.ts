import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { ApplicationMessageService, DataTableRequest, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { transition } from '@angular/core/src/animation/dsl';
import { debounce } from 'rxjs/operators';



@Component({
  selector: 'app-applicationmessagelist',
  templateUrl: './applicationmessagelist.component.html',

})

export class ApplicationMessagelistComponent implements OnInit {
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: DataTableRequest = new DataTableRequest();
  userid: any;
  userper: UserPermission = new UserPermission();
  mode: string = "View";

  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {
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

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/ApplicationMessage/ApplicationMessageList",
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

        { data: 'name', title: 'Name', name: 'Name' },
        {
          data: 'showmessage', title: 'Show Message', name: 'showmessage',
          "render": function (data: any, type: any, full: any) {
            if (data == true) { return "True"; }
            else { return "False" };
          }
        },

        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-id=' + data + '> View </button>'
          }
        },
      ],

      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      scrollX: true,
      pageLength: 10,
      order: [1, "desc"]
    };
  }


  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['applicationmessagelist/applicationmessagedetail', event.target.getAttribute("view-id")]);
      }
    });
  }
  applicationmessagedetailform() {
    this.router.navigate(['applicationmessagelist/applicationmessagedetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
