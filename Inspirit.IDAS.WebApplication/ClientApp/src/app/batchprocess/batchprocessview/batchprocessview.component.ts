import { Component, Inject, transition } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
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
import { DataTable } from 'angular2-datatable';

@Component({
  selector: 'batchprocessview-detatil',
  templateUrl: './batchprocessview.component.html'
})

export class BatchProcessViewComponent {
  id: string;
  private sub: any;
  isVisible: boolean = false;
  isProcess: boolean = false;
  points: number = 0;
  public display = 'block';
  isdisabled: boolean;
  loading: boolean;
  totalRecords: string;
  foundRecords: string;
  notfoundRecords: string;
  warningMesage: string;
  public pipe: any = new PercentPipe("en-US");
  public response: InvoiceGenResponse = new InvoiceGenResponse();
  public batchTrace: BatchTrace = new BatchTrace();
  public batchTracingServices: BatchTraceServices = new BatchTraceServices();

  public ageGroupOptions = {
    element: 'bar-example',
    horizontal: false,
    xkey: 'y',
    ykeys: ['a', 'b'],
    labels: ['Male', 'Female'],
  }

  //Bar chart
  public colors = [
    { // Male
      backgroundColor: '#0710B1'//'rgb(95, 139, 149)'//'rgb(248, 202, 0)'//'rgba(77,83,96,0.2)'
    },
    { // Female.
      backgroundColor: '#ff80ff'//'rgb(186, 77, 81)'//'rgba(30, 169, 224, 0.8)'
    }
  ];

