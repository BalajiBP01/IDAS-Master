import {
  Component,
  Inject,
  OnInit,
  ChangeDetectorRef,
  Renderer,
  OnDestroy,
} from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import {
  TracingService,
  ConsumerSearchRequest,
  ConsumerSearchResponse,
  DataTableRequest,
} from '../services/services'
import { headernavService } from '../header-nav/header-nav.service'
import { ActivatedRoute } from '@angular/router'
import { DataTablesModule } from 'angular-datatables'
import * as $ from 'jquery'
import 'datatables.net'
import * as _moment from 'moment'
import { debug } from 'util'
import { load } from 'ssf/types'
import { TabHeadingDirective } from 'ngx-bootstrap'
import { Console } from '@angular/core/src/console'

@Component({
  selector: 'tracingSearch/consumerSearchResult',
  templateUrl: './consumerSearchResult.component.html',
})
export class ConsumerSearchResultComponent implements OnInit, OnDestroy {
  dtOptions: DataTables.Settings = {}

  public id: any
  public sub: any
  public dataSuc: any
  public date: any
  public loading = false
  public oldTime: any
  public newTime: any
  public timetaken: any
  public time: any
  public idnumber: any
  IsRestrictedCustomerUser: any
  IsRestrictedCustomer: any

  isuserExists: any
  points: any = '0'
  userid: any
  customerid: any
  name: any
  loaddata: boolean = false
  isglobalSearch: boolean = false
  req: any

  // krishna start
  isXDS: any
  userName: any
//  enquiryReason: any
  // krishna end

  getpoint: boolean = false

  _tracingRequest: ConsumerSearchRequest = new ConsumerSearchRequest()
  data_req: ConsumerSearchRequest = new ConsumerSearchRequest()

  _dtReq: DataTableRequest = new DataTableRequest()
  _tracingResponse: ConsumerSearchResponse = new ConsumerSearchResponse()
  globalSearch: any
  errormsg: any
  istrailuser: boolean = false

