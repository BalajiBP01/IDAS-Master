import { Component, Inject, OnInit, Renderer, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { TracingService, AddressSearchRequest, AddressSearchResponse,ConsumerSearchRequest } from '../services/services';
import { headernavService } from '../header-nav/header-nav.service';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ActivatedRoute } from '@angular/router';
import * as _moment from 'moment';

@Component({
  selector: 'tracingSearch/addressSearchResult',
  templateUrl: './addressSearchResult.component.html'
})

export class AddressSearchResultComponent implements OnInit {

  dtOptions: DataTables.Settings = {};

  public id: any;
  public sub: any;
  _tracingRequest: AddressSearchRequest = new AddressSearchRequest();
  _tracingResponse: AddressSearchResponse = new AddressSearchResponse();
  _consumerRequest: ConsumerSearchRequest = new ConsumerSearchRequest();
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
  res: any;
  isTrailuser: any;

  oldTime: any;
  newTime: any;
  timeTaken: any;
  // krishna start 
  isXDS: any
  userName: any
  // krishna end

  constructor(public router: Router, private http: HttpClient, public tracingService: TracingService, public route: ActivatedRoute, public renderer: Renderer, public headernavService: headernavService, private modalService: NgbModal) {

    this.userid = localStorage.getItem('userid');
    this.customerid = localStorage.getItem('customerid');

    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end
    //if (this.isXDS == 'NO') {
      this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    //}
  }
  tracing() {
    this.sub.unsubscribe();
    this._tracingRequest.isTrailuser = this.isTrailuser;

    this.router.navigate(['tracingSearch'], { queryParams: { type: 'Profile' }, skipLocationChange: true });
  }
  ngOnInit(): void {
    this.oldTime = new Date();
    this.loading = true;
    this.isuserExists = localStorage.getItem('userid');
    if (this.isuserExists != null && this.isuserExists != "undefined") {

      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

      this.sub = this.route.queryParams.subscribe(params => {
        this._tracingRequest.address1 = params['address1'];
        this._tracingRequest.address2 = params['address2'];
        this._tracingRequest.address3 = params['address3'];
        this._tracingRequest.address4 = params['address4'];
        this._tracingRequest.postalCode = params['postalCode'];
      });
      var req = JSON.stringify(this._tracingRequest);
      this.dtOptions = {
        ajax: {
          url: '/api/Tracing/TracingAddressSearch',
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
          {
            data: 'address', title: 'Address', name: 'Address',
            "render": function (data, type, row) {
              var res = data.split("_").join(" ");

              return '<button class="btn btn-link" style="padding:0px;word-break: break-all;white-space: normal;min-width:300px"  view-add-id=' + row.address + '>' + res + '</button>';
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
    this.timeTaken = ((this.newTime.getTime() - this.oldTime.getTime()) / 1000).toFixed(2);
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("address-detail-id")) {
        if (this.points > 0) {
          this.searchaddress(event.target.getAttribute("address-detail-id"));
        }
        else {
          alert("You don't have credits");
          this.router.navigate(['tracingSearch'], { queryParams: { type: 'Address' }, skipLocationChange: true });
        }
      }
      else if (event.target.hasAttribute("view-add-id")) {
        this._consumerRequest = new ConsumerSearchRequest();
        var addr = event.target.getAttribute("view-add-id").split("_").join(" ");
        this._consumerRequest.address = addr;
        this._consumerRequest.type = "Profile"
        this.router.navigate(['tracingSearch/consumerSearchResult'], { queryParams: this._consumerRequest, skipLocationChange: true });

      }
    });
  }

  list() {
    this.router.navigate(['tracingSearch'], { queryParams: { type: 'Address' }, skipLocationChange: true });
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
  searchaddress(address: any) {
    this.res = address.split("_").join(" ");
    document.getElementById('addressdet').click();
    
  }
}
