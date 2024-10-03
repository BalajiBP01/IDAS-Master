//import { Component, OnInit, Renderer } from '@angular/core';
//import { Router } from '@angular/router';
//import { DataTablesModule } from 'angular-datatables';
//import * as $ from 'jquery';
//import 'datatables.net';
//import { DataTableRequest } from '../../services/services';
//import { ProductPackageRate, ProductRateService } from '../../services/services';


//@Component({
//  selector: 'app-productrateslist',
//  templateUrl: './productrateslist.component.html'
//})

//export class ProductRateListComponent {

//  dtOptions: DataTables.Settings = {};

//  _DataTableRequest: DataTableRequest = new DataTableRequest();
//  mode: string = "View";

//  constructor(public router: Router, private renderer: Renderer) { }

//  ngOnInit(): void {
//    var req = JSON.stringify(this._DataTableRequest);
//    this.dtOptions = {
//      ajax: {
//        url: "/api/ProductRate/ProductRateDetailList",
//        type: 'POST',
//        contentType: 'application/json; charset=UTF-8',
//        error: function (xhr, error, code) { console.log(error); },

//        data: function (data) {
//          var req1 = JSON.parse(req);
//          req1 = data;
//          var req2 = JSON.stringify(req1);
//          return req2;
//        }
//      },

//      columns: [

//        { data: 'product', title: 'Product Name', name: 'product' },
//        { data: 'productId', title: 'ProductId', name: 'productId' },

//        { data: 'minLimit', title: 'MinLimit', name: 'minLimit' },
//        { data: 'maxLimit', title: 'MaxLimit', name: 'maxLimit' },
//        { data: 'unitPrice', title: 'UnitPrice', name: 'unitPrice' },

//        {
//          data: 'id', title: 'Action', name: 'id',
//          "render": function (data: any, type: any, full: any) {
//            return '<button class="btn btn-link" style="padding:0px" view-id=' + data + '> View </button>'
//          }
//        },
//      ],

//      processing: true,
//      serverSide: true,
//      pagingType: 'full_numbers',
//      pageLength: 10,
//    };
//  }

//  ngAfterViewInit(): void {
//    this.renderer.listenGlobal('document', 'click', (event) => {
//      if (event.target.hasAttribute("view-id")) {
//        this.router.navigate(['productrateslist/productratesdetails', event.target.getAttribute("view-id")]);
//      }
//    });
//  }

//  ProductRatesdetailform() {
//    this.router.navigate(['productrateslist/productratesdetails']);
//  }
//}
