import { Component, OnInit, Renderer } from '@angular/core';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { UserService, DataTableRequest, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { debounce } from 'rxjs/operator/debounce';

@Component({
  selector: 'userlist',
  templateUrl: './userlist.component.html'
})

export class UserlistComponent implements OnInit {
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: DataTableRequest = new DataTableRequest();
  mode: string = "View";
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer, public asideNavService: AsideNavService, public userService: UserService, private modalService: NgbModal, public popupservice: PopupService) {
    this.asideNavService.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Manage Users").subscribe(result => {
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
        url: "/api/User/UserDataList",
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
        { data: 'firstName', title: 'First Name', name: 'firstName' },
        { data: 'lastName', title: 'Last Name', name: 'lastName' },
        { data: 'loginName', title: 'Login Name', name: 'loginName' },
        { data: 'emailid', title: 'Email Id', name: 'emailid' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-id=' + data + '> View </button>'
          }
        },
      ],

      processing: true,
      serverSide: false,
      scrollX: true,
      pagingType: 'full_numbers',
      pageLength: 10,
    };
  }

  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['userlist/userdetail', event.target.getAttribute("view-id")]);
      }
    });
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  userdetailform() {
    this.router.navigate(['userlist/userdetail']);
  }
}
