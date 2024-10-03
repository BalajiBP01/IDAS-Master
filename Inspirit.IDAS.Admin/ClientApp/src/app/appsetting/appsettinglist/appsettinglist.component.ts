import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { AppSettingService, DataTableRequest, UserPermission, UserService } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-appsettinglist',
  templateUrl: './appsettinglist.component.html'
})

export class appsettinglistComponent implements OnInit
{
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: DataTableRequest = new DataTableRequest();
  mode: string = "View";
  userid: any;
  userper: UserPermission = new UserPermission();



  constructor(public router: Router, private renderer: Renderer, public asideNavService: AsideNavService, public userService: UserService, private modalService: NgbModal, public popupservice: PopupService) {

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

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/AppSetting/SettingDataList",
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

        { data: 'settingName', title: 'Setting Name', name: 'settingName' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" message-id=' + data + '> View </button>'
          }
        },
      ],

      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      pageLength: 10,
      scrollX: true,
    };
  }

  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
        if (event.target.hasAttribute("message-id")) {
            this.router.navigate(['appsettinglist/appsettingdetail', event.target.getAttribute("message-id")]);
      }
    });
  }

  appsettingdetailform() {
    this.router.navigate(['appsettinglist/appsettingdetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
