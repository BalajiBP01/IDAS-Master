import { Component, Inject, OnInit, ChangeDetectorRef, Renderer, transition, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { BatchTracingService, BatchTracingServiceRequest, FullAuditSearchRequest } from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import 'datatables.net';
import { EventEmitter } from 'events';
import * as _moment from 'moment';
import { Subject } from 'rxjs';

@Component({
  selector: 'batchtracinglist',
  templateUrl: './batchtracinglist.component.html'
})
export class BatchProcessListComponent implements OnInit {
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective;

  dtOptions: any = {};
  dtTrigger: Subject<any> = new Subject();

  loading: boolean = false;
  public id: any;
  public sub: any;
  public dataSuc: any;
  public date: any;
  public warningMesage: string;
  public batchID: string;

  _batchTracingServiceRequest: BatchTracingServiceRequest = new BatchTracingServiceRequest();

  constructor(public router: Router, private renderer: Renderer,
    public http: HttpClient, public _batchTracingService: BatchTracingService, public headernavService: headernavService) {
  }
  newtrace() {
    var customerUserId = localStorage.getItem('userid');
    this._batchTracingService.checkBatchProcessConfiguration(customerUserId).subscribe((res) => {
      if (res == "")
        this.router.navigate(['batchprocess/batchprocessdetail']);
      else {
        this.warningMesage = res;
        document.getElementById("openPopUp").click();
      }
    });
  }
  ngOnInit(): void {
    if (localStorage.getItem('userid') != null && localStorage.getItem('userid') != "undefined") {
      this.headernavService.toggle(true);
      if (localStorage.getItem('name') != null && localStorage.getItem('name') != 'undefined') {
        this.headernavService.updateUserName(localStorage.getItem('name'));
      }
      var customerid = localStorage.getItem('customerid');
    }
    else {
      this.router.navigate(['/login']);
    }

    var req = JSON.stringify(this._batchTracingServiceRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/BatchTracing/BatchTraceServiceList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },
        data: function (data) {
          var req1 = JSON.parse(req);
          req1.customerId = customerid;
          req1.dtRequest = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },
      columns: [
        {
          data: 'isDataDownloaded', title: 'File Name', name: 'IsDataDownloaded',
          "render": function (data, type, row) {
            var linkaction = row.fileName;
            return linkaction;
          }
        },
        {
          data: 'uploadDate', title: 'Upload Date', name: 'UploadDate', 
          "render": function (data, type, row) {
            return data;
          }
        },
        {
          data: 'totalRecords', title: 'Total Records', name: 'TotalRecords',
          "render": function (data, type, row) {
            return data.toLocaleString('en-us');
          }
        },
        {
          data: 'foundRecords', title: 'Found Records', name: 'FoundRecords',
          "render": function (data, type, row) {
            return data.toLocaleString('en-us');
          }
        },
        { data: 'isDataDownloaded', title: 'File Downloaded', name: 'IsDataDownloaded' },
        { data: 'adminCertified', title: 'File Downloaded', name: 'adminCertified', visible: false },
        {
          data: 'id', title: 'Batch Number', name: 'ID',
          "render": function (data, type, row) {
            var linkaction = '<button class="btn btn-link" style="padding:0px"  batchtracing-viewchart-id=' + data + '>' + row.batchNumber + ' </button>';
            return linkaction;
          }
        },
        {
          data: 'id', title: 'Raise Invoice', name: 'ID',
          "render": function (data, type, row) {
            var linkaction = 'Invoice Raised';
            if (row.proFormaInvoiceId == "")
              linkaction = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" batchtracing-invoice-id=' + data + '>' + 'Raise Invoice' + ' </button>';
            return linkaction;
          }
        },
        {
          data: 'id', title: 'Cancel Batch', name: 'ID',
          "render": function (data, type, row) {
            var linkaction = 'Not Allowed';
            if (row.proFormaInvoiceId == "")
              linkaction = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" batchtracing-remove-id=' + data + '>' + 'Cancel' + ' </button>';
            return linkaction;
          }
        },
      ],
      language: {
        search: "Filter:"
      },
      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      pageLength: 10,
      order: [1, "desc"]
    };
  }
  rerender(): void {
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload();
    });
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {

      if (event.target.hasAttribute("batchtracing-viewchart-id")) {
        this.router.navigate(['batchprocess/batchprocessview', event.target.getAttribute("batchtracing-viewchart-id")]);
      }

      if (event.target.hasAttribute("batchtracing-invoice-id")) {
        this.warningMesage = "Do you want to generate Invoice?";
        this.batchID = event.target.getAttribute("batchtracing-invoice-id");
      }

      if (event.target.hasAttribute("batchtracing-remove-id")) {
        this.warningMesage = "Do you want to cancel the Batch?";
        this.batchID = event.target.getAttribute("batchtracing-remove-id");
      }
    });
    this.dtTrigger.next();
  }
  Submit(batchID) {
    this.loading = true;
    if (this.warningMesage == "Do you want to generate Invoice?") {
      this._batchTracingService.generateProformaInvoice(localStorage.getItem('userid'), batchID).subscribe((response) => {
        this.loading = false;
        this.rerender();
      });
    }
    else if (this.warningMesage == "Do you want to cancel the Batch?") {
      this._batchTracingService.removeBatchTrace(batchID).subscribe((response) => {
        this.loading = false;
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
