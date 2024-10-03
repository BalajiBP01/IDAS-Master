import { Component, OnInit, Renderer } from '@angular/core';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import {AsideNavService} from '../../aside-nav/aside-nav.service';
import 'datatables.net';
import { ProdservService, DataTableRequest, UserService, UserPermission } from '../../services/services';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-productservicelist',
  templateUrl: './productservicelist.component.html'
})

export class ProductServiceListComponent {
  dtOptions: DataTables.Settings = {};
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  mode: string = "View";
  userid: any;
  userper: UserPermission = new UserPermission();
  
  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

      this.asideNavService.toggle(true);
      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Product Services").subscribe(result => {
          this.userper = result;
          if (this.userper == null || this.userper.viewAction == false ) {
            document.getElementById('nopermission').click();
          }
      });

  }
  ngOnInit(): void {

     

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/Prodserv/ServiceDataList",
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

        { data: 'name', title: 'Service Name', name: 'name' },
        { data: 'code', title: 'Service Code', name: 'code' },
        { data: 'isActive', title: 'Is Active', name: 'isActive' },
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
      columnDefs: [{ orderable: false, targets: [2] }]
    };
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['productservicelist/productservicedetail', event.target.getAttribute("view-id")]);
      }
    });
  }
  servicedetailform() {
    this.router.navigate(['productservicelist/productservicedetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
