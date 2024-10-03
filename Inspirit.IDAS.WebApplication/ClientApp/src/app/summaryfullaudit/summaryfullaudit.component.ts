import { Component, OnInit, Renderer, ViewChild } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { DataTablesModule } from 'angular-datatables'
import { DataTableDirective } from 'angular-datatables'
import * as $ from 'jquery'
import 'datatables.net'
import { DatePipe } from '@angular/common'
import {
  SummaryFullAuditService,
  SummaryAuditSearchRequest,
} from '../services/services'
import { headernavService } from '../header-nav/header-nav.service'
import * as _moment from 'moment'
import { Subject } from 'rxjs'
import { locale } from 'moment'
import { retry } from 'rxjs/operators'

@Component({
  selector: 'app-summaryfullaudit',
  templateUrl: './summaryfullaudit.component.html',
})
export class SummaryFullAuditComponent implements OnInit {
  @ViewChild(DataTableDirective)
  dtElement: DataTableDirective
  dtOptions: any = {}
  dtTrigger: Subject<any> = new Subject()
  fromDatestring: string
  toDatestring: string
  name: any
  _DataTableRequest: SummaryAuditSearchRequest = new SummaryAuditSearchRequest()

  message: any

  mode: string = 'View'
  customerId: any
  customerUserId: any
  isuserExists: any
  loading: boolean = false
  numOfDays: number

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
          'SummaryFullAudit/SummaryAuditDataList',
          event.target.getAttribute('view-id'),
        ])
      }
    })
    this.dtTrigger.next()
  }

  change() {
    this._DataTableRequest = new SummaryAuditSearchRequest()
    this._DataTableRequest.customerId = this.customerId
    this._DataTableRequest.customerUserId = this.customerUserId
    var consumeruserid = localStorage.getItem('userid')

    var req = JSON.stringify(this._DataTableRequest)

    this.dtOptions = {
      ajax: {
        url: '/api/SummaryFullAudit/SummaryAuditDataList',
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) {
          console.log(error)
        },

        data: function (data) {
          var req1 = JSON.parse(req)
          req1.companyuserid = consumeruserid
          req1.fromdate = $('#fromdate').val()
          req1.todate = $('#todate').val()
          req1.dtRequest = data
          var req2 = JSON.stringify(req1)
          return req2
        },
      },

      columns: [
        { data: 'firstname', title: 'User Name', name: 'Firstname' },
        { data: 'creditPoints', title: 'Credit Points', name: 'CreditPoints' },
        {
          data: 'userType',
          title: 'User Type',
          name: 'userType',
          render: function (data, type, row) {
            if (data == true) return 'Admin'
            else return 'Normal'
          },
        },
        { data: 'loginName', title: 'Login Name', name: 'loginName' },
        {
          data: 'userActiveDate',
          title: 'Date user active',
          name: 'userActiveDate',
          render: function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD')
          },
        },
        {
          data: 'userLastActiveDate',
          title: 'Last Day User Login',
          name: 'userLastActiveDate',
          render: function (data, type, row) {
            document.getElementById('load').click()
            return _moment(new Date(data).toString()).format('YYYY-MM-DD')
          },
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
              filename: 'Application User Report',
              title: 'Application User Report',
            },
            {
              extend: 'print',
              filename: 'Application User Report',
              title: 'Application User Report',
            },
            {
              extend: 'excel',
              filename: 'Application User Report',
              title: 'Application User Report',
            },
            {
              extend: 'pdf',
              filename: 'Application User Report',
              title: 'Application User Report',
            },
            {
              extend: 'csvHtml5',
              filename: 'Application User Report',
              title: 'Application User Report',
            },
          ],
        },
      ],
    }
  }
  ngOnDestroy(): void {
    this.dtTrigger.unsubscribe()
  }

  load() {
    this.loading = false
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
