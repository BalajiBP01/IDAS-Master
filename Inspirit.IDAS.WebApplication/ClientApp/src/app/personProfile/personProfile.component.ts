import {
  Component,
  Inject,
  OnInit,
  OnDestroy,
  ViewChild,
  ElementRef,
} from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import {
  PersonProfileService,
  PersonProfile,
  ConsumerSearchRequest,
  ProfileRequest,
  TracingService,
  AddressDetail,
} from '../services/services'
import { ActivatedRoute } from '@angular/router'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import { ContactDetailsComponent } from '../contactdetails/contactdetails'
import * as html2canvas from 'html2canvas'
import { headernavService } from '../header-nav/header-nav.service'
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner'
import { debug, isNullOrUndefined } from 'util'
import { EventEmitter } from 'events'
import { moduleDef } from '@angular/core/src/view'
import * as jsPDF from 'jspdf'
import 'jspdf-autotable'

//import jsPDF from 'jspdf';
//import autoTable from 'jspdf-autotable';

import { RelationshipService } from '../relationshipLink/RelationshipService'

//declare var jsPDF: any; // Important

@Component({
  selector: 'personProfile',
  templateUrl: './personProfile.component.html',
  providers: [RelationshipService],
})
export class PersonProfileComponent implements OnInit, OnDestroy {
  res: PersonProfile = new PersonProfile()
  _tracingRequest: ConsumerSearchRequest = new ConsumerSearchRequest()
  _profileRequest: ProfileRequest = new ProfileRequest()
  dtOptions: DataTables.Settings = {}
  addres: AddressDetail

  id: any
  flexGrid: any
  private sub: any
  userid: any
  customerid: any
  type: any
  public loading = false
  userName: any
  idNumber: any
  serchCreteria: any
  points: any
  name: any
  globalSearch: any
  errormsg: any
  isuserExists: any
  rectPageHieght: any

  judgePresent: boolean = false
  debtPresent: boolean = false
  directorPresent: boolean = false
  relationPresent: boolean = false
  addressPresent: boolean = false
  contactPresent: boolean = false
  employmentPresent: boolean = false
  deedsPresent: boolean = false
  timelinePresent: boolean = false
  printconsumer: boolean = false
  // krishna start
  printaka: boolean = false
  isXDS: any
  // krishna end
  profilePresent: boolean = false
  istrailuser: boolean = false
  isSpouse: boolean = false
  isdownload: boolean = false
  company: string
  IsRestrictedCustomerUser: any
  IsRestrictedCustomer: any

  fname: any = ''
  sname: any = ''
  secname: any = ''

  oldtime: any
  newtime: any
  timetaken: any

  id_no: any
  _idnumber: any = ''

  addressList: any = []
  contactList: any = []
  employeesList: any = []
  directorshipList: any = []
  propertyOwnershipList: any = []
  relationshipLinkList: any = []
  debtReviewList: any = []
  judgementList: any = []

