import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { DataTableRequest } from '../../services/services';
import { UserService, CustomerUser, CrudUserResponse, UserDataTableRequest } from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import { locale } from 'moment';


@Component({
  selector: 'app-userlist',
  templateUrl: './userlist.component.html',

})

export class userlistComponent implements OnInit {
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: UserDataTableRequest = new UserDataTableRequest();

  mode: string = "View";
  custmerId: any;
  customerUserId: any;
  name: any;
  isuserExists: any;
  loading: boolean = false;

  constructor(public router: Router, private renderer: Renderer, public headernavService: headernavService) {

  }
  ngOnInit(): void {
    this.loading = true;
    this.isuserExists = localStorage.getItem('userid');
    if (this.isuserExists != null && this.isuserExists != "undefined") {

      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

      this.custmerId = localStorage.getItem('customerid');
      this.customerUserId = localStorage.getItem('userid');


      this._DataTableRequest.customerId = this.custmerId;
      this._DataTableRequest.customerUserId = this.customerUserId;

      var req = JSON.stringify(this._DataTableRequest);
      this.dtOptions = {
        ajax: {
          url: "/api/User/UserList",
          type: 'POST',
          contentType: 'application/json; charset=UTF-8',
          error: function (xhr, error, code) { console.log(error); },

          data: function (data) {
            var req1 = JSON.parse(req);
            req1.dtRequest = data;
            var req2 = JSON.stringify(req1);
            return req2;
          }
        },

        columns: [

          { data: 'firstName', title: 'First Name', name: 'firstName' },
          { data: 'lastName', title: 'Last Name', name: 'lastName' },
          { data: 'title', title: 'Title', name: 'title' },
          { data: 'idNumber', title: 'ID Number', name: 'idNumber' },
          { data: 'status', title: 'Status', name: 'status' },
          {
            data: 'id', title: 'Action', name: 'id',
            "render": function (data: any, type: any, full: any) {
              return '<button class="btn btn-link" style="padding:0px" user-view-id=' + data + '> View </button>'
            }
          },
        ],
        initComplete: function () {
          document.getElementById('load').click();
        },
        language: {
          search: "Filter:"
        },
        processing: true,
        serverSide: false,
        pagingType: 'full_numbers',
        pageLength: 10,
        scrollX: true,
      };
    } else {
      this.router.navigate(['/login']);
    }
  }


  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("user-view-id")) {
        this.router.navigate(['userlist/userdetails'], { queryParams: { id: event.target.getAttribute("user-view-id") }, skipLocationChange: true });
      }
    });
  }
  userdetailform() {
    this.router.navigate(['userlist/userdetails']);
  }
  list() {

    this.router.navigate(['userdetails']);
  }
load(){
this.loading = false;
}
}
