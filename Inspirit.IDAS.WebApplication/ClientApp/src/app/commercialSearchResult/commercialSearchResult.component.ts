import { Component, Inject, OnInit, Renderer, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { TracingService, CompanySearchRequest, CompanySearchResponse, } from '../services/services';
import { headernavService } from '../header-nav/header-nav.service';
import { ActivatedRoute } from '@angular/router';
import * as _moment from 'moment';

@Component({
  selector: 'tracingSearch/commercialSearchResult',
  templateUrl: './commercialSearchResult.component.html'
})

export class CommercialSearchResultComponent implements OnInit, OnDestroy {

  dtOptions: DataTables.Settings = {};

  public id: any;
  public sub: any;
  _tracingRequest: CompanySearchRequest = new CompanySearchRequest();
  _tracingResponse: CompanySearchResponse = new CompanySearchResponse();
  points: any;
  customerid: any;
  public loading = false;
  userid: any;
  globalSearch: any;
  public currentpage: number = 1;
  public totalpage: any;
  public datasuc: any;
  errormsg: any;
  name: any;
  isuserExists: any;
  public oldTime: any;
  public newTime: any;
  public timetaken: any;
  istrailuser: boolean = false;
  isglobalSearch: boolean = false;
  // krishna start 
  isXDS: any
  userName: any
  // krishna end

  constructor(public router: Router, private http: HttpClient, public tracingService: TracingService, public route: ActivatedRoute, public renderer: Renderer, public headernavService: headernavService) {
    this.userid = localStorage.getItem('userid');
    this.customerid = localStorage.getItem('customerid');
    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end

    // pending points
    //if (this.isXDS == 'NO') {
      this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    //}
  }

  ngOnInit(): void {
  this.loading = true;
    this.isuserExists = localStorage.getItem('userid');
    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end

    let usr = localStorage.getItem('trailuser');
    if (usr == "YES")
      this.istrailuser = true;
    else
      this.istrailuser = false;

    if (this.isuserExists != null && this.isuserExists != "undefined") {

      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

     


  
      this.oldTime = new Date();

      this.sub = this.route.queryParams.subscribe(params => {
        this._tracingRequest.companyName = params['companyName'];
        this._tracingRequest.Client_Logo = params['Client_Logo'] 
        this._tracingRequest.companyRegNumber = params['companyRegNumber'];
        this._tracingRequest.commercialAddress = params['commercialAddress'];
        this._tracingRequest.commercialTelephone = params['commercialTelephone'];
        this._tracingRequest.globalSearch = params['globalSearch'];
        this._tracingRequest.type = params['type'];
        this._tracingRequest.isTrailuser = this.istrailuser;
        this._tracingRequest.userId = this.userid;
        this._tracingRequest.custId = this.customerid;
      });
      
      var req = JSON.stringify(this._tracingRequest);
      this.dtOptions = {
        ajax: {
          url: '/api/Tracing/TracingCommercialSearch',
          type: 'POST',
          contentType: 'application/json; charset=UTF-8',
          error: function (xhr, error, code) { console.log(error); },
          data: function (data) {
            var req1 = JSON.parse(req);
            var req2 = JSON.stringify(req1);
            return req2;
          }
        },

        columns: [
          {
            data: 'companyid', title: 'Commercial Name', name: 'companyid',
            "render": function (data: any, type: any, row: any) {
              return '<button class="btn btn-link" style="padding:0px;word-break: break-all;white-space: normal;"  view-company-id=' + data + '> ' + row.companyName + ' </button>'
            }
          },
          { data: 'companyRegNumber', title: 'Commercial Number', name: 'companyRegNumber' },
          { data: 'commercialStatusCode', title: 'commercial Status Code', name: 'commercialStatusCode',
            "orderable": false
          },
          {
            data: 'businessStartDate', title: 'Business Start Date', name: 'businessStartDate',
            "render": function (data, type, row) {
              return _moment(new Date(data).toString()).format('YYYY-MM-DD');
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
  load() {
    this.loading = false;
    this.newTime = new Date();
    this.timetaken = ((this.newTime.getTime() - this.oldTime.getTime()) / 1000).toFixed(2);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {

      if (event.target.hasAttribute("view-company-id")) {
        if (this.points > 0) {
          this._tracingRequest.type = "Company";
          this._tracingRequest.userId = this.userid;
          this._tracingRequest.custId = this.customerid;
          this._tracingRequest.isTrailuser = this.istrailuser;
          this._tracingRequest.commercialId = event.target.getAttribute("view-company-id");
          this.router.navigate(['tracingSearch/companydetailresult'], { queryParams: this._tracingRequest, skipLocationChange: true });
          this._tracingRequest = null;
        }
        else {
          alert("You don't have credits");
          this.router.navigate(['tracingSearch'], { queryParams: { type: 'Company' }, skipLocationChange: true });
        }
      }
    });
  }

  list() {
    this.router.navigate(['tracingSearch'], { queryParams: { type: 'Company' }, skipLocationChange: true });
  }
  ngOnDestroy() {
    // pending points
    //if (this.isXDS == 'NO') {
      this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    //}

    this.sub.unsubscribe();
  }
  GlobalSearch() {
   
    this._tracingRequest = new CompanySearchRequest;
    if ((this.points != null && this.points != 'undefined' && this.points > 0) || (this.isXDS == 'YES')) {
      if (this.globalSearch != null && this.globalSearch != undefined && this.globalSearch != ' ') {
        this._tracingRequest.globalSearch = this.globalSearch;
        this._tracingRequest.isTrailuser = this.istrailuser;
        this._tracingRequest.type = "Company";
        this._tracingRequest.userId = this.userid;
        this._tracingRequest.custId = this.customerid;
        if (this.isglobalSearch == false) {
          this._tracingRequest.searchTimereq = "TRUE";
          this.router.navigate(['tracingSearch/commercialgloSearchResult'], { queryParams: this._tracingRequest, skipLocationChange: true });
        }
        else {
          this._tracingRequest.searchTimereq = "FALSE";
          this.router.navigate(['tracingSearch/consumerSearchResult'], { queryParams: this._tracingRequest, skipLocationChange: true });
        }
      }
      else {
        this.errormsg("You don't have credits");
      }
    }
  }
}