  response: any
  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private modalService: NgbModal,
    private spinnerService: Ng4LoadingSpinnerService,
    public personProfile: PersonProfileService,
    public tracingService: TracingService,
    public headernavService: headernavService,
    public relService: RelationshipService,
  ) {
    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end
  }
  ngOnInit() {
    this.company = localStorage.getItem('company')
    this.IsRestrictedCustomer = localStorage.getItem('IsRestrictedCustomer')
    this.IsRestrictedCustomerUser = localStorage.getItem(
      'IsRestrictedCustomerUser',
    )

    // console.log(
    //   'Profile_consumer_IsRestrictedCustomer: ' + this.IsRestrictedCustomer,
    // )
    // console.log(
    //   'Profile_IsRestrictedCustomerUser: ' + this.IsRestrictedCustomerUser,
    // )
    this.loading = true
    this.spinnerService.show()
    this.oldtime = new Date()
    this.isuserExists = localStorage.getItem('userid')

    if (this.isuserExists != null && this.isuserExists != 'undefined') {
      this.headernavService.toggle(true)
      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }

      this.dtOptions = {
        pagingType: 'full_numbers',
        scrollX: true,
        order: [3, 'desc'],
        language: {
          search: 'Filter:',
        },
      }
      //this.loading = true;
      this.userid = localStorage.getItem('userid')
      this.customerid = localStorage.getItem('customerid')
      let usertype = localStorage.getItem('trailuser')
      if (usertype == 'YES') {
        this.istrailuser = true
      } else this.istrailuser = false
      this.relService.idno.subscribe((res = this.id_no) => {
        this._idnumber = res
        if (this._idnumber != '' && this._idnumber != 'undefined') {
          this.Getrelation(this._idnumber)
        }
      })
      if (this._idnumber != 'undefined' || this._idnumber == '') {
        this.sub = this.route.queryParams.subscribe((params) => {
          this._tracingRequest.address = params['address']
          this._tracingRequest.dateOfBirth = params['dateOfBirth']
          this._tracingRequest.firstname = params['firstname']
          this._tracingRequest.fromDate = params['fromDate']
          this._tracingRequest.iDNumber = params['iDNumber']
          this._tracingRequest.phoneNumber = params['phoneNumber']
          this._tracingRequest.surname = params['surname']
          this._tracingRequest.globalSearch = params['globalSearch']
          this._tracingRequest.type = params['type']
          this._tracingRequest.custId = this.customerid

          this.id = params['consumerId']
          if (params['iDNumber'] != 'undefined' && params['iDNumber'] != null) {
            this._profileRequest.searchCriteria = params['iDNumber']
            this._profileRequest.inputType = 'ID Number'
          }
          if (
            params['dateOfBirth'] != 'undefined' &&
            params['dateOfBirth'] != null
          ) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria +
                ' ' +
                params['dateOfBirth']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Date Of Birth'
            } else {
              this._profileRequest.searchCriteria = params['dateOfBirth']
              this._profileRequest.inputType = 'Date Of Birth'
            }
          }
          if (
            params['firstname'] != 'undefined' &&
            params['firstname'] != null
          ) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria + ' ' + params['firstname']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'First Name'
            } else {
              this._profileRequest.searchCriteria = params['firstname']
              this._profileRequest.inputType = 'First Name'
            }
          }
          if (params['fromDate'] != 'undefined' && params['fromDate'] != null) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria + ' ' + params['fromDate']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Date Of Birth Range'
            } else {
              this._profileRequest.searchCriteria = params['fromDate']
              this._profileRequest.inputType = 'Date Of Birth Range'
            }
          }
          if (
            params['phoneNumber'] != 'undefined' &&
            params['phoneNumber'] != null
          ) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria +
                ' ' +
                params['phoneNumber']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Phone Number'
            } else {
              this._profileRequest.searchCriteria = params['phoneNumber']
              this._profileRequest.inputType = 'Phone Number'
            }
          }
          if (params['surname'] != 'undefined' && params['surname'] != null) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria + ' ' + params['surname']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Surname'
            } else {
              this._profileRequest.searchCriteria = params['surname']
              this._profileRequest.inputType = 'Surname'
            }
          }
          if (params['toDate'] != 'undefined' && params['toDate'] != null) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria + ' ' + params['toDate']
            } else {
              this._profileRequest.searchCriteria = params['toDate']
            }
          }
          if (params['address'] != 'undefined' && params['address'] != null) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria + ' ' + params['address']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Address'
            } else {
              this._profileRequest.searchCriteria = params['address']
              this._profileRequest.inputType = 'Address'
            }
          }
          if (
            params['globalSearch'] != 'undefined' &&
            params['globalSearch'] != null
          ) {
            if (
              this._profileRequest.searchCriteria != null &&
              this._profileRequest.searchCriteria != 'undefined'
            ) {
              this._profileRequest.searchCriteria =
                this._profileRequest.searchCriteria +
                ' ' +
                params['globalSearch']
              this._profileRequest.inputType =
                this._profileRequest.inputType + ' ' + 'Global Search'
            } else {
              this._profileRequest.searchCriteria = params['globalSearch']
              this._profileRequest.inputType = 'Global Search'
            }
          }
          this._profileRequest.searchType = params['type']
        })
        this._profileRequest.id = this.id
        this._profileRequest.customerId = this.customerid
        this._profileRequest.userId = this.userid
        this._profileRequest.istrailuser = this.istrailuser
        this.personProfile
          .getProfileDetils(this._profileRequest)
          .subscribe((result) => {
            this.loading = false
            this.spinnerService.hide()
            this.newtime = new Date()
            this.timetaken = (
              (this.newtime.getTime() - this.oldtime.getTime()) /
              1000
            ).toFixed(2)
            console.log(this.timetaken)
            this.res = result
            this.response = this.res
            console.log(this.res)
            console.log(this.response)
            

            if (this.res.errorMessage != '') {
              this.isSpouse = false
              document.getElementById('nodata').click()
            }
            if (
              this.res.consumerjudgements.length > 0 &&
              this.res.tabs.includes('ConsumerJudgement')
            ) {
              this.judgePresent = true
              this.judgementList = this.res.consumerjudgements
            }

            if (
              this.res.timelines.length > 0 &&
              this.res.tabs.includes('ConsumerTimeline')
            ) {
              this.timelinePresent = true
            }

            if (
              this.res.consumerDebtReview.length > 0 &&
              this.res.tabs.includes('ConsumerDebtReview')
            ) {
              this.debtPresent = true
              this.debtReviewList = this.res.consumerDebtReview
            }

            if (
              this.res.relationships.length > 0 &&
              this.res.tabs.includes('ConsumerRelationship')
            ) {
              this.relationPresent = true
              this.relationshipLinkList = this.res.relationships
            }

            if (
              this.res.directorShips.length > 0 &&
              this.res.tabs.includes('ConsumerDirector')
            ) {
              this.directorPresent = true
              this.directorshipList = this.res.directorShips
            }

            if (
              this.res.addresses.length > 0 &&
              this.res.tabs.includes('ConsumerAddress')
            ) {
              this.addressPresent = true
              this.addressList = this.res.addresses
            }

            if (
              this.res.contacts.length > 0 &&
              this.res.tabs.includes('ConsumerTelephone')
            ) {
              this.contactPresent = true
              this.contactList = this.res.contacts
            }

            if (
              this.res.employees.length > 0 &&
              this.res.tabs.includes('ConsumerEmployment')
            ) {
              this.employmentPresent = true
              this.employeesList = this.res.employees
            }

            if (
              this.res.propertyOwners.length > 0 &&
              this.res.tabs.includes('ConsumerProperty')
            ) {
              this.deedsPresent = true
              this.propertyOwnershipList = this.res.propertyOwners
            }

            if (this.res.tabs.includes('ConsumerProfile'))
              if (this.res.tabs.includes('ConsumerProfile')) {
                this.printconsumer = true
                this.profilePresent = true
                // krishna start
                this.printaka = true
                // krishna end
              }

            if (this.res.firstName != 'Unknown') this.fname = this.res.firstName
            if (this.res.firstName != 'Unknown') this.sname = this.res.surname
            if (this.res.secondName != 'Unknown')
              this.secname = this.res.secondName

            this.userName = this.res.firstName + ' ' + this.res.surname
            this.idNumber = this.res.idNumber
            //if (this.isXDS == 'NO') {
              this.tracingService
                .getPoints(this.userid, this.customerid)
                .subscribe((r) => {
                  this.points = r
                  this.headernavService.updatePoints(this.points)
                })
            //}
            //else {
            //  this.points = 100
            //  this.headernavService.updatePoints(this.points)
            //}

          })
      }
    } else {
      // pending check IsXDS
      this.router.navigate(['/login'])
    }
  }

  //Date format
  getDate(date: Date): string {
    if (date != null || date != undefined) {
      var dd = String(date.getDate()).padStart(2, '0')
      var mm = String(date.getMonth() + 1).padStart(2, '0')
      var yyyy = date.getFullYear()

      // return (dd.length !==1 )? dd:`0${dd}` + '-' + (mm.length !==1 )? mm:`0${mm}` + '-' + yyyy
      return dd + '-' + mm + '-' + yyyy
    }
    return ''
  }
  //------------------Profile PDF ----------------------

  //Get height of the Heading
  getHightofHeading(doc: any, rectHieght?: number) {
    this.rectPageHieght = (doc as any).lastAutoTable.finalY + rectHieght
    doc.setFillColor(255, 255, 255)
    doc.rect(14, this.rectPageHieght, 181.5, 7, 'F')
  }

  //Get EmptyAutoTable
  getEmptyAutoTable(doc: any, recthieght: number) {
    if (recthieght >= 285 && recthieght <= 297) {
      doc.autoTable({
        head: [],
        body: [],
        startY: (doc as any).lastAutoTable.finalY + 20,
      })
    } else if (recthieght >= 270 && recthieght < 285) {
      doc.autoTable({
        head: [],
        body: [],
        startY: (doc as any).lastAutoTable.finalY + 30,
      })
    } else if (recthieght >= 260 && recthieght < 270) {
      doc.autoTable({
        head: [],
        body: [],
        startY: (doc as any).lastAutoTable.finalY + 40,
      })
    } else if (recthieght >= 250 && recthieght < 260) {
      doc.autoTable({
        head: [],
        body: [],
        startY: (doc as any).lastAutoTable.finalY + 50,
      })
    }
  }

  //TableHeading
  getTableHeading(
    text: string,
    doc: any,
    rectHieght?: number,
    textHieght?: number,
  ) {
    //let pageNumber = doc.lastAutoTable.finalY + rectHieght
    

    this.rectPageHieght = (doc as any).lastAutoTable.finalY + rectHieght
    let textPageHieght = (doc as any).lastAutoTable.finalY + textHieght

    console.log(this.rectPageHieght)

    // old code
    //this.rectPageHieght = doc.lastAutoTable.finalY + rectHieght
    //let textPageHieght = doc.lastAutoTable.finalY + textHieght
    // new code

    doc.setFillColor(26, 78, 109)
    doc.rect(14, this.rectPageHieght, 181.5, 7, 'F')
    doc.setTextColor(255, 255, 255)
    doc
      .setFont(undefined, 'bold')
      .text(text, 15, textPageHieght)
      .setFont(undefined)
    doc.setFontSize(11)
  }

  //convert to currency
  formatToCurrency(amount) {
    if (amount == null) {
      amount = 0
    }
    return amount.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,')
  }

  //spling UserName to an array
  splitMulti(str, tokens) {
    var tempChar = tokens[0]
    for (var i = 1; i < tokens.length; i++) {
      str = str.split(tokens[i]).join(tempChar)
    }
    str = str.split(tempChar)
    return str
  }
  // krishna pending pdf
  downloadReport() {
    const doc = new jsPDF()
    var pageHeight = doc.internal.pageSize.height

    var today = new Date()
    var date =
      today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear()

    var inspiritlogo = localStorage.getItem('client_logo')

    if (inspiritlogo == 'null' || inspiritlogo == 'undefined' || inspiritlogo == null) {

      inspiritlogo = 'data:image/png;base64,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'
      //  inspiritlogo = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAOIAAABaCAYAAACliYrPAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAEkRJREFUeNrsXX1sHMUVn3Oc78BtCBVSAr117JBCVfnSFolWar2pqgqptD4oBSQabt1SAaWVL1WhIJC8kVqFFtScVVRoaeULhJLw5YuERAtIXkv9gz9aWFdqGwiO9wiIUNHmDkjq+OOuM84bM56b3du9W9sX+/2klX13c3Mzb97vfe3ubKxSqRBEeCRueyBRPPLXUsk+VERpIBpFC4qgPsS0T+TpnxRKAoFEXCS03/fHB8jJ95JIRAQScZGwbe/zesv5mzKT77/DXnZfdFWPhlJBIBEXGJVTH7xcaV3dOnX6I/6WgVJBIBEXMiS96w83xdaf387+H//fKf42hqcIJOKChaQ/HdDIipWPzLw4+a74UWrLtT/C8BSBRFwITH14MktWrd4wE55OjosfxdErIpCICxGS7v6N3nrexjR/XZ44IzdBIiKQiAsAS3xRLr0vf9598a57dRQTAok4j96Q/kkHaGqitBBIxAXyhj7IJO74FRZtEEjERfSGDKxok0GpIZCIC+QNW1atKXh5Rf3ORzFXRCARI/SGmpc3XLmlg13wXfLwihZKD4FEjA6m1wfldVrRh3Dprfc9gbkiAok430RkKNmHsvTPsFeIiuJDIBEbD0tZntfp08QQyKoKUTMde59Hr4iYfyJue6qQXMJyMWp8roFXdD1C1GVVQT0XdKHZxxiaiB1PvZWiR7FCYq/Rvw49lqLlr7Vos97yvT8NeIWoffojr+pLmYAKXdCbcIwGPVwYo9uMY6yLiNNnJrP0iNOD/d9Jj8wyJCJpe/hvYpuUR4iaW8pEVOiC2YRjtOiRgDGyv1YzyrI19Decl7SqMG1X+3IM6xn5HPbPO8/9unjxrnuZQRqQ2nTpj4+m3F3t+SUpAecl+R296XTBeUk/F/Q1tEf8YOQvWXoQOErs9TLNrzNtv//nrFF6+/Gf51Qh6voVLQ/d8ucTS7Jwo9AFqwnHmBHGSJpVX0MT8T8vH7Cmpibb6NFDD52+dpegjgXxYKqT91Vh+mSlsqU4Mb0kPSJd+yzowjXNqgt0THlBXzfS13ZDOafelqWHDUdkpI7hvqbVgKtqTs6xWKvWkvKadaRlzXpSWb2BTK89j5CVa0msJbbz2Hcvm11c/c5Hsy2r1/W2rI+TqY2bqUuMky9esHZimpD0oa9vPojSPbfBCMhSDng5/KY7ZiyKR1zqSAz8yxzd9wN25Ux/UO+pDxwRQ0/mJecUbo6cnly1adWKgaUaoiKQiJGi7cBocqbg4EEonxDVbht4fYZk7gPfZyQ2xQal6ZmoY810uXIYpYxAItYGy/FmCAVeMWjxgZ1XnM0Xjv3sJpYTzincvDM+TTavbf1y+oV3b0QxI2S0NvPgaDye5MQAFGlM7szLbz3pMk+YFglEyZil+WJKyAn8kNZzbxRd81JesGFe0QGPSY6cmSY74qvIBStX3E9fHpxHmemCV593uS2WLtD52MuaiFQgphh61ZusQtLLkaP95ARFYp4oxZVY+h67F5C1zdLvRPkAGK8NoOYQqgZ6E/uPOoX0ttyb91zt6o+8OufcYmmiTNa1xhotFKhkpsE4TeJxjSxtw8LsPHzPjpAc2Qh0QdQph/aTkXQhrfjOjiDGpZExyt8FiBdyJKU1IfXKoh6PqAf0ELWQFJTbhokzoffVqqdAG5O2T0Vo6ZVXCFGv6FKvyMa1L2A/A4nH3nQLN3fY7m2fzemPjxpckU5OlcmmNQ0FIaLcucxSoCyJALksG0eafod5fZPKzm1QZlpEulClU0DOAa8vhFj3RsZY67vxiOa/qDmiKEiNCj6nIOGwcJQUhMyDN2gI259+KykosqYgo98tTyrkKQGTAsFHZjziZJlQpxhluMaUdVBBwmHpUBHaAYvfjGGoLwlDrgXmiCFgCt6xBBa+KvSEBcoKbROg6FaDv5+SCi9ebdyAISprk2s7MGq4u9qL7QddNm775HQ5fnoqMiampLGykN3i4aqHclsCaWeqvfR9o8nyR00KB0dgXnkpD14oZ7FTei8ryH2ERHSXTbMQMS5MzPDK/ZiS0UVgZBiSQspGiVgznj/6i55ie98h7oGCoBNy2dTojbqz/Znj5mSFDH40VRmPSGYiCft5XuUTyjHZ5UGR0hIZ9Yjz7ajmtZ+Oy1TMxV2IgYBMbMmgFaUiWCT5djOdvmCeMFVLIWDi4vm4eAQh1pw4f+tDryj7G91zA1Pk/hD9dm994tiMkXj9ukvy61tizxYnp09ELLfdtUgoKhYo9mHJCFqLvPYqYu1RkXCpopmImA1h6fKKcKa+/PCZ44ZHeKT2jPdcPZvzBURf+5NnK2gvdm+5juaID0cos8NUZvVc72hKOXdvFLl2hEQskepqJRJxoYgYoq0dNrT0QbKOUNUkwa66mTUcNE+cUfTnvrH5UIQyqys/gagj65MnN4NRLiIRFx4jYQQfcY6gByTnx7//Y8MJSYI4if4m4eEG5ZCL0JhFDZssMzQLERfT+iXDEpFh7PbP5aRcq2a+uO1QwWwWZQUSl2oYpMWCg0REMCTaf/liEMVkxCqECbm2PVXQmkhZm1Lhl1tYikT0R81QbeyWy6vutAgQouKepwgkYpREZICbgveE6Ddz6dOR7HwXxVUxOi4zErHZkeqwng1EGLfnUxYJfkojqkd9N0REuDol0SR5OhIRReCpgGEJw0LUoKc0oiBid4OXeskhdR5VAYm4mPArWFhBOxnr2e6EaN8d0djrOukNJ+8zSEQkYjPB9vks0XH3Y4GLK655aTZoiLr9meNR5GfdcDF3PQQWL17fvxwrlUjEJsLr111i1wgpra2Zh8PkY0GJoUc0hQG4jzOoN8yRuTfalghWcsPAFf7viurSQPSItcOymTsUtt7RH4iMhfQ2FqL2h1zQeiB63j6qEA7cJOxFQBPuXJHvdjfQG9ZNRGX64rcOXmhFuc4KM12TjLc+aBz77U+cgP2ZxPvexRL1xG4ExoN58154zW4fGoQtMRxBYZjnTSrGwtqZ5/peNosAFlGIN7D3AvFEeSfoezvD3CKFHpGFp9/+JBNirbu+mSK/pn9vb80wsHBzh+qi6qB5aWDA7U89UmjNt29Iw9GlICGba1K82RYRWOZMV3bLtQSQcxepvWVJZB4xiNIGgePxf1AMRxjmcS82FKBdn77LMiuTZ6zCwb05v4JIbGribrJy9WqPYklUisFv+DWJz+ZRAHZtbDbCHdCKEelCVP0sSN/s1jOh8qyKegok5HlZ3HJfQNuB0Xx5cqJ7xakSiZ3+gB4fEjJ+ipTpQSYnCJmaJGV6kIkzhBKRlCfGC9OTE3n6Xv7EC7+bVe4Lv7pLL1fKVvzyK9KxK7/JtuWnko6Rlhj7SwpHr0/UVaihiy8uFrtxVpWf6BAeafDXAeWxcYWjBZAxKcm6rq0rMUecC5ME35eGhyQsR+u96KpbSKVSJpXy9OyH48f+QdZSItZK7uchdHJxKRckTC1GlWZgjihg7DvtYS/i9sX46Y9I67/HxLdGqDfMoaQRSMRahZaey/KKZLxuTLz9hvgSz9chkIhBceyHV7Jiyv4o+opNzG7a1n/0hgTmaQgkYhiM3vU1FqLuabSfynkbWeHnOFn8ndIQSMRzNBm3vsXIcw0Jt1HULNas20BWJj49MfV3+0ujN+p49QoCiVg3Ge+/meWMrEQd6tmGa+ObyPrPf+W/lRPHrnD7ewsoSYQf8PRFkJwxe7tL/6S23vogIyQLWQ3ic+J8w2e+UFqpXfjK0b7rr0LpIZCIURPy7HWms5XPS67dLT+/kcFxzz7kFIFAIi4Ejj+3Dy+YRmCOiEAgEREIRGTAi74RCPSICAQCiYhAIBERCAQSEYFAIiIQiHkjYofeZjS4DfySB5VPkh0oCcS8EJGRkJzdfCmFYvVFjszT8+HZHiqwDohzTCZRekR+zSVe9uUPdrG4O48kH6png9sljCzIpKl3R4jyWlMebtWtZPw5DmyLwCVqnfV5lpEDBtFG/s2RCZN7frkQUQcFacTaW6CkuSWqFHoEUcMAOXtvZJWMVNsrLnewPUjnKxVYdCIKe2cyL+jAnpns9YhP/si/k5f3fYTiBbPkbHtCl8f08l6c0E783XydY0/yPpj3kJ/9wOfHfh9+0wADYXs9JwLa8fHZHvuI8qihKM9dMVcuLzbPopTn8Ndz9tCE91yVMRT24OTzdhTz1mCufA5OSNlyWSn7V+hDEdq5Hn0xHXCgvcENjWru8rpJv6Xca1Qhk7plJ/2e4aMDSoS+1hSePCTvcHyYLwD9cUMSZo5U30TbD9vF83Yuqd6qfIS2SQqCYFYt7dUmSNIetA+YYx/MS3yWIbvTPikuAiw+m2OX1C971Jmp6pe+HxPes+G7baISwPtJUKyitLkwx7Akb9amauNhyBlz0pqVQLkdIeQdkPrfDR4liGxtaZ1HiPSAGx9Z9cihNsyfRxAJIGVM0BdmvDRpDC6QTpdkcpi+l1LIJCvpHX8eSL6G7JgepCQjmITwN1GPfrbUQcI+sAo7mfKQs1sPdsNAvSxoD7Tl+7+Y0ucm+XgLw93Qt6kI69hmTjvgYNuod4asiAXtQxc82B4Yz2EQsunRrzhHtlBpxSO7DFK9/01e+ExcfKasWUGR2Rj6JRllJEs8x9sKfQ2Ckl4jrFlRYaRG4PMdMIegjxyzgISs343COHXpN/i2I7vhN3rgd7IKWSUFpe6B9hyMpHFp3bhzsKTfrEoFgDSD8FKUiRytGILs+Bj6YVwZhQw0gRf7IXIJJMPWECTUgIQjohUGISbB0xSl+Nzh4Ri0KcKkOqV2tnBuzZFdOiijIYdsCsvqlyuo+rA9+uAKlOHWkX6HgMHRpH5d3l6YowOLpUtKoCmMlS0QMSdU+kpibgMy4nLPKUIjr6p1VvB+ReG9rPTdOF8/WDe9nkId/IaqQmlysgpe1gGZ9UIf4rrHYdxJxVxt0LeZEFB4DsWw5Fm9Cog5QSauh0zEdimhXQaMW1oyypogP5eE3Kg6TI6YEgYsw5WUSrQ8Fql+VLVqMyVNlRcKRsAi/o86C2JIgvbBlLDgkYO6irzEUoS8RJGXdBJpEyrIgUqCkTCBxLsVCpgUjErNqjXIPwGpQNHHSLG8fASM0hj9vwBkD1r8ycJ3B8FgsTla0vy5Yu6jbfYFyDUJ1BNU47alKKLKG0oG1ZXWqxNSB7dGLYF75DGYlx9scFRDsJ55kIEbNRF1D+sifibnA6+BRdkjCC/v0YcqbBPDty4IC2z4HSb87hAJsWcfCi+SINUPG/Wavw3txTlmZc8pnLpwPMaWFkhd8MjNNOL9ZCNV1VpThaseZEyCpTfA6LKHnxaD5IjMYNG2bfA9A2RqSDLQwQB7VcRV487VMhySN7QDVKn99Fj13WGiPh0kR38WPJXLAKOThnmkoiZiUSCMLVmvlMIDpITwLifE3H65pONhHbsg4c5IVaxA+43W6INIBYWkh/JWLargvfYIFb0kWNzhEIaMh1o82e/xmEpXDSIWaoSOs3OUCg0aFD/Y7+dBoYaC5ohCf1lIVVQhP09TspK8TcU5USOAAeHFobyHN/SKIIpyTs4NpWTEikK4LeedhhwtCTJwQAaVEDl2KCLmYLJ9QuKdEkK9gkfOYkCVSxfC2qKHImlglXXBEvN+ksIVIxYobNDn3qn6yHj0oanC7BphoQFGRhPm6NYKHRWhVifk4Dk/S83L/4p8yFHk3mxduoFcOZg38yQzlWtYSwdkb8H4LA8ZeBbw6F9+bnPW6Cnm2AU5XVbwFjNeTfK8mkdoL/fXC33u94iMqiIISAWG4Xfzgh53c5kI7UYgUuH6b/D8F4jnwv85aNcP/WV8op/GqqZCsaMEAhgCElpAQlVCXABLPwQKmq8ROsahSmWJyiRULAfhsEOQkPfRL/XhEPV5Ty9rrCna52EcXTDHQSGccgMWU3g4OSIYCC8MwxyGFMUAr0gjBX13w/i4V80K62rB9/dBGx1OKQQJ+7NS/33QvymHbrAGnXCaZB+QtkcR/iZ9vPtsOCxVLL0iCOJROBqGMQ/A32FFKMxlx3W4D9bVkLxnVuJFN1RNg+bY9e1Zw/OdIImoeGI2YL86UZ9s1ol0srbOgk0y6od2Njo24RyYI1WkveSphf2tIHOvt28p1Hf8CkNCiuI2eBVWFOsWaMxB9cZPf+eFiIjIFSIL1nQnPtl3eQI3GF5ca5yBkLUXCklIQiQiYoGRhJyDqHIqxPIChqYIRBMA96xBIJoA/xdgAIquIowpEdrZAAAAAElFTkSuQmCC'
      //  inspiritlogo = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAABPCAYAAAA5vC0kAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAC9VJREFUeNrsXV9sW1cZP9d21n+ivaGTGGXUN2k7bQhWRxraG7ZfOsFLHaFSlaLG7mCiAyk2FPYy5BjYEAjJjgStGGi2JzSoeIjDHtBeyK3ENO1hqishjfVPctOOgdZCb1v617Ev53O+kxyfXDvXW5zZyfeTXDvXx/d85zu/8/0595xTxggEAoFAIBAIBAKBQCAQCAQCYSWx4/lTIdLC6sLXq4IP/uqtWOW9dyeoC4kwy2LPz/+ia9VKvvLPC8aD+46QlSHCLINqNV27dV13KveYpvki1I1EmKbY9fyrIadWTVZv3ZhvgN8/St1IhGmOWjULb2BdANzCGDsOpMgtEWFcAt3UiRhzanUXNGdfWbiu+f0j1JVEmKVwWNb1ur8vHvz2L3TqTiLMonVJ/jrO3wy37zSm6Zw0SepOIoyMRrfj99uNfwdGgz98mawMEaZuXSCobUift+wZGudvZcnM6JrPR1aGCONiXerhjBbUNC3R4Jp8/tFdL/yZrEwHEZD/2HVqFpQNsYJ98WCw0EVyLk2bNc2wp/5Y3rH/WIb/lRZWxunbAJ9T3aZorlvQK+i3wHVr92pdDRbGufZBlt34L7zygyfeineRviNLCeOrk+j9yZNjnDyLrimwIRnM/8PoKpcKuuQ6Bd3WdbxaddlX8x21MHf/VtJZrYpzG4HuNu2aphsvnY1Yz+w1tcAD4JrOLHzl94Oiot0i6r03X9Od6hwOUb++inV11iX97/J5MOXChHWTS7LcU2ofuB/zciFdDn7vZI5/rge9G/2+yLde/3fst089VOoG4W9eOge63ItuIrUKdelYX8azGwsaY6LfL85aVtNx2u1BlvGbsu57543kfJyiMW3TFqZt0Znzie2sulmHUZSxnn5sLHj8Jd23YfMZbfvDRu3Bh9nj/Zvsz2zwD5zc95DNCF4I4+DHKCeM2bNZkuPvA5+fkyyfinTw5Xcis798xuauaGH0XqnU9E0+LU9UWEdptfHKhThkQ9O579itTbk2Ecy/G5r+8aGS4w/U3dDVSo3HxVrsB3/9IE7d3Km0OmgYmFYzbpbGPJixOMYWJpgx/ncE50xi6EcBYN6K/Pv2YyJtcf6Fk6YwmDwx4poxzdc1ESyeH2J3bqawjP7e/Sr7/JY+TxmT6sOltkSk+KmMr3FeptymyfcUI7j1A/6dlvQKA2io2X281CXXoWAE2y5gyi4qoA7qhTkNxsY86EEodC+vZL8IOl1S4gh8zyse9pwe/mHGcKq1iDNPOJEZiWxId5uX4f9OOZu3RpmvXm7i/XtV9sTWB7xWKdpd5rJmsXPc5oPgFedlYACkeJvsNu9vYhDf0riK8rwesJhTSpv1ZUjnpS65r2W4kchcaZcUQ7KA8qCzhvhrgL+G2eL0fQwtUjv35EFMbcFCTGePWcwfSLT4DXRm1vrGrhK3Trm5msNuctfUJvISWQqYnvdjmxJSe6AtU7xNnZ5+EGQxsf7oCk0ZWJhFZZRsqqBcN5u6pI8IIMuAMuLAtEOFM9joUa/puqZpYQjbNcdpcCkXf3q4tOfF1wqO+0gANxY3fn/x7PmvBVOfKl0O2ffbJoyOpEgobke4owJvUx4JE8JR2slUGeTJeAkR2gFaqDHJRQlrU1ytLGnYzTzjtYJkATzOyy3GKgPjbzTGIf4A7yCt3GKuIDv46nTkC5v7hiuOU/4QxE+0ilH4dwlp5CU7bGXMlSZLN2RJVitWcpxu52aP/OkSJ4imK/52Aeef+7LttHZNgInX/3OHndzX9uRdwWNAm1niPjuD8bWYVlseRm1bGXXjZExtSWY0/d0ny47ma5Vq69ytfZh5mEmPJt1sKu/KorwWCdNZ1Kpht8uz3/wcTOiVWrim2O5TVqST7gLfw52qYLkUnAjjTpjI7p+UXOMER6tnDi0smJbuoGSRbrQCRJh5t+QaJ8wefczW5tPNpsHz7npM5Bn7vRTiga4cwM8SYT5eLDHDzlylqaWwjj5aclqk65r77HAzxD1mPbI8JSLMx4hzB3ZywjgqaYzB479rtWY31cI1tWNhgCwtJ+RwAjImZVXWGuKE3pMuyXFYccnFajU7+Gwu7lb+UvwRm/uflZhAA9KBu5nhxEjiMxdBFHjEASdG5KXYJbXGjMioGCzY3kiPEMYpuF+v5Y2nfzbmGs8c2V1gylS2x7Rfhpj+B6VlkTgOrheZkiwL1BNt41lStyMnBfLXpPa2JIyNijDbmCMwPWQJ7d6XTR8asJqRhtMmvfPwj8589qvfX2JtKjN/LyqWyjp/YGehHQvDSSCeG7nJC/EKzAS3SxbRfi+/aVtfH6EuAZiILLjIYSnxYPfCeOWCzu7dntGuX9F9N64y56bNnLu3mHP/Lqvdvc2cO7fZ3N3bdrVaMWu16tlabS7I32PbDh7X/Vs/yXw+DdKkzIWDwTEPWY+nFWfrHV2dVltHdtts7v5yjwB0dBNpjWlxThH93rm3561LfXQ4OermdTQPYx17osSqc4l2flP517TwR6kLBw1a07ueCAOYee6pgtMGaQLbdwBbChcPDZSoi9chYepB8AuHC6xWjS6X8fi3f5ptfPxLhemvDyaoe1cegV4Sdnr8WQhGB4JHX4zzAAUe+BlS2mf3PfrF8sYnv1K05tNrQgegkQooS1qTLolALqnbIFbQWaQKAoFAIBB6PnPQ1cfbPdiGyCpsNqMsCQHrQqZ6vI1TrI39UArZYLdmsgcHeb5Tg2Q5whjM4yN2WOPa5lbYVVEevHuZV0FLpMoP63t7zToZOEA6InfAQ+VeH97BE2OYfS10kfJCbcgPrnevLD/ucOwp4Ca8oU7dXz3uA0ZYli0eKQFzEpPSaJU3qoNg9UVE/LuFlVkwY8qvafhZ7EFmeK+hZouO8IiKUanuqNiBiPcXnQpY2GuMMVaeLa7blfchGyinKAf36UeZ4W9YbjmA727yX2PzW4DNVjLiaQ/CdZWanVKh6FfWny7LwObXCScUuaF8CGTDZaK2VAa+h92nCZQxDAu8hOXHOt3uLbfHwrYKfcltyvHrqQaXJCk+gwpL4Qi1kLk2kqcfX+LoC4bCgVJTQtmI09gh/dKIb4YyEkrDzzFl9Nt4nxRr3DhmKzKHFQtpSW7Jlu4LCoFzXmxJ/oQivy4RLo7KHcYyMNFn4ZpfUGwUR3arnZNZ/B20oygNngmUsx/1FXNJNoqSbKclgkdQr2LicRtr3Jos3zsqGQDRnqik81Hpnkn8zbB8PzmGGUH25VDBBckyCAsDnQEnMVxrYv7LSvwQxtFxjS2/cr8eYOMznYgah7DFs1hsF7c5gr9TjzQFFyPvGQJShKWDkHJK/ZYygJjUqUI/Jl7PSfKUsWPSrPVyVQtlnJD0amB7Q3g97+Iqy6JfhBUD+dF6pJH4luqGsQ1wlkwCBwZs7C9J7YFyWbRQIamPLPxuBsuZboTR1SBQGvkMG6LjKBpyCSZ11jitLspHcdQw1mTaHTOREWn0WpKQwopZalyFygYlT+Lv1LPwdKUDxchMo1VqVVZ3Cfivu8QMYg3wMFtcON4svhC6syRiLLhS6SWfQ7NNlQN1YWE7QgrxDeZ9J6ap1JsS9+evfpQjJLmmhhgGTBWkY0KJcWWE6VIHNttUBlsUJpFIS8q32L8jyhp4TonRIlMDq3FW+Z2N/jjJGk9ViCh/m9ihljxipV2MsK1EHNEVcnGZ0D74HAS5wJVhnBZC/enNskrJBUxieWGpTNT5KBI6jPKVJP2dbtLZcRfiy4kK6FvHmOesNCjFWTdxtMDbUHcDUkxk4290eRD6JNYWJEbp+FnOEsRGMah0nC3dnppTlCyXL7LWZ8bmUAFCoRnJeln4e5nYppQRpLCObfhZ7rCMMtos6bqaWeSU+MeUy2HQl5HKiP1I4j/JCGOc0WyfUkkqZyr6EwvDwtg++R7FJplnUQnwhfvOiMAVB6g4rSrMpLP5lPboGMuo+gljKLA+10WDFeKvGZqv7dw8zFohSghjhlAn5yiIMGsHFpp5a43tgyYQCAQCgUAgEAgEAoFAIBAIBAKBQOgk/i/AAIMd8sJDGqlfAAAAAElFTkSuQmCC'

    }
    else {
      inspiritlogo = localStorage.getItem('client_logo')
    }

    doc.setFont('Avenir')
    var dislaimer = `Inspirit Data Analytics Services(Pty) Ltd, an authorized agent of XDS, shall not be liable for any damage or loss, both directly or indirectly, as a result\nof the use of or the omission to use the statement made in response to the enquiry made herein or for any consequential / inconsequential damages, loss\nof profit or special damages arising out of the issuing of that statement or any use thereof. Copyright 2022 Inspirit Data Analytics Services(Pty) Ltd \n(Reg No: 2017653373) Powered by Xpert Decision Systems(XDS).`

    var addressHead = [['ADDRESS', 'PROVINCE', 'UPDATED DATE', 'CREATED DATE']]
    var contactHead = [
      ['TYPE', 'CONTACT', 'PEOPLE LINKED', 'UPDATED DATE', 'CREATED DATE'],
    ]
    var employeesHead = [
      ['COMMERCIAL NAME', 'DESIGNATION', 'UPDATED DATE', 'CREATED DATE'],
    ]
    var directorHead = [
      ['COMMERCIAL NAME', 'STATUS', 'APPOINTMENT DATE', 'CREATED DATE'],
    ]
    var propertyHead = [
      [
        'TYPE',
        'PURCHASE DATE',
        'CURRENT OWNER',
        'ADDRESS',
        'TOWNSHIP',
        'PURCHASE AMOUNT',
      ],
    ]
    var relationshipHead = [
      ['TYPE', 'LINK VALUE', 'DATE OF BIRTH', 'FULL NAME'],
    ]
    var debtRevieHead = [
      [
        'DEBT COUNSELLOR REGISTRATION NUMBER',
        'DC FIRST NAME',
        'DC LAST NAME ',
        'STATUS CODE',
        'CREATED DATE',
      ],
    ]
    var judgementHead = [
      [
        'CASE NUMBER',
        'CASE FILLING',
        'CASE TYPE',
        'CASE REASON',
        'PLAINTTIFF NAME',
        'CREATED DATE',
      ],
    ]

    var addressData = []
    var contactData = []
    var employeesData = []
    var directorData = []
    var propertyData = []
    var relationshipData = []
    var debtRevieData = []
    var judgementData = []

    var UserName1 = localStorage.getItem('name').toUpperCase()
    var UserName = this.splitMulti(localStorage.getItem('name').toUpperCase(), [
      ' ',
      '  ',
    ])

    // doc.addImage(inspiritlogo, 'png', 4, 10, 64, 25) //( w,h)
    doc.addImage(inspiritlogo, 'png', 15, 10, 40, 15)
    doc.setFontSize(11)
    //doc.setFont('Avenir-Regular', 'normal')
    doc.setFont('Avenir')
    //doc.text(
    //  this.company.substring(0,3),
    //  163,
    //  20,
    //)
    //doc.text(
    //  'Brandfin House\n4 Holwood Crescent\nLa Lucia Ridge\nUmhlanga\n4319',
    //  163,
    //  20,
    //)

    //Subscriber boarder
    //User fullname
    doc.setFillColor(217, 217, 217)
    doc.rect(14, 45, 181.5, 25, 'F')
    doc.setTextColor(0, 0, 0)
    doc.setFontSize(10)
    doc
      .setFont(undefined, 'bold')
      .text(
        'USER\t\t\t    : ' +
        UserName[0].charAt(0) +
        ' ' +
        UserName[1].charAt(0),
        18,
        51,
      )
      .setFont(undefined)
    doc.setFontSize(10)

    doc
      .setFont(undefined, 'bold')
      .text('COMPANY\t\t  : ' + this.company.toUpperCase(), 18, 58)
      .setFont(undefined)
    doc.setFontSize(10)

    doc
      .setFont(undefined, 'bold')
      .text('ENQUIRY DATE\t: ' + date.toString(), 18, 65)
      .setFont(undefined)
    doc.setFontSize(10)

    //Name
    //rgb(26, 78, 109)
    doc.setFillColor(26, 78, 109)
    doc.rect(14, 75, 181.5, 7, 'F')
    doc.setTextColor(255, 255, 255)
    const secondName = this.secname !== null ? this.secname : ''
    doc
      .setFont(undefined, 'bold')
      //.text('NAME: ' + this.res.firstName + ' ' + this.res.surname, 15, 80)
      .text(
        'NAME: ' +
        this.sname +
        ' ' +
        this.fname +
        ' ' +
        secondName +
        ', ' +
        this.res.idNumber,
        15,
        80,
      )
      .setFont(undefined)
    doc.setFontSize(11)

    // important dummy col

    var col = ["", ""];


    //Personal details

    var perDetData = [];

    var perDet = [
      { header: 'ID NUMBER:', body: this.res.idNumber },
      { header: 'PASSPORT NUMBER:', body: this.res.passportNo },
      { header: 'BIRTH DATE:', body: this.getDate(this.res.birthDate) },
      { header: 'GENDER:', body: this.res.genderInd },
      { header: 'ID ISSUED DATE:', body: this.getDate(this.res.iDIssuedDate) },
      { header: 'TITLE CODE:', body: this.res.titleCode }
    ]


    perDet.forEach((element, index) => {
      var temp = [
        element.header,
        element.body
      ]
      perDetData.push(temp)

    })

    doc.autoTable(col, perDetData, {
      startY: 85,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } }
    })


    // 2nd personal details
    var perDetAddData = [];
    var perDetAdd = [
      { header: 'MAIDEN NAME:', body: this.res.maidenName },
      { header: 'INITIAL:', body: this.res.firstInitial },
      { header: 'LSM:', body: this.res.lsm },
      { header: 'CONTACT SCORE:', body: this.res.contactScore },
      { header: 'RISK SCORE:', body: this.res.riskScore }
    ]

    perDetAdd.forEach((element, index) => {
      var temp = [
        element.header,
        element.body
      ]
      perDetAddData.push(temp)
    })


    doc.autoTable(col, perDetAddData, {
      startY: 85,
      margin: { left: 120 },
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } }
    })


    // PAGE NUMBERING
    const pageCount = doc.internal.getNumberOfPages()
    for (var i = 1; i <= pageCount; i++) {
      doc.setTextColor(0, 0, 0)
      doc.setPage(i)
      doc.setFontSize(9)
      doc.text(
        'Page ' + String(i) + ' of ' + String(pageCount),
        210 - 15,
        297 - 10,
        null,
        null,
        'right',
      )
    }

    doc.setFontSize(8)
    doc.setTextColor(0, 0, 0)
    doc.text(dislaimer, 105, 270, 'center')

    doc.save('ProfileReport.pdf')
  }

  downloadReportMain() {
    const doc = new jsPDF()
    var pageHeight = doc.internal.pageSize.height

    var today = new Date()
    var date =
      today.getDate() + '-' + (today.getMonth() + 1) + '-' + today.getFullYear()

    var inspiritlogo = localStorage.getItem('client_logo')

    if (inspiritlogo == 'null' || inspiritlogo == 'undefined' || inspiritlogo == null) {

      inspiritlogo = 'data:image/png;base64,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'
    //  inspiritlogo = 'data:image/png;base64,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'
    //  inspiritlogo = 'data:image/png;base64,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'

    }
    else {
      inspiritlogo = localStorage.getItem('client_logo')
    }
    
    doc.setFont('Avenir')
    var dislaimer = `Inspirit Data Analytics Services(Pty) Ltd, an authorized agent of XDS, shall not be liable for any damage or loss, both directly or indirectly, as a result\nof the use of or the omission to use the statement made in response to the enquiry made herein or for any consequential / inconsequential damages, loss\nof profit or special damages arising out of the issuing of that statement or any use thereof. Copyright 2022 Inspirit Data Analytics Services(Pty) Ltd \n(Reg No: 2017653373) Powered by Xpert Decision Systems(XDS).`

    var addressHead = [['ADDRESS', 'PROVINCE', 'UPDATED DATE', 'CREATED DATE']]
    var contactHead = [
      ['TYPE', 'CONTACT', 'PEOPLE LINKED', 'UPDATED DATE', 'CREATED DATE'],
    ]
    var employeesHead = [
      ['COMMERCIAL NAME', 'DESIGNATION', 'UPDATED DATE', 'CREATED DATE'],
    ]
    var directorHead = [
      ['COMMERCIAL NAME', 'STATUS', 'APPOINTMENT DATE', 'CREATED DATE'],
    ]
    var propertyHead = [
      [
        'TYPE',
        'PURCHASE DATE',
        'CURRENT OWNER',
        'ADDRESS',
        'TOWNSHIP',
        'PURCHASE AMOUNT',
      ],
    ]
    var relationshipHead = [
      ['TYPE', 'LINK VALUE', 'DATE OF BIRTH', 'FULL NAME'],
    ]
    var debtRevieHead = [
      [
        'DEBT COUNSELLOR REGISTRATION NUMBER',
        'DC FIRST NAME',
        'DC LAST NAME ',
        'STATUS CODE',
        'CREATED DATE',
      ],
    ]
    var judgementHead = [
      [
        'CASE NUMBER',
        'CASE FILLING',
        'CASE TYPE',
        'CASE REASON',
        'PLAINTTIFF NAME',
        'CREATED DATE',
      ],
    ]

    var addressData = []
    var contactData = []
    var employeesData = []
    var directorData = []
    var propertyData = []
    var relationshipData = []
    var debtRevieData = []
    var judgementData = []

    var UserName1 = localStorage.getItem('name').toUpperCase()
    var UserName = this.splitMulti(localStorage.getItem('name').toUpperCase(), [
      ' ',
      '  ',
    ])

   // doc.addImage(insp=-------------------------------------------------=-=-=-=--=-----------------------=---iritlogo, 'png', 4, 10, 64, 25) //( w,h)
    doc.addImage(inspiritlogo, 'png', 15, 10, 40, 15) 
    doc.setFontSize(11)
    //doc.setFont('Avenir-Regular', 'normal')
    doc.setFont('Avenir')
    //doc.text(
    //  this.company.substring(0,3),
    //  163,
    //  20,
    //)
    //doc.text(
    //  'Brandfin House\n4 Holwood Crescent\nLa Lucia Ridge\nUmhlanga\n4319',
    //  163,
    //  20,
    //)

    //Subscriber boarder
    //User fullname
    doc.setFillColor(217, 217, 217)
    doc.rect(14, 45, 181.5, 25, 'F')
    doc.setTextColor(0, 0, 0)
    doc.setFontSize(10)
    doc
      .setFont(undefined, 'bold')
      .text(
        'USER\t\t\t    : ' +
          UserName[0].charAt(0) +
          ' ' +
          UserName[1].charAt(0),
        18,
        51,
      )
      .setFont(undefined)
    doc.setFontSize(10)

    doc
      .setFont(undefined, 'bold')
      .text('COMPANY\t\t  : ' + this.company.toUpperCase(), 18, 58)
      .setFont(undefined)
    doc.setFontSize(10)

    doc
      .setFont(undefined, 'bold')
      .text('ENQUIRY DATE\t: ' + date.toString(), 18, 65)
      .setFont(undefined)
    doc.setFontSize(10)

    //Name
    //rgb(26, 78, 109)
    doc.setFillColor(26, 78, 109)
    doc.rect(14, 75, 181.5, 7, 'F')
    doc.setTextColor(255, 255, 255)
    const secondName = this.secname !== null ? this.secname : ''
    doc
      .setFont(undefined, 'bold')
      //.text('NAME: ' + this.res.firstName + ' ' + this.res.surname, 15, 80)
      .text(
        'NAME: ' +
          this.sname +
          ' ' +
          this.fname +
          ' ' +
          secondName +
          ', ' +
          this.res.idNumber,
        15,
        80,
      )
      .setFont(undefined)
    doc.setFontSize(11)

    //Personal details
    doc.autoTable({
      startY: 85,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      body: [
        { header: 'ID NUMBER:', body: this.res.idNumber },
        { header: 'PASSPORT NUMBER:', body: this.res.passportNo },
        { header: 'BIRTH DATE:', body: this.getDate(this.res.birthDate) },
        { header: 'GENDER:', body: this.res.genderInd },
        {
          header: 'ID ISSUED DATE:',
          body: this.getDate(this.res.iDIssuedDate),
        },
        { header: 'TITLE CODE:', body: this.res.titleCode },
      ],
      columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } },
    })

    //Personal details
    doc.autoTable({
      startY: 85,
      margin: { left: 120 },
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      body: [
        { header: 'MAIDEN NAME:', body: this.res.maidenName },
        { header: 'INITIAL:', body: this.res.firstInitial },
        { header: 'LSM:', body: this.res.lsm },
        { header: 'CONTACT SCORE:', body: this.res.contactScore },
        { header: 'RISK SCORE:', body: this.res.riskScore },
      ],
      columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } },
    })

    //MARITAL INFORMATION
    if (this.res.marriageDate != null || this.res.divorceDate != null) {
      this.getTableHeading('MARITAL INFORMATION', doc, 10, 15)
      let spouseDOB = this.res.spouseIdnoOrDOB
      if (spouseDOB != null) {
        spouseDOB =
          this.res.spouseIdnoOrDOB.substring(0, 2) +
          '-' +
          this.res.spouseIdnoOrDOB.substring(4, 2) +
          '-' +
          this.res.spouseIdnoOrDOB.substring(4, 6)
      }

      doc.autoTable({
        startY: 140,
        styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
        body: [
          { header: 'MARITAL STATUS:', body: this.res.maritalStatus },
          {
            header: 'MARRIAGE DATE:',
            body: this.getDate(this.res.marriageDate),
          },
          { header: 'PLACE OF MARRIAGE:', body: this.res.placeOfMarriage },
          { header: 'SPOUSE DATE OF BIRTH:', body: spouseDOB },
          { header: 'SPOUSE SURNAME:', body: this.res.spouseSurName },
        ],
        columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } },
      })

      doc.autoTable({
        margin: { left: 120 },
        startY: 140,
        styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
        body: [
          { header: 'SPOUSE FORENAMES:', body: this.res.spouseForeNames },
          { header: 'NAME COMBO:', body: this.res.nameCombo },
          { header: 'DIVORCE DATE:', body: this.getDate(this.res.divorceDate) },
          {
            header: 'DIVORCE ISSUED BY COURT:',
            body: this.res.divorceIssuedCourt,
          },
        ],
        columnStyles: { 0: { cellWidth: 40, fontStyle: 'bold' } },
      })
    }
    //DECEASE INFORMATION
    this.getTableHeading('DECEASED INFORMATION', doc, 10, 15)
    doc.autoTable({
      head: [['DECEASED STATUS', 'DECEASED DATE', 'PLACE OF DEATH']],
      body: [
        [
          this.res.deceasedStatus,
          this.getDate(this.res.deceasedDate),
          this.res.placeOfDeath,
        ],
      ],
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      headStyles: { fillColor: [230, 230, 230] },
    })

    //ADDRESS
    if (this.addressList.length > 0) {
      this.addressList.forEach((element, index) => {
        if (index < 15) {
          var createdOnDate =
            element.createdOnDate != undefined
              ? this.getDate(element.createdOnDate)
              : null
          var lastUpdatedDate =
            element.lastUpdatedDate != undefined
              ? this.getDate(element.lastUpdatedDate)
              : null
          var temp = [
            element.fullAddress,
            element.province,
            lastUpdatedDate,
            createdOnDate,
          ]
          addressData.push(temp)
        }
      })
    } else {
      var nonDataList = ['No records found']
      this.addressList.push(nonDataList)
      addressData.push(nonDataList)
      this.addressList = []
    }
    this.getTableHeading('ADDRESSES', doc, 10, 15)
    doc.autoTable({
      head: addressHead,
      body: addressData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: { 0: { cellWidth: 93 }, 1: { cellWidth: 30 } },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //CONTACTS
    if (this.contactList.length > 0) {
      this.contactList.forEach((element, index) => {
        //if (index < 15) {
        var createdonDate =
          element.createdonDate != undefined
            ? this.getDate(element.createdonDate)
            : null
        var lastUpdatedDate =
          element.lastUpdatedDate != undefined
            ? this.getDate(element.lastUpdatedDate)
            : null
        var contactTemp = [
          element.type,
          element.telephoneNo,
          element.peopleLinked,
          lastUpdatedDate,
          createdonDate,
        ]
        contactData.push(contactTemp)
        //}
      })
    } else {
      var nonDataList = ['No records found']
      this.contactList.push(nonDataList)
      contactData.push(nonDataList)
      this.contactList = []
    }
    this.getTableHeading('CONTACTS', doc, 10, 15)
    doc.autoTable({
      head: contactHead,
      body: contactData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: {
        0: { cellWidth: 33 },
        1: { cellWidth: 50 },
        2: { cellWidth: 40 },
      },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //DEBT REVIEW
    if (this.debtReviewList.length > 0) {
      this.debtReviewList.forEach((element) => {
        var applicationCreationDate =
          element.applicationCreationDate != undefined
            ? this.getDate(element.applicationCreationDate)
            : null
        var debtReviewTemp = [
          element.debtCounsellorRegistrationNo,
          element.debtCounsellorFirstName,
          element.debtCounsellorSurname,
          element.debtReviewStatusCode,
          applicationCreationDate,
        ]

        debtRevieData.push(debtReviewTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.debtReviewList.push(nonDataList)
      debtRevieData.push(nonDataList)
      this.debtReviewList = []
    }
    this.getTableHeading('DEBT REVIEW', doc, 10, 15)
    doc.autoTable({
      head: debtRevieHead,
      body: debtRevieData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: {
        0: { cellWidth: 50 },
        1: { cellWidth: 35 },
        3: { cellWidth: 35 },
      },
      headStyles: { fillColor: [230, 230, 230] },
    })

    this.getHightofHeading(doc, 10)
    // console.log('CONSUMER JUDGEMENT ' + this.rectPageHieght)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //CONSUMER JUDGEMENT
    if (this.judgementList.length > 0) {
      this.judgementList.forEach((element) => {
        var casefilingdate =
          element.casefilingdate != undefined
            ? this.getDate(element.casefilingdate)
            : null
        var createdodate =
          element.createdodate != undefined
            ? this.getDate(element.createdodate)
            : null
        var judgementTemp = [
          element.casenumber,
          casefilingdate,
          element.casetype,
          element.casereason,
          element.plaintiffname,
          createdodate,
        ]
        judgementData.push(judgementTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.judgementList.push(nonDataList)
      judgementData.push(nonDataList)
      this.judgementList = []
    }
    this.getTableHeading('CONSUMER JUDGEMENT', doc, 10, 15)
    doc.autoTable({
      head: judgementHead,
      body: judgementData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: {
        1: { cellWidth: 28 },
        2: { cellWidth: 30 },
        4: { cellWidth: 36 },
      },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //EMPLOYMENT
    if (this.employeesList.length > 0) {
      this.employeesList.forEach((element) => {
        var createdDate =
          element.createdDate != undefined
            ? this.getDate(element.createdDate)
            : null
        var date = element.date != undefined ? this.getDate(element.date) : null
        var employeeTemp = [
          element.employer,
          element.occupation,
          date,
          createdDate,
        ]
        employeesData.push(employeeTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.employeesList.push(nonDataList)
      employeesData.push(nonDataList)
      this.employeesList = []
    }
    this.getTableHeading('EMPLOYMENT', doc, 10, 15)
    doc.autoTable({
      head: employeesHead,
      body: employeesData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: {
        0: { cellWidth: 68 },
        1: { cellWidth: 45 },
        2: { cellWidth: 40 },
      },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //DIRECTORSHIP
    if (this.directorshipList.length > 0) {
      this.directorshipList.forEach((element) => {
        var appointmentDate =
          element.appointmentDate != undefined
            ? this.getDate(element.appointmentDate)
            : null
        var createdOnDate =
          element.createdOnDate != undefined
            ? this.getDate(element.createdOnDate)
            : null
        var directorTemp = [
          element.companyname,
          element.directorStatusCode,
          appointmentDate,
          createdOnDate,
        ]
        directorData.push(directorTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.directorshipList.push(nonDataList)
      directorData.push(nonDataList)
      this.directorshipList = []
    }

    this.getTableHeading('DIRECTORSHIP', doc, 10, 15)
    doc.autoTable({
      head: directorHead,
      body: directorData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: { 0: { cellWidth: 80 }, 1: { cellWidth: 35 } },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //PROPERTY OWNERSHIP
    if (this.propertyOwnershipList.length > 0) {
      this.propertyOwnershipList.forEach((element) => {
        var datePurchase =
          element.datePurchase != undefined
            ? this.getDate(element.datePurchase)
            : null
        var propertyTemp = [
          element.type,
          datePurchase,
          element.isCurrentOwner,
          element.fullAddress,
          element.town,
          this.formatToCurrency(element.purchaseAmount), //converting currency
        ]
        propertyData.push(propertyTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.propertyOwnershipList.push(nonDataList)
      propertyData.push(nonDataList)
      this.propertyOwnershipList = []
    }
    this.getTableHeading('PROPERTY OWNERSHIP', doc, 10, 15)
    doc.autoTable({
      head: propertyHead,
      body: propertyData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      columnStyles: { 2: { cellWidth: 25 }, 3: { cellWidth: 40 } },
      headStyles: { fillColor: [230, 230, 230] },
    })
    this.getHightofHeading(doc, 10)
    this.getEmptyAutoTable(doc, this.rectPageHieght)

    //RELATIONSHIP LINK
    if (this.relationshipLinkList.length > 0) {
      this.relationshipLinkList.forEach((element) => {
        const DOB =
          element.idNumber.substring(0, 2) +
          '-' +
          element.idNumber.substring(4, 2) +
          '-' +
          element.idNumber.substring(4, 6)
        var relationshipTemp = [
          element.type,
          element.keyMatch,
          DOB,
          element.fullName,
        ]
        relationshipData.push(relationshipTemp)
      })
    } else {
      var nonDataList = ['No records found']
      this.relationshipLinkList.push(nonDataList)
      relationshipData.push(nonDataList)
      this.relationshipLinkList = []
    }
    this.getTableHeading('RELATIONSHIP LINK', doc, 10, 15)
    doc.autoTable({
      head: relationshipHead,
      body: relationshipData,
      startY: doc.lastAutoTable.finalY + 18,
      styles: { fontSize: 9, font: 'Avenir', textColor: [0, 0, 0] },
      headStyles: { fillColor: [230, 230, 230] },
    })

    // PAGE NUMBERING
    const pageCount = doc.internal.getNumberOfPages()
    for (var i = 1; i <= pageCount; i++) {
      doc.setTextColor(0, 0, 0)
      doc.setPage(i)
      doc.setFontSize(9)
      doc.text(
        'Page ' + String(i) + ' of ' + String(pageCount),
        210 - 15,
        297 - 10,
        null,
        null,
        'right',
      )
    }

    doc.setFontSize(8)
    doc.setTextColor(0, 0, 0)
    doc.text(dislaimer, 105, 270, 'center')

    doc.save('Profile.pdf')
  }
  
  //------------------ EndProfile PDF ----------------------

  /*downloadpdf() {
    console.log('divorce data :' + this.res.deceasedDate)
    if (this.response.tabs.includes('CommercialProfile')) {
      this.profilePresent = true
      this.printconsumer = true
    } else {
      this.profilePresent = true
      this.printconsumer = false
    }
    let popupWinindow
    document.getElementById('DetailsList').style.display = 'block'
    document.getElementById('logoprint').style.display = 'block'
    let innerContents = document.getElementById('Personaldetail').innerHTML
    popupWinindow = window.open(
      '',
      '_blank',
      'width=600,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no',
    )
    popupWinindow.document.open()
    //popupWinindow.document.write('<html><head><style>@page { size: auto;  margin: 0mm; } @media print { body {-webkit-print-color-adjust: exact;} } </style><link rel="stylesheet" type="text/css" href="../../assets/demo/demo2/base/style.bundle.css" /></head><body onload="window.print()">' + innerContents + '</body></html>');
    popupWinindow.document.write(
      '<html><head><meta charset="utf-8"><base href="/"><meta name="viewport" content="width=device-width, initial-scale=2"><style>@page { size: auto;  margin: 0mm; } @media print { body {-webkit-print-color-adjust: inherit;font-size: 50pt;} } </style><link rel="stylesheet" type="text/css" href="../../assets/demo/demo2/base/style.bundle.css" /><link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.1.3/css/bootstrap.min.css"><link href="./assets/vendors/base/vendors.bundle.css" rel="stylesheet" type="text/css" /><link href="./assets/demo/demo2/base/style.bundle.css" rel="stylesheet" type="text/css" /><link href="//db.onlinewebfonts.com/c/b4fe53dcec196556a796e66baa9b7cc3?family=Avenir" rel="stylesheet" type="text/css" /><script src="https://stackpath.bootstrapcdn.com/bootstrap/4.1.3/js/bootstrap.min.js" integrity="sha384-ChfqqxuZUCnJSK3+MXmPNIyE6ZbWh2IMqE241rYiqJxyMiZ6OW/JmZQ5stwEULTy" crossorigin="anonymous"></script><script src="https://code.jquery.com/jquery-3.3.1.slim.min.js" integrity="sha384-q8i/X+965DzO0rT7abK41JStQIAqVgRVzpbzo5smXKp4YfRvH+8abtTE1Pi6jizo" crossorigin="anonymous"></script><script src="https://cdnjs.cloudflare.com/ajax/libs/popper.js/1.14.3/umd/popper.min.js" integrity="sha384-ZMP7rVo3mIykV+2+9J3UJ46jBk0WLaUAdn689aCwoqbBJiSnjAK/l8WvCWPIPm49" crossorigin="anonymous"></script> <script>    WebFont.load({google: { "families": ["Poppins:300,400,500,600,700", "Roboto:300,400,500,600,700"] },active: function () { sessionStorage.fonts = true; }});</script></head><body onload="window.print()"> <label for="javascript" style="font-size: 16px;font-family: Calibri">An Authorized Agent of XDS (A Credit Bureau)  </label> <br/>' +
        innerContents +
        '     Disclaimer: Inspirit Data Analytics Services(Pty) Ltd, an authorized agent of XDS, shall not be liable for any damage or loss, both directly or indirectly, as a result of the use of or the omission to use the statement made in response to the enquiry made herein or for any consequential / inconsequential damages, loss of profit or special damages arising out of the issuing of that statement or any use thereof. Copyright 2021 Inspirit Data Analytics Services(Pty) Ltd(Reg No: 2017653373) Powered by Xpert Decision Systems(XDS).</body></html>',
    )
    //popupWinindow.document.write('<html><head></head><body onload="window.print()">' + innerContents + '</body></html>');
    popupWinindow.document.close()
    document.getElementById('DetailsList').style.display = 'none'
    document.getElementById('logoprint').style.display = 'none'
  }*/

  ngOnDestroy() {
    //if (this.isXDS == 'NO') {
      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          this.headernavService.updatePoints(this.points)
        })
    //}
    //else {
    //  this.points = 100
    //  this.headernavService.updatePoints(this.points)
    //}
    this._tracingRequest = null
    this._profileRequest = null
    this.sub.unsubscribe()
  }

  Getrelation(idno: any) {
    this.loading = true
    $('#content').css('display', 'block')
    this.searchprofile(idno, 'relation')
  }

  searchprofile(id: any, tp: any) {
    this.addressList = []
    this.contactList = []
    this.employeesList = []
    this.directorshipList = []
    this.propertyOwnershipList = []
    this.relationshipLinkList = []
    this.debtReviewList = []
    this.judgementList = []

    this.res = new PersonProfile()
    if ((this.points > 0) || (this.isXDS == 'YES')) {
      this.loading = true
      if (tp == 'relation') this._profileRequest.searchCriteria = 'Id Number'
      else this._profileRequest.searchCriteria = 'Spouse Id'
      this._profileRequest.spouseID = id
      this._profileRequest.istrailuser = this.istrailuser
      this.oldtime = new Date()
      // krishna start
      this._profileRequest.customerId = this.customerid
      this._profileRequest.userId = this.userid
      // krishna end
      this.personProfile
        .getProfileDetils(this._profileRequest)
        .subscribe((result1) => {
          this.newtime = new Date()
          this.timetaken = (
            (this.newtime.getTime() - this.oldtime.getTime()) /
            1000
          ).toFixed(2)
          //response
          this.res = result1
          this.response = this.res
          console.log(this.res)
          console.log(this.response)
          
          //console.log(JSON.stringify(this.response))
          
          if (this.res.errorMessage != '') {
            this.isSpouse = true
            this.loading = false
            document.getElementById('nodata').click()
          } else {
            this.loading = false
            if (
              this.res.consumerjudgements.length > 0 &&
              this.res.tabs.includes('ConsumerJudgement')
            ) {
              this.judgementList = this.res.consumerjudgements
              this.judgePresent = true
            }
            if (
              this.res.timelines.length > 0 &&
              this.res.tabs.includes('ConsumerTimeline')
            )
              this.timelinePresent = true
            if (
              this.res.consumerDebtReview.length > 0 &&
              this.res.tabs.includes('ConsumerDebtReview')
            ) {
              this.debtReviewList = this.res.consumerDebtReview
              this.debtPresent = true
            }
            if (
              this.res.relationships.length > 0 &&
              this.res.tabs.includes('ConsumerRelationship')
            ) {
              this.relationPresent = true
              this.relationshipLinkList = this.res.relationships
            }
            if (
              this.res.directorShips.length > 0 &&
              this.res.tabs.includes('ConsumerDirector')
            ) {
              this.directorPresent = true
              this.directorshipList = this.res.directorShips
            }
            if (
              this.res.addresses.length > 0 &&
              this.res.tabs.includes('ConsumerAddress')
            ) {
              this.addressPresent = true
              this.addressList = this.res.addresses
            }
            if (
              this.res.contacts.length > 0 &&
              this.res.tabs.includes('ConsumerTelephone')
            )
              this.contactPresent = true
            this.contactList = this.res.contacts
            if (
              this.res.employees.length > 0 &&
              this.res.tabs.includes('ConsumerEmployment')
            )
              this.employmentPresent = true
            this.employeesList = this.res.employees
            if (
              this.res.propertyOwners.length > 0 &&
              this.res.tabs.includes('ConsumerProperty')
            ) {
              this.propertyOwnershipList = this.res.propertyOwners
              this.deedsPresent = true
            }

            if (this.res.tabs.includes('ConsumerProfile')) {
              this.printconsumer = true
              this.profilePresent = true
              // krishna start
              this.printaka = true
                // krishna end
            }
            if (this.res.firstName != 'Unknown') this.fname = this.res.firstName
            if (this.res.firstName != 'Unknown') this.sname = this.res.surname
            if (this.res.secondName != 'Unknown')
              this.secname = this.res.secondName

            this.userName = this.res.firstName + ' ' + this.res.surname
            this.idNumber = this.res.idNumber
            // pending xds no credit points
            // if is xds set to 100 points
            //if (this.isXDS == 'NO') {
            this.tracingService
              .getPoints(this.userid, this.customerid)
              .subscribe((result) => {
                this.points = result
                this.headernavService.updatePoints(this.points)
              })
            //}
            //else {
            //  this.points = 100
            //  this.headernavService.updatePoints(this.points)
            //}
          }
        })
    } else {
      this.loading = false
      document.getElementById('nopoints').click()
    }
  }
  showModalContact(telephoneNo: string, type: any, dialingCode: any) {
    this._tracingRequest = new ConsumerSearchRequest()

    if (type == 'Email') {
      this._tracingRequest.emailaddress = telephoneNo
      this._tracingRequest.type = 'lookup'

      this._tracingRequest.isTrailuser = this.istrailuser
      this.router.navigate(['tracingSearch/consumerSearchResult'], {
        queryParams: this._tracingRequest,
        skipLocationChange: true,
      })
    } else {
      if (dialingCode == '27') {
        this._tracingRequest.phoneNumber = telephoneNo.replace(dialingCode, '')
      }
      this._tracingRequest.type = 'lookup'

      this._tracingRequest.isTrailuser = this.istrailuser
      this.router.navigate(['tracingSearch/consumerSearchResult'], {
        queryParams: this._tracingRequest,
        skipLocationChange: true,
      })
    }
  }
  consumerlist() {
    //this.sub.unsubscribe();
    this._tracingRequest.isTrailuser = this.istrailuser
    this.router.navigate(['tracingSearch/consumerSearchResult'], {
      queryParams: this._tracingRequest,
      skipLocationChange: true,
    })
  }
  tracing() {
    this.sub.unsubscribe()
    this._tracingRequest.isTrailuser = this.istrailuser
    this.router.navigate(['tracingSearch'], {
      queryParams: { type: 'Profile' },
      skipLocationChange: true,
    })
  }
  change(file: any) {}
  contactmodal(id: any) {
    const modalRef = this.modalService.open(ContactDetailsComponent, {
      size: 'lg',
    })
    modalRef.componentInstance.consumer = true
    modalRef.componentInstance.contact = this.res.contacts.find(
      (x) => x.id == id,
    )
  }
  GlobalSearch() {
    this._tracingRequest = new ConsumerSearchRequest()
    if ((this.points != null && this.points != 'undefined' && this.points > 0) || (this.isXDS == 'YES')) {
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
        this.router.navigate(['tracingSearch/consumerSearchResult'], {
          queryParams: this._tracingRequest,
          skipLocationChange: true,
        })
      }
    } else {
      this.errormsg("You don't have credits")
    }
  }
  downloadFile(blob, filename: string) {
    var url = window.URL.createObjectURL(blob)
    var link = document.createElement('a')
    link.setAttribute('href', url)
    link.setAttribute('download', filename)
    link.click()
  }
}
