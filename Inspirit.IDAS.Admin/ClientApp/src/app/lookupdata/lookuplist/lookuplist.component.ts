import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { LookupDataService, DataTableRequest, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
//import * as XLSX from 'xlsx';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { retry } from 'rxjs/operator/retry';


@Component({
  selector: 'app-lookuplist',
  templateUrl: './lookuplist.component.html',

})

export class lookuplistComponent implements OnInit {
  dtOptions: any = {};
  @ViewChild('tableid') table: ElementRef;

  _DataTableRequest: DataTableRequest = new DataTableRequest();

  mode: string = "View";
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Lookup Data").subscribe(result => {
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
        url: "/api/LookupData/LookupDataList",
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

        { data: 'type', title: 'Type', name: 'type' },
        { data: 'value', title: 'Value', name: 'value' },
        { data: 'text', title: 'Text', name: 'text' },
        { data: 'isActive', title: 'Active', name: 'isActive',
          "render": function (data: any, type: any, full: any) {
            if (data == true) { return "True"}
            else {
              return "False"
            }
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
      pageLength: 10,
      scrollX: true,
      dom: 'lBfrtip',
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
        this.router.navigate(['lookuplist/lookupdetail', event.target.getAttribute("view-id")]);
      }
    });
  }
  lookupdetailform() {
    this.router.navigate(['lookuplist/lookupdetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
