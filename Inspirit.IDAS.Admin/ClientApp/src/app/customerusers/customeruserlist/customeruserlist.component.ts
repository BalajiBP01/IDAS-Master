import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import {
  DataTableRequest, CustomerUserRequest, CustomerService, UserService, UserPermission
} from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
  selector: 'app-customeruserlist',
  templateUrl: './customeruserlist.component.html'
})

export class customeruserlistComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  dtOptions: any = {};

  _DataTableRequest: CustomerUserRequest = new CustomerUserRequest();
  sub: any;
  id: any;
  mode: string = "View";
  companyname: string;
  userid: any;
  userper: UserPermission = new UserPermission();


  constructor(public router: Router, private renderer: Renderer, public route: ActivatedRoute, public customerService: CustomerService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Customers").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        //alert("You don't have permission to access.");
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
      this.customerService.getName(this.id).subscribe((result) => {
        this.companyname = result;
      });
      localStorage.setItem('customerid', this.id);
    });
    this._DataTableRequest.id = this.id;
    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/CustomerUser/CustomerUserList",
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
        { data: 'firstName', title: 'FirstName', name: 'firstName' },
        { data: 'lastName', title: 'LastName', name: 'lastName' },
        { data: 'title', title: 'Title', name: 'title' },
        { data: 'idNumber', title: 'ID Number', name: 'idNumber' },
        { data: 'status', title: 'Status', name: 'status' },
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
      pageLength: 10,
      dom: 'lBfrtip',
      scrollX: true,
      buttons: [
        {
          extend: 'collection',
          text: 'Export',
          buttons: [
            'copy',
            'excel',
            'csv',
            'pdf',
            'print'
          ]
        }
      ]
    };
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['customerlist/customeruserdetails', event.target.getAttribute("view-id")]);
      }
    });
  }
  customeruserdetailform() {
    this.router.navigate(['customerlist/customeruserdetails']);
  }
  list() {
    this.router.navigate(['customerlist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
