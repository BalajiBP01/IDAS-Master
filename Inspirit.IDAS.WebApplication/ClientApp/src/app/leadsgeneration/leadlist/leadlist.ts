import { Component, Inject, OnInit, ChangeDetectorRef, Renderer, transition, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DataTablesModule, DataTableDirective } from 'angular-datatables';
import { LeadGenerationService, DatatableLeadRequest } from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import 'datatables.net';
import { EventEmitter } from 'events';
import * as _moment from 'moment';
import { Subject } from 'rxjs';

@Component({
  selector: 'leadlist',
  templateUrl: './leadlist.html'
})
export class LeadListComponent implements OnInit {
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

  customerid: any;
  customeruserid: any;
  name: any;
  leadRequest: DatatableLeadRequest = new DatatableLeadRequest;
  LeadId: any;

  constructor(public router: Router, private renderer: Renderer,
    public http: HttpClient, public headernavService: headernavService, public leadservice: LeadGenerationService) {
    this.customerid = localStorage.getItem('customerid');
    this.customeruserid = localStorage.getItem('userid');
    this.name = localStorage.getItem('name');
    if (this.name != null && this.name != 'undefined') {
      this.headernavService.updateUserName(this.name);
    }
  }
  newtrace() {
    var customerUserId = localStorage.getItem('userid');
    this.leadservice.checkLeadConfig(customerUserId).subscribe((res) => {
      if (res == "")
        this.router.navigate(["leadGeneration/process"]);
      else {
        this.warningMesage = res;
        document.getElementById("openPopUp1").click();
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
    this.leadRequest.customerId = this.customerid;
    this.leadRequest.customerUserId = this.customeruserid;
    var req = JSON.stringify(this.leadRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/LeadGeneration/GetLeadList",
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
          data: 'fileName', title: 'File Name', name: 'fileName'
        },
        {
          data: 'leadDate', title: 'Upload Date', name: 'leadDate',
          "render": function (data, type, row) {
            return data;
          }
        },
        {
          data: 'requiredRecords', title: 'Requested Records', name: 'RequiredRecords',
          "render": function (data, type, row) {
            return data;
          }
        },
        { data: 'isAdminCertified', title: 'File Downloaded', name: 'isAdminCertified', visible: false },
        {
          data: 'id', title: 'Profile Report', name: 'ID',
          "render": function (data, type, row) {
            if (row.profileReport == true) {
              var linkaction = '<button class="btn btn-link" style="padding:0px"  lead-info-id=' + data + '>View</button>';
            } else {
               linkaction = "View";
            }
            return linkaction;
          }
        },
        {
          data: 'id', title: 'Lead Number', name: 'ID',
          "render": function (data, type, row) {
              var linkaction = '<button class="btn btn-link" style="padding:0px"  lead-table-id=' + data + '>' + row.leadNumber + ' </button>';
            return linkaction;
          }
        },
        {
          data: 'id', title: 'Raise Invoice', name: 'ID',
          "render": function (data, type, row) {
            var linkaction = 'Invoice Raised';
            if (row.proFormaInvoiceId == "")
              linkaction = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" lead-invoice-id=' + data + '>' + 'Raise Invoice' + ' </button>';
            return linkaction;
          }
        },
        {
          data: 'id', title: 'Cancel Lead', name: 'ID',
          "render": function (data, type, row) {
            var linkaction = 'Not Allowed';
            if (row.proFormaInvoiceId == "")
              linkaction = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModal" lead-remove-id=' + data + '>' + 'Cancel' + ' </button>';
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

      if (event.target.hasAttribute("lead-info-id")) {
        this.router.navigate(['leadGeneration/information', event.target.getAttribute("lead-info-id")]);
      }

      if (event.target.hasAttribute("lead-table-id")) {
        this.router.navigate(['leadGeneration/leadresponse', { id:event.target.getAttribute("lead-table-id"),type:"view" }]);
      }

      if (event.target.hasAttribute("lead-invoice-id")) {
        this.warningMesage = "Do you want to generate Invoice?";
        this.LeadId = event.target.getAttribute("lead-invoice-id");
      }

      if (event.target.hasAttribute("lead-remove-id")) {
        this.warningMesage = "Do you want to cancel the Lead?";
        this.LeadId = event.target.getAttribute("lead-remove-id");
      }
    });
    this.dtTrigger.next();
  }
  Submit(LeadId) {
    this.loading = true;
    if (this.warningMesage == "Do you want to generate Invoice?") {
      this.leadservice.generateProformaInvoice(localStorage.getItem('userid'), LeadId).subscribe((response) => {
        this.loading = false;
        this.rerender();
      });
    }
    else if (this.warningMesage == "Do you want to cancel the Lead?") {
      this.leadservice.cancelLead(LeadId).subscribe((response) => {
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
