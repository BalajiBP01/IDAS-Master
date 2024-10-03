import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import {
    DataTableRequest , UserService, UserPermission
} from '../../services/services';
import * as _moment from 'moment';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions} from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';




@Component({
  selector: 'app-creditnotelist',
  templateUrl: './creditnotelist.component.html',

})

export class CreditnotelistComponent implements OnInit {
  dtOptions: DataTables.Settings = {};

  _DataTableRequest: DataTableRequest = new DataTableRequest();
  userid: any;
  userper: UserPermission = new UserPermission();
  mode: string = "View";

  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService ) {
      this.asideNavService.toggle(true);

      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Invoice").subscribe(result => {
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
        url: "/api/CreditnoteList/CreditnoteList",
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

        { data: 'creditNoteNumber', title: 'Number', name: 'creditNoteNumber' },
        {
          data: 'CreditNoteDate', title: 'Date', name: 'CreditNoteDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },
        { data: 'CreditNoteValue', title: 'Value', name: 'CreditNoteValue' },
        
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
    };
  }


  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['creditnotelist/creditnotedetail', event.target.getAttribute("view-id")]);
      }
    });
  }
 creditnotedetailform() {
    this.router.navigate(['creditnotelist/creditnotedetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
