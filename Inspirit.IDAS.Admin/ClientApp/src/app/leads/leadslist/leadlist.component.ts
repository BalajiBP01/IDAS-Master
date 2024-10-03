import { Component, OnInit, Renderer, ViewChild, ElementRef, transition } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import * as $ from 'jquery';
import { Subject } from 'rxjs';
import { DatePipe } from '@angular/common';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import 'datatables.net';
import {
  Customer, SubscriptionService,
  LeadGenerationService, SubscriptionResponse, UserService, UserPermission, LeadDataTableRequest, LeadDataTableResponse, LeadGenerationVM, LeadResponse
} from '../../services/services';
import { SearchCustomer } from '../../searchcustomer/searchcustomer.component';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as _moment from 'moment';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';
import { retry } from 'rxjs/operators';
@Component({
  selector: 'leadlist',
  templateUrl: './leadlist.component.html'
})
export class LeadlistComponent implements OnInit {
  @ViewChild('tableid') table: ElementRef;
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();

  fromDate: string = Date.toString();
  toDate: string = Date.toString();
  fromDatestring: string;
  toDatestring: string;
  customer: Customer = new Customer();
  userid: any;
  userper: UserPermission = new UserPermission();
  response: LeadResponse = new LeadResponse();
  errormsg: any;

  dataTablereq: LeadDataTableRequest = new LeadDataTableRequest();
  warningMesage: string;
  LeadId: any;

  constructor(public router: Router, public route: ActivatedRoute,
    public renderer: Renderer, public datePipe: DatePipe,
    public subscriptionservice: SubscriptionService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService, public _leadservice: LeadGenerationService) {


    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "LeadGeneration").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('leadlistcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforlead');
      if (!isNullOrUndefined(customerid)) {
        this.subscriptionservice.getCustomerById(customerid).subscribe((result) => {
          this.customer = result;
        });
      }
    }

    this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
    this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');

    var req = JSON.stringify(this.dataTablereq);
    this.dtOptions = {
      ajax: {
        url: '/api/LeadGeneration/GetLeadList',
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },
        data: function (data) {
          var req1 = JSON.parse(req);
          req1.fromdate = $('#fromdate').val();
          req1.todate = $('#todate').val();
          req1.customerId = $('#customer').val();
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },
      columns: [
        { data: 'customerName', title: 'Customer Name', name: 'customerName' },
        { data: 'leadNumber', title: 'Lead Number', name: 'leadNumber' },
        {
          data: 'id', title: 'File Name', name: 'id',
          "render": function (data: any, type: any, row: any) {
            if (row.isFileExists == true) {
              return '<button class="btn btn-link" style="padding:0px"  lead-id=' + data + '> ' + row.fileName + ' </button>'
            }
            else {
              return row.fileName;
            }
          }
        },
        {
          data: 'uploadDate', title: 'Upload Date', name: 'uploadDate',
          "render": function (data, type, row) {
            return data;
          }
        },
        {
          data: 'proFormaInvoiceId', title: 'Invoice Raised', name: 'proFormaInvoiceId',
          "render": function (data: any, type: any, row: any) {
            if (data != "") {
              return "YES";
            } else
              return "NO";
          }
        },
        { data: 'proFormaInvoiceNumber', title: 'Proforma Invoice Number', name: 'proFormaInvoiceNumber' },
        {
          data: 'adminCertified', title: 'admin Certified', name: 'adminCertified',
          "render": function (data: any, type: any, row: any) {
            if (data == false) {
              return '<button class="btn btn-link" style="padding:0px"  lead-approve=' + row.id + '>Approve</button>'
            } else
              return "Appoved";
          }
        },
        {
          data: 'id', title: 'Delete Lead', name: 'ID',
          "render": function (data, type, row) {
            if (row.adminCertified == true) {
              return "Not Allowed";
            } else {
              return '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" lead-remove=' + data + '>' + 'Delete' + ' </button>';
            }
          },
        }
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
  rerender(): void {
    if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
      this.errormsg = 'To date should be greater than from date';
      document.getElementById('errormsg').click();
      return;
    }
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload();
    });
  }
  searchcustomer() {
    const modalRef = this.modalService.open(SearchCustomer, { size: 'lg' });
    modalRef.componentInstance.componentName = "LeadlistComponent";
    this.router.navigate(['leadlist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("lead-id")) {
        this._leadservice.downloadExcel(event.target.getAttribute("lead-id")).subscribe((response) => {
          this.downloadFile(response.data, response.fileName);
        });
      }
      else if (event.target.hasAttribute("lead-approve")) {

        this._leadservice.updateLead(event.target.getAttribute("lead-approve"), this.userid).subscribe((response) => {
          this.response = response;
          this.rerender();
          if (this.response.isSuccess != true) {
            if (this.response.message != null && this.response.message != "undefined") {
              this.errormsg = this.response.message;
              document.getElementById('error').click();
            }
          } else {
            this.rerender();
          }
        });
      }
      else if (event.target.hasAttribute("lead-remove")) {
        this.warningMesage = "Do you want to cancel the Lead?";
        this.LeadId = event.target.getAttribute("lead-remove");
      }
    });
    this.dtTrigger.next();
  }
  Submit(LeadId) {
    if (this.warningMesage == "Do you want to cancel the Lead?") {
      this._leadservice.removeLeads(LeadId).subscribe((response) => {
        this.rerender();
      });
    }
  }
  downloadFile(blob, filename: string) {
    var url = window.URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    link.click();
  }
}
