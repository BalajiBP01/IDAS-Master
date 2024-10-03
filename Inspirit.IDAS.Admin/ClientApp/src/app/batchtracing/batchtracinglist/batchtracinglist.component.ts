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
  DataTableRequest, SubscriptionResponse, UserService, UserPermission, BatchDataTableRequest, BatchTracingService, BatchtraceResponse
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
  selector: 'batchtrace',
  templateUrl: './batchtracinglist.component.html'
})
export class BatchtracinglistComponent implements OnInit {
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
  response: BatchtraceResponse = new BatchtraceResponse();
  errormsg: any;

  dataTablereq: BatchDataTableRequest = new BatchDataTableRequest();
  warningMesage: string;
  batchID: any;

  constructor(public router: Router, public route: ActivatedRoute,
    public renderer: Renderer, public datePipe: DatePipe,
    public subscriptionservice: SubscriptionService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService, public _batchTracingService: BatchTracingService) {

  
    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "BatchTracing").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    let origin = window.location.href;
    this.customer.id = "00000000-0000-0000-0000-000000000000";
    if (origin.indexOf('batchtracelistcustomer') >= 0) {
      var customerid = localStorage.getItem('customerforbatch');
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
        url: '/api/BatchTracing/GetBatchTrace',
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
        { data: 'batchNumber', title: 'Batch Number', name: 'batchNumber' },
        {
          data: 'id', title: 'File Name', name: 'id',
          "render": function (data: any, type: any, row: any) {
            if (row.isFileExists == true) {
              return '<button class="btn btn-link" style="padding:0px"  batch-id=' + data + '> ' + row.fileName + ' </button>'
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
              return '<button class="btn btn-link" style="padding:0px"  batch-approve=' + row.id + '>Approve</button>'
            } else
              return "Appoved";
          }
        },
        {
          data: 'id', title: 'Delete Batch', name: 'ID',
          "render": function (data, type, row) {
            if (row.adminCertified == true) {
              return "Not Allowed";
            } else {
              return '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" batchtracing-remove=' + data + '>' + 'Delete' + ' </button>';
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
    modalRef.componentInstance.componentName = "BatchtracinglistComponent";
    this.router.navigate(['batchtrace']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("batch-id")) {
        this._batchTracingService.downloadExcel(event.target.getAttribute("batch-id")).subscribe((response) => {
          this.downloadFile(response.data, response.fileName);
        });
      }
      else if (event.target.hasAttribute("batch-approve")) {
       
        this._batchTracingService.updateBatchTrace(event.target.getAttribute("batch-approve"), this.userid).subscribe((response) => {
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
      else if (event.target.hasAttribute("batchtracing-remove")) {
        this.warningMesage = "Do you want to cancel the Batch?";
        this.batchID = event.target.getAttribute("batchtracing-remove");
      }
    });
    this.dtTrigger.next();
  }
  Submit(batchID) {
    if (this.warningMesage == "Do you want to cancel the Batch?") {
      this._batchTracingService.removeBatchTrace(batchID).subscribe((response) => {
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
