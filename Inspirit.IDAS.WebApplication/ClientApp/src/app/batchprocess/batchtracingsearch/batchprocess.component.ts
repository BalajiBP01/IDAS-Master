import { Component, Inject, transition } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import {
  BatchTracingService, BatchTrace,
  BatchTraceServices, InvoiceGenResponse
} from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import { EventEmitter } from 'events';
import { debug, isNullOrUndefined } from 'util';
import * as XLSX from 'xlsx';
import { forEach } from '@angular/router/src/utils/collection';
import { PercentPipe } from '@angular/common';

@Component({
  selector: 'batchprocess-detatil',
  templateUrl: './batchprocess.component.html'
})

export class BatchProcessComponent {
  arrayBuffer: any;
  array_values: any;
  columnNames: any;
  loading: boolean = false;
  id: string;
  message: string;
  warningMesage: string;
  progress: number;
  notFoundRecords: number;
  _baseUrl: string;
  isdisabled: boolean = true;
  isVisible: boolean = false;
  isProcess: boolean = false;
  points: number = 0;
  batchId: any;
  fileName: string;
  userId: any;

  responses: string[]=[];


  dtOptions: DataTables.Settings = {};

  public pipe: any = new PercentPipe("en-US");
  public response: InvoiceGenResponse = new InvoiceGenResponse();
  public batchTrace: BatchTrace = new BatchTrace();

  public batchTracingServices: BatchTraceServices = new BatchTraceServices();

  //Bar chart
  public colors = [
    { // Male
      backgroundColor: 'rgb(95, 139, 149)'//'rgb(248, 202, 0)'//'rgba(77,83,96,0.2)'
    },
    { // Female.
      backgroundColor: 'rgb(186, 77, 81)'//'rgba(30, 169, 224, 0.8)'
    }
  ]

  public barChartLabelsAgeGroup: string[] = ['61+', '41-60', '35-40', '18-34'];
  public horizontalBarType: string = 'horizontalBar';
  public barChartLegend: boolean = true;
  public barChartLabelsIncome = new Array();
  public barChartDataAgeGroup = new Array();
  public barChartDataIncome = new Array();

  constructor(private router: Router,
    private httpclient: HttpClient,
    public _batchTracingService: BatchTracingService, public headernavService: headernavService) {
    this.batchTracingServices.totalRecords = 0;
    this.batchTracingServices.foundRecords = 0;
    this.notFoundRecords = 0;
  }
  ngOnInit(): void {
    this.isdisabled = true;
    if (localStorage.getItem('userid') != null && localStorage.getItem('userid') != "undefined") {
      this.headernavService.toggle(true);
      if (localStorage.getItem('name') != null && localStorage.getItem('name') != 'undefined') {
        this.headernavService.updateUserName(localStorage.getItem('name'));
      }

      var userid = localStorage.getItem('userid');
      this.userId = userid;
      console.log(this.userId);
      var customerid = localStorage.getItem('customerid');
      this._batchTracingService.checkBatchProcessConfiguration(userid).subscribe((res) => {
        if (res != "") {
          this.warningMesage = res;
          document.getElementById("openModalButton").click();          
        }
      });


      this._batchTracingService.getPoints(userid, customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    }
    else
      this.router.navigate(['/login']);
  }
  Importfile(files) {
    localStorage.setItem('filename', files[0].name);
    this._batchTracingService.getIdNos(files[0].name, this.userId).subscribe((res)=> {
      this.responses = res;
      this.responses = res.slice(0, 100);
      if (this.responses.length >= 100 && this.responses.length <= 100000) {
        this.loading = false;
        this.isProcess = true;
        this.dtOptions = {
          scrollX: true,
          pageLength: 10,
          serverSide: false,
          pagingType: 'full_numbers',
          language: {
            search: "Filter:"
          }
        };
      }
      else {
        this.isProcess = false;
        this.loading = false;
        this.warningMesage = "Should be minimum 100 and below 50000 records in the document";
        document.getElementById("openModalButton").click();
        this.fileName = "";
      }
    });
  }
  process(files) {
    localStorage.setItem('filename', files[0].name);
    this.router.navigate(['batchprocess/batchprocessview']);
  }
  change(files) {
    this.loading = true;
    if (files.length === 0) {
      this.warningMesage = "Please upload your excel document";
      document.getElementById("openModalButton").click();
      this.fileName = "";
      this.loading = false;
      return false;
    }
    this.fileName = files[0].name;
    const formData = new FormData();
    for (let file of files)
      formData.append(file.name, file, this.userId);
    const uploadReq = new HttpRequest('POST', 'api/batchtracing/UploadFile', formData, {
      reportProgress: true,
    });
    this.httpclient.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.Response) {
        this.message = event.body.toString();
        if (this.message == "Uploaded Successfully") {
          this.Importfile(files);
        }
      }
    });
  }
  list() {
    this.router.navigate(['batchprocess']);
  }
}
