import { Component, OnInit, Renderer, ViewChild, ElementRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import { DataTableDirective } from 'angular-datatables';
import { DatePipe } from '@angular/common';
import * as $ from 'jquery';
import 'datatables.net';
import * as _moment from 'moment';
import { ContactusService, ContactUsSearchRequest, UserService, UserPermission } from '../services/services';
import { Subject } from 'rxjs';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../popup/popup.component';
import { PopupService } from '../popup/popupService';



@Component({
  selector: 'app-contactus',
  templateUrl: './contactus.component.html',

})

export class ContactusComponent implements OnInit {
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  @ViewChild('tableid') table: ElementRef;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();
  fromDatestring: string;
  toDatestring: string;
  userid: any;
  userper: UserPermission = new UserPermission();
  errormsg: any;

  _DataTableRequest: ContactUsSearchRequest = new ContactUsSearchRequest();

  mode: string = "View";

  constructor(public router: Router, private renderer: Renderer, private datePipe: DatePipe, public asideNavService: AsideNavService, public userService: UserService, private modalService: NgbModal, public popupservice: PopupService) {

    this.asideNavService.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Contact Us").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    this.change();
    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
  }
  dash() {
    this.router.navigate(['dashboard']);
  }


  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-id")) {
        this.router.navigate(['contactus/contactusdetail', event.target.getAttribute("view-id")]);
      }
    });
    this.dtTrigger.next();
  }
  change() {

    this._DataTableRequest = new ContactUsSearchRequest();
    

    var req = JSON.stringify(this._DataTableRequest);

    this.dtOptions = {
      ajax: {
        url: "/api/Contactus/ContactusList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },

        data: function (data) {
          var req1 = JSON.parse(req);
          req1.fromdate = $('#fromdate').val();
          req1.todate = $('#todate').val();
          req1.isRead = $('#read').val();
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },

      columns: [
        {
          data: 'contactName', title: 'Name', name: 'contactName',
          "render": function (data: any, type: any, full: any) {
            if (full.isRead) {
              return data;
            } else {
              return '<p style="color:red"> ' + data + '</p>'
            }

          }
        },
        {
          data: 'email', title: 'Email', name: 'email',
          "render": function (data: any, type: any, full: any) {
            if (full.isRead) {
              return data;
            } else
              return '<p style="color:red"> ' + data + '</p>'
          }
        },
        {
          data: 'date', title: 'Date', name: 'date',
          "render": function (data: any, type: any, full: any) {
            var datecal = _moment(new Date(data).toString()).format('YYYY-MM-DD');
            if (full.isRead) {
              return datecal;
            } else
              return '<p style="color:red">' + datecal + '</p>'
          }
        },
        {
          data: 'isRead', title: 'Is Read', name: 'isRead',
          "render": function (data: any, type: any, full: any) {
            if (full.isRead) {
              return "True";
            } else
              return '<p style="color:red;text-transform: capitalize;">' + data + '</p>'
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
  ngOnDestroy(): void {
    // Do not forget to unsubscribe the event
    this.dtTrigger.unsubscribe();
  }

 
  rerender(): void {

    if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
      this.errormsg = "To date should be greater than from date.";

      document.getElementById('errormsg').click();
      return;
    }
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload();
    });
  }
  refresh() {

  }
}
