import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import * as $ from 'jquery';
import 'datatables.net';
import { Invoice, InvoiceDataResponse, InvoiceService, InvDataTableRequest } from '../../services/services';
import * as _moment from 'moment';
import { headernavService } from '../../header-nav/header-nav.service';



@Component({
    selector: 'app-invoicelist',
    templateUrl: './invoicelist.component.html',

})

export class InvoiceListComponent implements OnInit {
    dtOptions: DataTables.Settings = {};

    _DataTableRequest: InvDataTableRequest = new InvDataTableRequest();
  isuserExists: any;

    mode: string = "View";
    custmerId: any;
  name: any;
  loading: boolean = false;

    constructor(public router: Router, private renderer: Renderer, public invoiceService: InvoiceService, public headernavService: headernavService) {

    }
  ngOnInit(): void {
    this.loading = true;

    this.isuserExists = localStorage.getItem('userid');
    if (this.isuserExists != null && this.isuserExists != "undefined") {

      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

      this.custmerId = localStorage.getItem('customerid');
      this._DataTableRequest.customerId = this.custmerId;

      this.custmerId = localStorage.getItem('customerid');
      this._DataTableRequest.customerId = this.custmerId;

      var req = JSON.stringify(this._DataTableRequest);
      this.dtOptions = {
        ajax: {
          url: "/api/Invoice/InvoiceDataList",
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
          { data: 'displayNumber', title: 'Invoice Number', name: 'displayNumber' },
          {
            data: 'date', title: 'Date', name: 'date',
            "render": function (data, type, row) {
              return _moment(new Date(data).toString()).format('YYYY-MM-DD');
            }
          },
          {
            data: 'total', title: 'Total', name: 'total',
            "render": function (data, type, row) {
              var value = data.toFixed(2);
              return value;
            }
          },

          {
            data: 'id', title: 'Action', name: 'id',
            "render": function (data: any, type: any, full: any) {
              return '<button class="btn btn-link" style="padding:0px"  inv-view-id=' + data + '> View </button>'
            }
          },
          {
            data: 'id', title: 'Action', name: 'inv-pay-id',
            "render": function (data: any, type: any, row: any) {

              if (row.isPayed == "Pay") {
                return 'Pay';
              }
              else if (row.isPayed == "Payed") {
                return 'Paid';
              }
            }
          },
          {
            data: 'isCancelled', title: 'Is Cancelled', name: 'isCancelled',
            "render": function (data: any, type: any, full: any) {
              if (data == true)
                return "TRUE";
              else
                return "FALSE";
            }
          }
        ],
        initComplete: function () {
          document.getElementById('load').click();
        },
        language: {
          search: "Filter:"
        },
        processing: true,
        serverSide: false,
        pagingType: 'full_numbers',
        pageLength: 10,
        scrollX: true,
      };
    } else {
      this.router.navigate(['/login']);
    }
    }

    ngAfterViewInit(): void {
        this.renderer.listenGlobal('document', 'click', (event) => {
            if (event.target.hasAttribute("inv-view-id")) {
              this.router.navigate(['invoicelist/invoicedetail'], { queryParams: { id: event.target.getAttribute("inv-view-id")}, skipLocationChange: true });
            }
            else if (event.target.hasAttribute("inv-pay-id")) {
                
                console.log(event.target.getAttribute("inv-pay-id"));
                this.router.navigate(['invoicelist/payment', event.target.getAttribute("inv-pay-id")]);
            }
            else if (event.target.hasAttribute("view-billingType")) {
                this.router.navigate(['payment', event.target.getAttribute("view-billingType")]);
            }
        });
    }

    invoicedetailform() {
        this.router.navigate(['invoicelist/invoicedetail']);
  }

  load() {
    this.loading = false;
  }
}


