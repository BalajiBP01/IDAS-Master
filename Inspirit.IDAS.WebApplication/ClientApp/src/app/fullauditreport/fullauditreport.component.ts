import { Component, OnInit, Renderer, ViewChild } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router, RouterLink } from '@angular/router'
import { DataTablesModule } from 'angular-datatables'
import { DataTableDirective } from 'angular-datatables'
import * as $ from 'jquery'
import 'datatables.net'
import { DatePipe } from '@angular/common'
import {
  FullAuditReportService,
  FullAuditSearchRequest,
} from '../services/services'
import { isNullOrUndefined } from 'util'
import { Subject } from 'rxjs'
import * as _moment from 'moment'
import { headernavService } from '../header-nav/header-nav.service'
import { retry } from 'rxjs/operators'

@Component({
  selector: 'app-fullauditreport',
  templateUrl: './fullauditreport.component.html',
})
export class FullAuditReportComponent implements OnInit {
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective

  dtOptions: any = {}
  dtTrigger: Subject<any> = new Subject()
  fromDatestring: string
  toDatestring: string
  name: any
  isuserExists: any
  loading: boolean = false
  _DataTableRequest: FullAuditSearchRequest = new FullAuditSearchRequest()

  message: any

  mode: string = 'View'
  customerId: any
  numOfDays: number
  customerUserId: any
  firstName: string

  constructor(
    public router: Router,
    private renderer: Renderer,
    private datePipe: DatePipe,
    public headernavService: headernavService,
  ) {}
  ngOnInit(): void {
    this.loading = true
    this.numOfDays = parseInt(localStorage.getItem('numOfDays'))
    this.isuserExists = localStorage.getItem('userid')
    if (this.isuserExists != null && this.isuserExists != 'undefined') {
      this.headernavService.toggle(true)
      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }

      this.customerId = localStorage.getItem('customerid')
      this.customerUserId = localStorage.getItem('userid')
      this.firstName = localStorage.getItem('firstName')

      this.change()
      this.fromDatestring = this.datePipe.transform(
        new Date(new Date().getFullYear(), new Date().getMonth(), 1),
        'yyyy-MM-dd',
      )
      this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd')
    } else {
      this.router.navigate(['/login'])
    }
    if (this.numOfDays > 30) {
      document.getElementById('pwdReset').click()
    }
  }

  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute('view-id')) {
        this.router.navigate([
          'FullAuditReport/FullAuditDataList',
          event.target.getAttribute('view-id'),
        ])
      }
    })
    this.dtTrigger.next()
  }
  load() {
    this.loading = false
  }

  change() {
    this._DataTableRequest = new FullAuditSearchRequest()
    this._DataTableRequest.customerId = this.customerId
    this._DataTableRequest.customerUserId = this.customerUserId

    var req = JSON.stringify(this._DataTableRequest)

    this.dtOptions = {
      ajax: {
        url: '/api/fullauditreport/FullAuditDataList',
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) {
          console.log(error)
        },

        data: function (data) {
          var req1 = JSON.parse(req)
          req1.fromdate = $('#fromdate').val()
          req1.todate = $('#todate').val()
          req1.dtRequest = data
          var req2 = JSON.stringify(req1)
          return req2
        },
      },

      columns: [
        {
          data: 'dateTime',
          title: 'Date',
          name: 'dateTime',
          render: function (data, type, row) {
            return data
          },
        },
        { data: 'name', title: 'User Name', name: 'name', orderable: false },
        { data: 'creditPoints', title: 'Credit', name: 'creditPoints' },
        {
          data: 'searchType',
          title: 'Type',
          name: 'searchType',
          render: function (data, type, row) {
            if (data == 'Profile') return 'Consumer'
            else return data
          },
        },
        { data: 'searchCriteria', title: 'Criteria', name: 'searchCriteria' },
        {
          data: 'userEmail',
          title: 'Email Address',
          name: 'userEmail',
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
      pagingType: 'full_numbers',
      pageLength: 10,
      scrollX: true,
      dom: 'lBfrtip',
      buttons: [
        {
          extend: 'collection',
          text: 'Export',
          buttons: [
            {
              extend: 'copy',
              filename: 'Full Audit Report',
              title: 'Full Audit Report',
            },
            {
              extend: 'print',
              filename: 'Full Audit Report',
              title: 'Full Audit Report',
            },
            {
              extend: 'excel',
              filename: 'Full Audit Report',
              title: 'Full Audit Report',
            },
            {
              extend: 'pdf',
              filename: 'Full Audit Report',
              title: 'Full Audit Report',
            },
            {
              extend: 'csvHtml5',
              filename: 'Full Audit Report',
              title: 'Full Audit Report',
            },
          ],
        },
      ],
    }
  }
  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe()
  }

  rerender(): void {
    if (
      new Date($('#fromdate').val().toString()) >
      new Date($('#todate').val().toString())
    ) {
      this.message = 'To date should be greater than from date'
      document.getElementById('errormsg').click()
      return
    }
    this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
      dtInstance1.ajax.reload()
    })
  }
  resetPasswordRoute() {
    this.router.navigate(['resetpassword'])
  }
}
