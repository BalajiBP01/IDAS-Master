import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
//import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
//import 'datatables.net';
//import * as XLSX from 'xlsx';
import { DataTableRequest } from '../../services/services';
import { Customer, CustomerService, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';

@Component({
  selector: 'app-customerlist',
  templateUrl: './customerlist.component.html'
})


export class customerlistComponent implements OnInit {

  dtOptions: any = {};
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  userid: any;
  userper: UserPermission = new UserPermission();
  mode: string = "View";
  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Customers").subscribe(result => {
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
        url: "/api/Customer/CustomerDetailList",
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
        { data: 'tradingName', title: 'Trading Name', name: 'tradingName' },
        { data: 'registrationName', title: 'Registration Name', name: 'registrationName' },
        { data: 'registrationNumber', title: 'Registration Number', name: 'registrationNumber' },
        { data: 'code', title: 'Code', name: 'code' },
        { data: 'status', title: 'Status', name: 'status' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px"  view-id=' + data + '> View </button>'
          }
        },
        {
          data: 'id', title: 'Users', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" user-id=' + data + '> Users </button>'
          }
        },
        {
          data: 'id', title: 'Products', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" product-id=' + data + '> Products </button>'
          }
        },

        {
          data: 'id', title: 'Tabs', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" tabs-id=' + data + '> Tabs </button>'
          }
        }
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
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['customerlist/customerdetails', event.target.getAttribute("view-id")]);
      }
      else if (event.target.hasAttribute("user-id")) {
        this.router.navigate(['customerlist/customeruserlist', event.target.getAttribute("user-id")]);
      }
      else if (event.target.hasAttribute("product-id")) {
        this.router.navigate(['customerlist/customerproduct', event.target.getAttribute("product-id")]);
      }
      else if (event.target.hasAttribute("tabs-id")) {
        this.router.navigate(['customerlist/customertabs', event.target.getAttribute("tabs-id")]);
      }
    });

  }
  customerdetailform() {
    this.router.navigate(['customerlist/customerdetails']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