  public barChartLabelsAgeGroup: string[] = ['61+', '41-60', '35-40', '18-34'];
  public barChartLabelsIncome = new Array();
  public horizontalBarType: string = 'horizontalBar';
  public barChartLegend: boolean = true;
  public barChartDataAgeGroup = new Array();
  public barChartDataIncome = new Array();
  constructor(private router: Router, private route: ActivatedRoute,
    private httpclient: HttpClient,
    public _batchTracingService: BatchTracingService, public headernavService: headernavService) {

  }
  ngOnInit(): void {
    if (localStorage.getItem('userid') != null && localStorage.getItem('userid') != "undefined") {
      this.headernavService.toggle(true);
      if (localStorage.getItem('name') != null && localStorage.getItem('name') != 'undefined') {
        this.headernavService.updateUserName(localStorage.getItem('name'));
      }

      var userid = localStorage.getItem('userid');
      var customerid = localStorage.getItem('customerid');
      this._batchTracingService.getPoints(userid, customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    }
    else
      this.router.navigate(['/login']);

    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
       this.headernavService.toggle(true);
      if (typeof this.id == 'undefined' || typeof this.id == null) {
        if (localStorage.getItem('filename') == null || localStorage.getItem('filename') == "undefined")
          this.router.navigate(['batchprocess']);
        this.display = 'block';
        this.batchGeneration();
        this.isVisible = true;
        this.isProcess = false;
      }
      else {
        this.display = 'none';
        this.loading = true;
        this._batchTracingService.view(this.id).subscribe((resp) => {
          if (resp.proFormaInvoiceId != "00000000-0000-0000-0000-000000000000")
            this.isVisible = true;
          else
            this.batchTracingServices.batchId = this.id;

          this.generateCharts(resp);
          this.isProcess = true;
          this.loading = false;
        });
      }

    });
  }
  customizeLabel(point) {
    return point.argumentText + ": " + point.valueText;
  }
  customizeTooltip = (arg: any) => {
    return {
      text: arg.valueText + " - " + this.pipe.transform(arg.percent, "1.2-2")
    };
  }
  customizeTooltipForTotalRecords = (arg: any) => {
    return {
      text: arg.valueText
    };
  }
  invoice() {
    if ((typeof window !== "undefined") ? window.localStorage : null) {
      this.batchTrace.customerUserID = localStorage.getItem('userid');
    }
    this.batchTrace.totalRecords = this.batchTracingServices.totalRecords;
    this.batchTrace.foundRecords = this.batchTracingServices.foundRecords;
    this.batchTrace.isDataDownloaded = false;
    this.isdisabled = true;
    this.warningMesage = "Do you want to generate Invoice?";
  }
  cancelbatch() {
    this.warningMesage = "Do you want to cancel the Batch?";
  }
  batchGeneration() {
    this.headernavService.toggle(true);
    var userid = localStorage.getItem('userid');
    var custid = localStorage.getItem('customerid');
    var filename = localStorage.getItem('filename');
    localStorage.removeItem('filename');
    this._batchTracingService.excelValidation(filename, userid, custid).subscribe((excelvalidation) => {

      this.batchTracingServices.excelValidation = excelvalidation.excelValidation;
      if (this.batchTracingServices.excelValidation) {
        this.headernavService.toggle(true);
        this._batchTracingService.fetchingData(excelvalidation.idnumberlst, filename, custid).subscribe((fetchingData) => {
          this.batchTracingServices.fetchingData = fetchingData.fetchingData;
          if (this.batchTracingServices.fetchingData) {
            this.batchTrace = new BatchTrace();
            this.batchTrace.fileName = filename;
            this.batchTrace.customerUserID = userid;
            this.batchTrace.customerId = custid;
            this.batchTrace.outPutFileName = fetchingData.outPutFileName;
            this.batchTrace.isDataDownloaded = false;
            this.batchTrace.idNumbers = fetchingData.idnos;
            this.batchTrace.totalRecords = fetchingData.totalRecords;
            this.batchTrace.foundRecords = fetchingData.foundRecords;
            this.batchTrace.ageGroupGenders = JSON.stringify(fetchingData.ageGrouGenders);
            this.batchTrace.incomeBrackets = JSON.stringify(fetchingData.incomeBrackets);
            this.batchTrace.profileGender = JSON.stringify(fetchingData.morrisGenders);
            this.batchTrace.maritalStaus = JSON.stringify(fetchingData.morrisMaritalStaus);
            this.batchTrace.riskCategories = JSON.stringify(fetchingData.morrisRiskCategories);
            this.batchTrace.alloyBreakDowns = JSON.stringify(fetchingData.morrisAlloyBreakDowns);
            this.batchTrace.locationServices = JSON.stringify(fetchingData.locationDistributorAgeGroups);
            this.batchTrace.totalRecordsAvailable = JSON.stringify(fetchingData.totalRecordsAvailables);
            this._batchTracingService.preparingChart(this.batchTrace).subscribe((preparingChart) => {
             this.headernavService.toggle(true);
              this.batchTracingServices.preparingChart = preparingChart.preparingChart;
              if (this.batchTracingServices.preparingChart) {
                this.batchTracingServices.batchId = preparingChart.batchTrace.id;
                this.generateCharts(preparingChart.batchTrace);
                this.isVisible = false;
                this.isProcess = true;
                this.display = 'none';
              } else {
                this.display = 'none';
                document.getElementById('errormsg').click();
                return;
              }
            });
          }
          else {
            this.display = 'none';
            document.getElementById('errormsg').click();
            return;
          }
        });

      }
      else {
        this.display = 'none';
        document.getElementById('errormsg').click();
        return;
      }
    });
  }
  generateCharts(batchTrace: BatchTrace) {
    this.headernavService.toggle(true);
    this.batchTracingServices.totalRecords = batchTrace.totalRecords;
    this.batchTracingServices.foundRecords = batchTrace.foundRecords;

    this.totalRecords = batchTrace.totalRecords.toLocaleString('en-us');
    this.foundRecords = batchTrace.foundRecords.toLocaleString('en-us');
    this.notfoundRecords = (batchTrace.totalRecords - batchTrace.foundRecords).toLocaleString('en-us'); 
    this.batchTracingServices.ageGrouGenders = JSON.parse(batchTrace.ageGroupGenders);
    this.batchTracingServices.incomeBrackets = JSON.parse(batchTrace.incomeBrackets);
    this.batchTracingServices.morrisGenders = JSON.parse(batchTrace.profileGender);
    this.batchTracingServices.morrisMaritalStaus = JSON.parse(batchTrace.maritalStaus);
    this.batchTracingServices.morrisRiskCategories = JSON.parse(batchTrace.riskCategories);
    this.batchTracingServices.morrisAlloyBreakDowns = JSON.parse(batchTrace.alloyBreakDowns);
    this.batchTracingServices.locationDistributorAgeGroups = JSON.parse(batchTrace.locationServices);
    this.batchTracingServices.totalRecordsAvailables = JSON.parse(batchTrace.totalRecordsAvailable);
    //for bar chart
    this.barChartDataAgeGroup = [
      { data: this.batchTracingServices.ageGrouGenders.male, label: 'Male' },
      { data: this.batchTracingServices.ageGrouGenders.feMale, label: 'Female' }
    ];

    this.barChartLabelsIncome = this.batchTracingServices.incomeBrackets.incomeBracketColumns;
    this.barChartDataIncome = [
      { data: this.batchTracingServices.incomeBrackets.incomeBracketMale, label: 'Male' },
      { data: this.batchTracingServices.incomeBrackets.incomeBracketFeMale, label: 'Female' }
    ];
  }
  close() {
    this.display = 'none';
  }
  Submit() {
    this.loading = true;
    if (this.warningMesage == "Do you want to generate Invoice?") {
      this._batchTracingService.generateProformaInvoice(localStorage.getItem('userid'), this.batchTracingServices.batchId).subscribe((response) => {
        this.loading = false;
        this.router.navigate(['batchprocess']);
      });
    }
    else if (this.warningMesage == "Do you want to cancel the Batch?") {
      this._batchTracingService.removeBatchTrace(this.batchTracingServices.batchId).subscribe((response) => {
        this.loading = false;
        this.router.navigate(['batchprocess']);
      });
    }
  }
  batchsearch() {
    this.router.navigate(['batchprocess']);
  }
}
