import { Component, Inject, transition } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import {
  LeadGenerationResponse, BatchTraceServices,LeadGenerationService
} from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import { EventEmitter } from 'events';
import { debug, isNullOrUndefined } from 'util';
import * as XLSX from 'xlsx';
import { PercentPipe } from '@angular/common';
import { DataTable } from 'angular2-datatable';

@Component({
  selector: 'leadinformation',
  templateUrl: './leadinformation.html'
})

export class LeadInformationComponent {
  id: string;
  private sub: any;
  warningMesage: string;
  name: any;
  loading: boolean = false;
  public pipe: any = new PercentPipe("en-US");
  public response: LeadGenerationResponse = new LeadGenerationResponse();
  public chartreq: LeadGenerationResponse = new LeadGenerationResponse();
  public ageGroupOptions = {
    element: 'bar-example',
    horizontal: false,
    xkey: 'y',
    ykeys: ['a', 'b'],
    labels: ['Male', 'Female']
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
  public _batchservice: BatchTraceServices


  constructor(private router: Router, private route: ActivatedRoute,
    private httpclient: HttpClient,
    public _leadservice: LeadGenerationService, public headernavService: headernavService) {
    this.name = localStorage.getItem('name');
    if (this.name != null && this.name != 'undefined') {
      this.headernavService.updateUserName(this.name);
    }
  }
  ngOnInit(): void {
    if (localStorage.getItem('userid') != null && localStorage.getItem('userid') != "undefined") {
      this.headernavService.toggle(true);
      if (localStorage.getItem('name') != null && localStorage.getItem('name') != 'undefined') {
        this.headernavService.updateUserName(localStorage.getItem('name'));
      }
      var userid = localStorage.getItem('userid');
      var customerid = localStorage.getItem('customerid');
    }
    else
      this.router.navigate(['/login']);
    this.loading = true;
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
      this.headernavService.toggle(true);
      if (this.id != 'undefined' || this.id != null) {
        this.batchGeneration();
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
  batchGeneration() {
    this.headernavService.toggle(true);
    this._leadservice.getLeadInformation(this.id).subscribe((result) => {
      this.chartreq = result;
      this.generateCharts(this.chartreq);
    });
  }
  generateCharts(leads: LeadGenerationResponse) {
    this.headernavService.toggle(true);
    this.response.ageGrouGenders = JSON.parse(leads.profileAgeGroup);
    this.response.incomeBrackets = JSON.parse(leads.profileIncomeBrackets);
    this.response.locationDistributorAgeGroups = JSON.parse(leads.profileLocationDistributorAge);
    this.response.morrisAlloyBreakDowns = JSON.parse(leads.profileAlloyBreakdown);
    this.response.morrisGenders = JSON.parse(leads.profileGender);
    this.response.morrisMaritalStaus = JSON.parse(leads.profileMarital);
    this.response.morrisRiskCategories = JSON.parse(leads.profileRiskCategory);
    this.response.totalRecordsAvailables = JSON.parse(leads.profileTotalRecords);
    console.log(this.response.morrisGenders);
    this.loading = false;
    //for bar chart
    this.barChartDataAgeGroup = [
      { data: this.response.ageGrouGenders.male , label: 'Male' },
      { data: this.response.ageGrouGenders.feMale, label: 'Female' }
    ];

    this.barChartLabelsIncome = this.response.incomeBrackets.incomeBracketColumns;
    this.barChartDataIncome = [
      { data: this.response.incomeBrackets.incomeBracketMale, label: 'Male' },
      { data: this.response.incomeBrackets.incomeBracketFeMale, label: 'Female' }
    ];
  }


  batchsearch() {
    this.router.navigate(['leadGeneration']);
  }
}
