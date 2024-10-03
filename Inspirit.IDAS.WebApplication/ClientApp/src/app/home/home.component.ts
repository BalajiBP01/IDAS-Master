import { Component, OnInit } from '@angular/core'
import { headernavService } from '../header-nav/header-nav.service'
import { Router } from '@angular/router'
import { FooterService } from '../footer/footer.service'
// import { DatePipe } from '@angular/common'
import {
  SecurityService,
  DashboardService,
  DashboardVm,
} from '../services/services'
import { PopupComponent } from '../popup/popup.component'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import { MessagePopupComponent } from '../messagepopup/messagepopup.component'
import { stagger, AnimateTimings } from '@angular/core/src/animation/dsl'
declare var $: any

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent implements OnInit {
  name: any
  isuserexists: any
  messages: any
  marritalStatus: any = 'm'
  divorsed: any = 'm'
  married: any = 'm'
  resetDate: Date
  todayDate: Date

  result: DashboardVm[] = []

  inseartConsumerper: any
  allconsumers: any
  consumersmonth: any
  updateconsumermonth: any
  updateconsumeryear: any
  updateconsumermonthper: any
  updateconsumeryearper: any

  alltelephone: any
  telephonemonth: any
  telephoneinsertper: any
  updatetelmonth: any
  updatetelyear: any
  updatetelmonthper: any
  updatetelyearper: any

  alladdress: any
  addressmonth: any
  addressinsertper: any
  updateaddmonth: any
  updateaddyear: any
  updateaddmonthper: any
  updateaddyearper: any

  alldeeds: any
  deedsmonth: any
  deedsinsertper: any
  updatedeedsmonth: any
  updatedeedsyear: any
  updatedeedsmonthper: any
  updatedeedsyearper: any

  alldirector: any
  directormonth: any
  directorinsertper: any
  updatedirmonth: any
  updatediryear: any
  updatedirmonthper: any
  updatediryearper: any

  alladverse: any
  adversemonth: any
  adverseinsertper: any
  updateadvmonth: any
  updateadvyear: any
  updateadvmonthper: any
  updateadvyearper: any

  allcompany: any
  companymonth: any
  companyinsertper: any
  updatecommonth: any
  updatecompyear: any
  updatecommonthper: any
  updatecomyearper: any

  allmarried: any
  marriedmonth: any
  marriedinsertper: any
  updatemarriedmonth: any
  updatemarriedyear: any
  updatemarriedmonthper: any
  updatemarriedyearper: any

  alldivorsed: any
  divorsedmonth: any
  divorsedinsertper: any
  updatedivorsedmonth: any
  updatedivorsedyear: any
  updatedivorsedmonthper: any
  updatedivorsedyearper: any

  alldeceased: any
  deceasedmonth: any
  deceasedinsertper: any
  updatedeceasedmonth: any
  updatedeceasedyear: any
  updatedeceasedmonthper: any
  updatedeceasedyearper: any

  loading: boolean = false
  LastPasswordResetDate: string
  userId: string

  constructor(
    public headernavService: headernavService,
    public router: Router,
    public footerService: FooterService,
    public securityService: SecurityService,
    public modalService: NgbModal,
    public dashboardService: DashboardService, // public datepipe: DatePipe,
  ) {
    this.isuserexists = localStorage.getItem('userid')
    var ProductName = sessionStorage.getItem('username1')
    if (this.isuserexists != null && this.isuserexists != 'undefined') {
      this.headernavService.toggle(true)
      this.footerService.updatefooter(true)
      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }
    } else {
      this.router.navigate(['/login'])
    }
  }
  // dateFormat(date: Date) {
  //   return this.datepipe.transform(date, 'yyyy-MM-dd')
  // }
  getDiffDays(sDate, eDate) {
    var startDate = new Date(sDate)
    var endDate = new Date(eDate)

    var Time = endDate.getTime() - startDate.getTime()
    return Time / (1000 * 3600 * 24)
  }

  ngOnInit() {
    var LastResetDate
    this.loading = true
    console.log('customerid:' + localStorage.getItem('customerid'))
    console.log('userid:' + localStorage.getItem('userid'))
    //this.LastPasswordResetDate = localStorage.getItem('LastPasswordResetDate')
    //this.userId = localStorage.getItem('userid')

    //get Reseted date
    this.securityService
      .getLastPasswordResetDateCustomerUser(this.isuserexists)
      .subscribe((result) => {
        LastResetDate = result

        var numOfDays = Math.floor(
          this.getDiffDays(
            new Date(Date.parse(LastResetDate)),
            //'2022/05/07',
            new Date(Date.parse(Date())),
          ),
        )

        console.log(
          Math.floor(
            this.getDiffDays(
              new Date(Date.parse(LastResetDate)),
              // '2022/05/07',
              new Date(Date.parse(Date())),
            ),
          ),
        )

        localStorage.setItem('numOfDays', numOfDays.toString())
        console.log(
          new Date(Date.parse(Date())) +
            ' - ' +
            new Date(Date.parse(LastResetDate)),
        )
        localStorage.setItem(
          'LastPasswordResetDate',
          this.LastPasswordResetDate,
        )

        if (numOfDays > 30) {
          document.getElementById('pwdReset').click()
        }
      })

    // this.resetDate = new Date(Date.parse(this.LastPasswordResetDate))
    // this.todayDate = new Date(Date.parse(Date()))

    this.securityService.getMessages(this.isuserexists).subscribe((result) => {
      this.messages = result
      if (this.messages.length > 0) {
        this.securityService
          .removeAppMessages(this.isuserexists)
          .subscribe((res) => {})
        const modalRef = this.modalService.open(MessagePopupComponent, {
          size: 'lg',
        })
        modalRef.componentInstance.messages = this.messages
      }

      // console.log(
      //   'LastPasswordResetDate: ' +
      //     localStorage.getItem('LastPasswordResetDate'),
      // )
    })

    this.dashboardService.getData().subscribe((response) => {
      this.result = response
      if (this.result.length > 0) {
        this.allconsumers = this.result.find(
          (x) => x.tableName == 'Consumers',
        ).totalCount
        this.consumersmonth = this.result.find(
          (t) => t.tableName == 'Consumers',
        ).insertCount
        this.inseartConsumerper = this.result
          .find((t) => t.tableName == 'Consumers')
          .insertper.toFixed(2)
        this.updateconsumermonth = this.result.find(
          (t) => t.tableName == 'Consumers',
        ).updateCount
        this.updateconsumeryear = this.result.find(
          (t) => t.tableName == 'Consumers',
        ).yearToDateUpdate
        this.updateconsumermonthper = this.result
          .find((t) => t.tableName == 'Consumers')
          .updateper.toFixed(2)
        this.updateconsumeryearper = this.result
          .find((t) => t.tableName == 'Consumers')
          .yeartodateper.toFixed(2)

        if (this.inseartConsumerper != 'NaN')
          $('#inseartConsumerper').css('width', this.inseartConsumerper + '%')
        else {
          this.inseartConsumerper = '0'
          $('#inseartConsumerper').css('width', '0' + '%')
        }

        if (this.updateconsumermonthper != 'NaN')
          $('#updateconsumermonthper').css(
            'width',
            this.updateconsumermonthper + '%',
          )
        else {
          this.updateconsumermonthper = '0'
          $('#updateconsumermonthper').css('width', '0' + '%')
        }

        if (this.updateconsumeryearper != 'NaN')
          $('#updateconsumeryearper').css(
            'width',
            this.updateconsumeryearper + '%',
          )
        else {
          this.updateconsumeryearper = '0'
          $('#updateconsumeryearper').css('width', '0' + '%')
        }

        this.alltelephone = this.result.find(
          (t) => t.tableName == 'Telephone',
        ).totalCount
        this.telephonemonth = this.result.find(
          (r) => r.tableName == 'Telephone',
        ).insertCount
        this.telephoneinsertper = this.result
          .find((r) => r.tableName == 'Telephone')
          .insertper.toFixed(2)
        this.updatetelmonth = this.result.find(
          (r) => r.tableName == 'Telephone',
        ).updateCount
        this.updatetelyear = this.result.find(
          (r) => r.tableName == 'Telephone',
        ).yearToDateUpdate
        this.updatetelmonthper = this.result
          .find((r) => r.tableName == 'Telephone')
          .updateper.toFixed(2)
        this.updatetelyearper = this.result
          .find((r) => r.tableName == 'Telephone')
          .yeartodateper.toFixed(2)

        if (this.telephoneinsertper != 'NaN')
          $('#telephoneinsertper').css('width', this.telephoneinsertper + '%')
        else {
          this.telephoneinsertper = '0'
          $('#telephoneinsertper').css('width', '0' + '%')
        }

        if (this.updatetelmonthper != 'NaN')
          $('#updatetelmonthper').css('width', this.updatetelmonthper + '%')
        else {
          this.updatetelmonthper = '0'
          $('#updatetelmonthper').css('width', '0' + '%')
        }

        if (this.updatetelyearper != 'NaN')
          $('#updatetelyearper').css('width', this.updatetelyearper + '%')
        else {
          this.updatetelyearper = '0'
          $('#updatetelyearper').css('width', '0' + '%')
        }

        this.alladdress = this.result.find(
          (t) => t.tableName == 'Address',
        ).totalCount
        this.addressmonth = this.result.find(
          (r) => r.tableName == 'Address',
        ).insertCount
        this.addressinsertper = this.result
          .find((r) => r.tableName == 'Address')
          .insertper.toFixed(2)
        this.updateaddmonth = this.result.find(
          (r) => r.tableName == 'Address',
        ).updateCount
        this.updateaddyear = this.result.find(
          (r) => r.tableName == 'Address',
        ).yearToDateUpdate
        this.updateaddmonthper = this.result
          .find((r) => r.tableName == 'Address')
          .updateper.toFixed(2)
        this.updateaddyearper = this.result
          .find((r) => r.tableName == 'Address')
          .yeartodateper.toFixed(2)

        if (this.addressinsertper != 'NaN')
          $('#addressinsertper').css('width', this.addressinsertper + '%')
        else {
          this.addressinsertper = '0'
          $('#addressinsertper').css('width', '0' + '%')
        }

        if (this.updateaddmonthper != 'NaN')
          $('#updateaddmonthper').css('width', this.updateaddmonthper + '%')
        else {
          this.updateaddmonthper = '0'
          $('#updateaddmonthper').css('width', '0' + '%')
        }

        if (this.updateaddyearper != 'NaN')
          $('#updateaddyearper').css('width', this.updateaddyearper + '%')
        else {
          this.updateaddyearper = '0'
          $('#updateaddyearper').css('width', '0' + '%')
        }

        this.alldeeds = this.result.find(
          (t) => t.tableName == 'Deeds',
        ).totalCount
        this.deedsmonth = this.result.find(
          (r) => r.tableName == 'Deeds',
        ).insertCount
        this.deedsinsertper = this.result
          .find((r) => r.tableName == 'Deeds')
          .insertper.toFixed(2)
        this.updatedeedsmonth = this.result.find(
          (r) => r.tableName == 'Deeds',
        ).updateCount
        this.updatedeedsyear = this.result.find(
          (r) => r.tableName == 'Deeds',
        ).yearToDateUpdate
        this.updatedeedsmonthper = this.result
          .find((r) => r.tableName == 'Deeds')
          .updateper.toFixed(2)
        this.updatedeedsyearper = this.result
          .find((r) => r.tableName == 'Deeds')
          .yeartodateper.toFixed(2)

        if (this.deedsinsertper != 'NaN')
          $('#deedsinsertper').css('width', this.deedsinsertper + '%')
        else {
          this.deedsinsertper = '0'
          $('#deedsinsertper').css('width', '0' + '%')
        }

        if (this.updatedeedsmonthper != 'NaN')
          $('#updatedeedsmonthper').css('width', this.updatedeedsmonthper + '%')
        else {
          this.updatedeedsmonthper = '0'
          $('#updatedeedsmonthper').css('width', '0' + '%')
        }

        if (this.updatedeedsyearper != 'NaN')
          $('#updatedeedsyearper').css('width', this.updatedeedsyearper + '%')
        else {
          this.updatedeedsyearper = '0'
          $('#updatedeedsyearper').css('width', '0' + '%')
        }

        this.alldirector = this.result.find(
          (t) => t.tableName == 'Directorship',
        ).totalCount
        this.directormonth = this.result.find(
          (r) => r.tableName == 'Directorship',
        ).insertCount
        this.directorinsertper = this.result
          .find((r) => r.tableName == 'Directorship')
          .insertper.toFixed(2)
        this.updatedirmonth = this.result.find(
          (r) => r.tableName == 'Directorship',
        ).updateCount
        this.updatediryear = this.result.find(
          (r) => r.tableName == 'Directorship',
        ).yearToDateUpdate
        this.updatedirmonthper = this.result
          .find((r) => r.tableName == 'Directorship')
          .updateper.toFixed(2)
        this.updatediryearper = this.result
          .find((r) => r.tableName == 'Directorship')
          .yeartodateper.toFixed(2)

        if (this.directorinsertper != 'NaN')
          $('#directorinsertper').css('width', this.directorinsertper + '%')
        else {
          this.directorinsertper = '0'
          $('#directorinsertper').css('width', '0' + '%')
        }

        if (this.updatedirmonthper != 'NaN')
          $('#updatedirmonthper').css('width', this.updatedirmonthper + '%')
        else {
          this.updatedirmonthper = '0'
          $('#updatedirmonthper').css('width', '0' + '%')
        }

        if (this.updatediryearper != 'NaN')
          $('#updatediryearper').css('width', this.updatediryearper + '%')
        else {
          this.updatediryearper = '0'
          $('#updatediryearper').css('width', '0' + '%')
        }

        this.alladverse = this.result.find(
          (t) => t.tableName == 'Adverse Indicators',
        ).totalCount
        this.adversemonth = this.result.find(
          (r) => r.tableName == 'Adverse Indicators',
        ).insertCount
        this.adverseinsertper = this.result
          .find((r) => r.tableName == 'Adverse Indicators')
          .insertper.toFixed(2)
        this.updateadvmonth = this.result.find(
          (r) => r.tableName == 'Adverse Indicators',
        ).updateCount
        this.updateadvyear = this.result.find(
          (r) => r.tableName == 'Adverse Indicators',
        ).yearToDateUpdate
        this.updateadvmonthper = this.result
          .find((r) => r.tableName == 'Adverse Indicators')
          .updateper.toFixed(2)
        this.updateadvyearper = this.result
          .find((r) => r.tableName == 'Adverse Indicators')
          .yeartodateper.toFixed(2)

        if (this.adverseinsertper != 'NaN')
          $('#adverseinsertper').css('width', this.adverseinsertper + '%')
        else {
          this.adverseinsertper = '0'
          $('#adverseinsertper').css('width', '0' + '%')
        }

        if (this.updateadvmonthper != 'NaN')
          $('#updateadvmonthper').css('width', this.updateadvmonthper + '%')
        else {
          this.updateadvmonthper = '0'
          $('#updateadvmonthper').css('width', '0' + '%')
        }

        if (this.updateadvyearper != 'NaN')
          $('#updateadvyearper').css('width', this.updateadvyearper + '%')
        else {
          this.updateadvyearper = '0'
          $('#updateadvyearper').css('width', '0' + '%')
        }

        this.allcompany = this.result.find(
          (t) => t.tableName == 'Company',
        ).totalCount
        this.companymonth = this.result.find(
          (r) => r.tableName == 'Company',
        ).insertCount
        this.companyinsertper = this.result
          .find((r) => r.tableName == 'Company')
          .insertper.toFixed(2)
        this.updatecommonth = this.result.find(
          (r) => r.tableName == 'Company',
        ).updateCount
        this.updatecompyear = this.result.find(
          (r) => r.tableName == 'Company',
        ).yearToDateUpdate
        this.updatecommonthper = this.result
          .find((r) => r.tableName == 'Company')
          .updateper.toFixed(2)
        this.updatecomyearper = this.result
          .find((r) => r.tableName == 'Company')
          .yeartodateper.toFixed(2)

        if (this.companyinsertper != 'NaN')
          $('#companyinsertper').css('width', this.companyinsertper + '%')
        else {
          this.companyinsertper = '0'
          $('#companyinsertper').css('width', '0' + '%')
        }

        if (this.updatecommonthper != 'NaN')
          $('#updatecommonthper').css('width', this.updatecommonthper + '%')
        else {
          this.updatecommonthper = '0'
          $('#updatecommonthper').css('width', '0' + '%')
        }

        if (this.updatecomyearper != 'NaN')
          $('#updatecomyearper').css('width', this.updatecomyearper + '%')
        else {
          this.updatecomyearper = '0'
          $('#updatecomyearper').css('width', '0' + '%')
        }

        this.allmarried = this.result.find(
          (t) => t.tableName == 'Married',
        ).totalCount
        this.marriedmonth = this.result.find(
          (r) => r.tableName == 'Married',
        ).insertCount
        this.marriedinsertper = this.result
          .find((r) => r.tableName == 'Married')
          .insertper.toFixed(2)
        this.updatemarriedmonth = this.result.find(
          (r) => r.tableName == 'Married',
        ).updateCount
        this.updatemarriedyear = this.result.find(
          (r) => r.tableName == 'Married',
        ).yearToDateUpdate
        this.updatemarriedmonthper = this.result
          .find((r) => r.tableName == 'Married')
          .updateper.toFixed(2)
        this.updatemarriedyearper = this.result
          .find((r) => r.tableName == 'Married')
          .yeartodateper.toFixed(2)

        if (this.marriedinsertper != 'NaN')
          $('#marriedinsertper').css('width', this.companyinsertper + '%')
        else {
          this.companyinsertper = '0'
          $('#marriedinsertper').css('width', '0' + '%')
        }

        if (this.updatemarriedmonthper != 'NaN')
          $('#updatemarriedmonthper').css(
            'width',
            this.updatemarriedmonthper + '%',
          )
        else {
          this.updatemarriedmonthper = '0'
          $('#updatemarriedmonthper').css('width', '0' + '%')
        }

        if (this.updatemarriedyearper != 'NaN')
          $('#updatemarriedyearper').css(
            'width',
            this.updatemarriedyearper + '%',
          )
        else {
          this.updatemarriedyearper = '0'
          $('#updatemarriedyearper').css('width', '0' + '%')
        }

        this.alldivorsed = this.result.find(
          (t) => t.tableName == 'Divorse',
        ).totalCount
        this.divorsedmonth = this.result.find(
          (r) => r.tableName == 'Divorse',
        ).insertCount
        this.divorsedinsertper = this.result
          .find((r) => r.tableName == 'Divorse')
          .insertper.toFixed(2)
        this.updatedivorsedmonth = this.result.find(
          (r) => r.tableName == 'Divorse',
        ).updateCount
        this.updatedivorsedyear = this.result.find(
          (r) => r.tableName == 'Divorse',
        ).yearToDateUpdate
        this.updatedivorsedmonthper = this.result
          .find((r) => r.tableName == 'Divorse')
          .updateper.toFixed(2)
        this.updatedivorsedyearper = this.result
          .find((r) => r.tableName == 'Divorse')
          .yeartodateper.toFixed(2)

        if (this.divorsedinsertper != 'NaN')
          $('#divorsedinsertper').css('width', this.divorsedinsertper + '%')
        else {
          this.divorsedinsertper = '0'
          $('#divorsedinsertper').css('width', '0' + '%')
        }

        if (this.updatedivorsedmonthper != 'NaN')
          $('#updatedivorsedmonthper').css(
            'width',
            this.updatedivorsedmonthper + '%',
          )
        else {
          this.updatedivorsedmonthper = '0'
          $('#updatedivorsedmonthper').css('width', '0' + '%')
        }

        if (this.updatedivorsedyearper != 'NaN')
          $('#updatedivorsedyearper').css(
            'width',
            this.updatedivorsedyearper + '%',
          )
        else {
          this.updatedivorsedyearper = '0'
          $('#updatedivorsedyearper').css('width', '0' + '%')
        }

        this.alldeceased = this.result.find(
          (t) => t.tableName == 'Deceased',
        ).totalCount
        this.deceasedmonth = this.result.find(
          (r) => r.tableName == 'Deceased',
        ).insertCount
        this.deceasedinsertper = this.result
          .find((r) => r.tableName == 'Deceased')
          .insertper.toFixed(2)
        this.updatedeceasedmonth = this.result.find(
          (r) => r.tableName == 'Deceased',
        ).updateCount
        this.updatedeceasedyear = this.result.find(
          (r) => r.tableName == 'Deceased',
        ).yearToDateUpdate
        this.updatedeceasedmonthper = this.result
          .find((r) => r.tableName == 'Deceased')
          .updateper.toFixed(2)
        this.updatedeceasedyearper = this.result
          .find((r) => r.tableName == 'Deceased')
          .yeartodateper.toFixed(2)

        if (this.deceasedinsertper != 'NaN')
          $('#deceasedinsertper').css('width', this.deceasedinsertper + '%')
        else {
          this.deceasedinsertper = '0'
          $('#deceasedinsertper').css('width', '0' + '%')
        }

        if (this.updatedeceasedmonthper != 'NaN')
          $('#updatedeceasedmonthper').css(
            'width',
            this.updatedeceasedmonthper + '%',
          )
        else {
          this.updatedeceasedmonthper = '0'
          $('#updatedeceasedmonthper').css('width', '0' + '%')
        }

        if (this.updatedeceasedyearper != 'NaN')
          $('#updatedeceasedyearper').css(
            'width',
            this.updatedeceasedyearper + '%',
          )
        else {
          this.updatedeceasedyearper = '0'
          $('#updatedeceasedyearper').css('width', '0' + '%')
        }
        this.loading = false
      }
      this.loading = false
    })
  }
  getmarried(type: any) {
    if (type == 'y') this.married = 'y'
    else this.married = 'm'
  }
  getdiv(type: any) {
    if (type == 'y') this.divorsed = 'y'
    else this.divorsed = 'm'
  }
  getstatus(status: any) {
    if (status == 'm') {
      this.marritalStatus = 'm'
      this.married = 'm'
    }
    if (status == 'd') {
      this.marritalStatus = 'd'
      this.divorsed = 'm'
      this.getdiv('m')
    }
  }

  resetPasswordRoute() {
    this.router.navigate(['resetpassword'])
  }
}