  constructor(
    public router: Router,
    private renderer: Renderer,
    public headernavService: headernavService,
    public http: HttpClient,
    public tracingService: TracingService,
    public route: ActivatedRoute,
    private chRef: ChangeDetectorRef,
  ) {
    this.userid = localStorage.getItem('userid')
    this.customerid = localStorage.getItem('customerid')

    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end

    // pending points
    //if (this.isXDS == 'NO') {
      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          this.headernavService.updatePoints(this.points)
        })
    //}
  }

  ngOnInit(): void {
    this.loading = true
    this.isuserExists = localStorage.getItem('userid')

    this.IsRestrictedCustomer = localStorage.getItem('IsRestrictedCustomer')
    this.IsRestrictedCustomerUser = localStorage.getItem(
      'IsRestrictedCustomerUser',
    )

    // console.log('consumer_IsRestrictedCustomer: ' + this.IsRestrictedCustomer)
    // console.log(
    //   'consumer_IsRestrictedCustomerUser: ' + this.IsRestrictedCustomerUser,
    // )
    let usr = localStorage.getItem('trailuser')
    if (usr == 'YES') this.istrailuser = true
    else this.istrailuser = false

    if (this.isuserExists) {
      this.headernavService.toggle(true)
      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }

      this._tracingRequest = new ConsumerSearchRequest()
      this.sub = this.route.queryParams.subscribe((params) => {
        this._tracingRequest.iDNumber =
          params['iDNumber'] /*+ params['dateOfBirth']*/
        this._tracingRequest.dateOfBirth = params['dateOfBirth']
        this._tracingRequest.firstname = params['firstname']
        this._tracingRequest.fromDate = params['fromDate']
        this._tracingRequest.phoneNumber = params['phoneNumber']
        this._tracingRequest.surname = params['surname']
        this._tracingRequest.toDate = params['toDate']
        this._tracingRequest.address = params['address']
        this._tracingRequest.type = params['type']
        this._tracingRequest.globalSearch = params['globalSearch']
        this._tracingRequest.emailaddress = params['emailaddress']

        this._tracingRequest.enquiryReason = params['enquiryReason']
        this._tracingRequest.customerRefNum = params['customerRefNum']
        this._tracingRequest.voucherCode = params['voucherCode']
        this._tracingRequest.isXDS = params['isXDS']
        this._tracingRequest.userName = params['userName']

        this._tracingRequest.isTrailuser = this.istrailuser
        this.idnumber = this._tracingRequest.iDNumber
        sessionStorage.setItem('idnum', this.idnumber)
        this.req = params['searchTimereq']
        if (this.req == 'TRUE') {
          this.isglobalSearch = true
        } else {
          this.isglobalSearch = false
        }
        this._tracingRequest.userId = this.userid
        this._tracingRequest.custId = this.customerid
        this.data_req = this._tracingRequest
      })

      this.oldTime = new Date()
      var req = JSON.stringify(this._tracingRequest)
      this.dtOptions = {
        ajax: {
          url: '/api/Tracing/TracingConsumerSearch',
          type: 'POST',
          contentType: 'application/json; charset=UTF-8',
          error: function (xhr, error, code) {
            console.log(error)
          },
          data: function (data) {
            console.log(data)
            var req1 = JSON.parse(req)
            var req2 = JSON.stringify(req1)
            return req2
          },
        },

        columns: [
          {
            data: 'consumerId',
            title: 'Id Number',
            name: 'Consumer Id',
            render: function (data: any, type: any, row: any) {
              const IdNumbSession = sessionStorage.getItem('idnum')

              if (IdNumbSession !== 'undefined') {
                return (
                  '<button class="btn btn-link" style="padding:0px;word-break: break-all;white-space: normal"  view-person-id=' +
                  data +
                  '> ' +
                  row.iDNumber +
                  ' </button>'
                )
              } else
                return (
                  '<button class="btn btn-link" style="padding:0px;word-break: break-all;white-space: normal"  view-person-id=' +
                  data +
                  '> ' +
                  /*row.iDNumber +*/ row.iDNumber.substring(0, 6) +
                  'XXXX' +
                  row.iDNumber.substring(10, row.iDNumber.length) +
                  ' </button>'
                )
            },
          },
          {
            data: 'gender',
            title: 'Gender',
            name: 'gender',
            render: function (data, type, row) {
              if (data == '0') {
                return '<img style="width:30px" title="Female" src="../../assets/demo/demo2/media/img/Icons/Female.png"/>'
              } else if (data == '1') {
                return '<img style="width:30px"  title="Male" src="../../assets/demo/demo2/media/img/Icons/Male.png"/>'
              } else {
                return null
              }
            },
          },
          { data: 'fullName', title: 'Full Name', name: 'fullName' },
          {
            data: 'age',
            title: 'Age',
            name: 'age',
            render: function (data, type, row) {
              if (row.isdeceased == true) return 'DECEASED'
              else return data
            },
          },
          {
            data: 'dateOfBirth',
            title: 'Date Of Birth',
            name: 'dateOfBirth',
            render: function (data, type, row) {
              return _moment(new Date(data).toString()).format('YYYY-MM-DD')
            },
          },
          {
            data: 'totalCount',
            title: 'totalCount',
            name: 'totalCount',
            visible: false,
          },
        ],
        initComplete: function () {
          document.getElementById('load').click()
        },
        language: {
          search: 'Filter:',
        },
        processing: true,
        serverSide: false,
        responsive: true,
        scrollX: true,
        pageLength: 10,
      }
    } else {
      this.router.navigate(['/login'])
    }
  }

  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute('view-person-id')) {
        if (this.points > 0) {
          this._tracingRequest.type = 'Profile'
          this._tracingRequest.isTrailuser = this.istrailuser
          this._tracingRequest.consumerId = event.target.getAttribute(
            'view-person-id',
          )
          this.router.navigate(['tracingSearch/personProfile'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
          this._tracingRequest = null
        } else {
          alert("You don't have credits")
          this.router.navigate(['tracingSearch'], {
            queryParams: { type: 'Profile' },
            skipLocationChange: true,
          })
        }
      }
    })
  }

  list() {
    this.router.navigate(['tracingSearch'], {
      queryParams: { type: 'Profile' },
      skipLocationChange: true,
    })
  }
  load() {
    this.loading = false
    this.newTime = new Date()
    this.timetaken = (
      (this.newTime.getTime() - this.oldTime.getTime()) /
      1000
    ).toFixed(2)
    console.log(this.timetaken)
    this.loaddata = true
  }
  ngOnDestroy() {
    this._tracingRequest = null
    this._dtReq = null
    this.sub.unsubscribe()
  }

  tracing() {
    this.istrailuser = this.istrailuser
    this.router.navigate(['tracingSearch'], {
      queryParams: { type: 'Consumer' },
      skipLocationChange: true,
    })
  }

  GlobalSearch2(idnumber) {
    //if (this.points != null && this.points != 'undefined' && this.points > 0) {
    //  if (this.globalSearch != null && this.globalSearch != undefined && this.globalSearch != ' ') {
    //    if (this.profileType == 1) {
    this._tracingRequest.type = 'Profile'
    this._tracingRequest.userId = this.userid
    this._tracingRequest.custId = this.customerid
    this._tracingRequest.globalSearch = idnumber //this.globalSearch;
    //if (this.istrailuser == "YES")
    //  this._tracingRequest.isTrailuser = true;
    //else
    //  this._tracingRequest.isTrailuser = false;
    this.router.navigate(['tracingSearch/consumerSearchResult'], {
      queryParams: this._tracingRequest,
      skipLocationChange: true,
    })
    //  }
    //else {
    //  this._companyRequest.globalSearch = this.globalSearch;
    //  this._tracingRequest.custId = this.customerid;
    //  this._companyRequest.type = "Company";
    //  if (this.istrailuser == "YES")
    //    this._companyRequest.isTrailuser = true;
    //  else
    //    this._companyRequest.isTrailuser = false;
    //  this._tracingRequest.custId = this.customerid;
    //  this.router.navigate(['tracingSearch/commercialSearchResult'], { queryParams: this._companyRequest, skipLocationChange: true });
    //}
    // }
    //}
    //else {
    //  this.errormsg = "You don't have credits";
    //}
  }

  GlobalSearch() {
    this._tracingRequest = new ConsumerSearchRequest()
    if (this.points > 0) {
      if (
        this.globalSearch != null &&
        this.globalSearch != undefined &&
        this.globalSearch != ' '
      ) {
        this._tracingRequest.type = 'Profile'
        this._tracingRequest.userId = this.userid
        this._tracingRequest.custId = this.customerid
        this._tracingRequest.globalSearch = this.globalSearch
        this._tracingRequest.isTrailuser = this.istrailuser
        if (this.isglobalSearch == false) {
          this._tracingRequest.searchTimereq = 'TRUE'
          this.router.navigate(['tracingSearch/consumergloSearchResult'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
        } else {
          this._tracingRequest.searchTimereq = 'FALSE'
          this.router.navigate(['tracingSearch/consumerSearchResult'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
        }
      }
    } else {
      this.errormsg("You don't have credits")
    }
  }
}
