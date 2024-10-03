import { Component, Input, Inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import { DirectorDetialSearchresultComponent } from '../directordetialsearchresult/directordetialsearchresult.component'
import {
  ConsumerSearchRequest,
  CompanySearchRequest,
  PersonProfileService,
  DirectorShip,
  TracingService,
} from '../services/services'
import { retry } from 'rxjs/operator/retry'
import { headernavService } from '../header-nav/header-nav.service'

@Component({
  selector: 'directorshiplist',
  templateUrl: './directorshiplist.component.html',
})
export class DirectorShipListComponent {
  dtOptions: DataTables.Settings = {}
  userid: any
  customerid: any
  consumerid: any
  loading: boolean = false
  result: any
  points: any
  @Input() directorship: any
  @Input() pagename: string
  request: ConsumerSearchRequest = new ConsumerSearchRequest()
  com_request: CompanySearchRequest = new CompanySearchRequest()
  directorinfo: DirectorShip = new DirectorShip()
  load: boolean = false
  istrailuser: boolean = false
  IsRestrictedCustomerUser: any
  IsRestrictedCustomer: any
  // krishna start 
  isXDS: any
  userName: any
  // krishna end

  constructor(
    public router: Router,
    public modalService: NgbModal,
    public service: PersonProfileService,
    public tracingService: TracingService,
    public headerservice: headernavService,
  ) {}

  ngOnInit(): void {
    this.headerservice.toggle(true)
    this.userid = localStorage.getItem('userid')
    this.customerid = localStorage.getItem('customerid')

    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end


    this.IsRestrictedCustomer = localStorage.getItem('IsRestrictedCustomer')
    this.IsRestrictedCustomerUser = localStorage.getItem(
      'IsRestrictedCustomerUser',
    )

    this.IsRestrictedCustomer = localStorage.getItem('IsRestrictedCustomer')
    this.IsRestrictedCustomerUser = localStorage.getItem('IsRestrictedCustomer')

    let usertype = localStorage.getItem('trailuser')
    if (usertype == 'YES') {
      this.istrailuser = true
    } else this.istrailuser = false

    this.dtOptions = {
      pagingType: 'full_numbers',
      scrollX: true,
      order: [4, 'desc'],
      language: {
        search: 'Filter:',
      },
    }
    this.result = this.directorship
  }
  // pending points
  searchprofile(idno: any) {
    $('#content').css('display', 'none')
    this.request.iDNumber = idno

    //if (this.isXDS == 'NO') {
      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          if (this.points > 0) {
            this.service.getConsumerID(idno).subscribe((result) => {
              this.consumerid = result
              if (this.consumerid != 0) {
                this.request.type = 'Profile'
                this.request.consumerId = this.consumerid
                $('#content').css('display', 'block')
                this.router.navigate(['tracingSearch/personProfile'], {
                  queryParams: this.request,
                  skipLocationChange: true,
                })
              } else {
                document.getElementById('nodata').click()
                return
              }
            })
          } else {
            document.getElementById('nopoints').click()
            return
          }
        })
    //}

  }
  // pending points
  searchcompany(id: any, companyname: any) {
    $('#content').css('display', 'none')

    //if (this.isXDS == 'NO') {
      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          if (this.points > 0) {
            if (id != 0) {
              this.com_request.type = 'Company'
              this.com_request.companyName = companyname
              this.com_request.commercialId = id
              $('#content').css('display', 'block')
              this.router.navigate(['tracingSearch/companydetailresult'], {
                queryParams: this.com_request,
                skipLocationChange: true,
              })
            } else {
              document.getElementById('nodata').click()
              return
            }
          } else {
            document.getElementById('nopoints').click()
            return
          }
        })
    //}
  }

  showdetail(id: any) {
    this.load = true
    this.service.getDirectorInfo(id).subscribe((response) => {
      if (this.istrailuser == false) {
        this.directorinfo = response
      }
      this.load = false
      const modalRef = this.modalService.open(
        DirectorDetialSearchresultComponent,
        { size: 'lg' },
      )
      modalRef.componentInstance.directors = this.directorship.find(
        (x) => x.commDirId == id,
      )
      modalRef.componentInstance.directoraddress = this.directorinfo.directoraddresses
      modalRef.componentInstance.directortelephone = this.directorinfo.directortelephones
    })
  }
}
