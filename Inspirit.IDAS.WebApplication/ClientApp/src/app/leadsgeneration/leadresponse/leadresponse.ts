import { Component, Inject, DebugElement } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router, ActivatedRoute } from '@angular/router'
import {
  LeadGenerationService,
  LeadGenerationResponse,
  LeadsRequest,
  LeadsResponse,
  LeadsMessage,
} from '../../services/services'
import { headernavService } from '../../header-nav/header-nav.service'
import { EventEmitter } from 'events'
import { debug, isNullOrUndefined } from 'util'
import * as XLSX from 'xlsx'
import { forEach } from '@angular/router/src/utils/collection'
import { PercentPipe } from '@angular/common'
import { concat } from 'rxjs/operators'
import { MorrisJsModule } from 'angular-morris-js'

@Component({
  selector: 'leadresponse',
  templateUrl: './leadresponse.html',
})
export class LeadResponseComponent {
  leadres: LeadGenerationResponse = new LeadGenerationResponse()
  leadreq: LeadsRequest = new LeadsRequest()
  dtOptions: DataTables.Settings = {}
  loading: boolean = false
  userid: any
  customerid: any
  warningMesage: any
  tableresponse: LeadsResponse[]
  maritalinfo: any
  geninfo: any
  sub: any
  id: any
  readony: boolean = false
  mode: string = 'View'
  messageres: LeadsMessage = new LeadsMessage()
  responsetype: string = 'Table'
  leadsreqdisplay: LeadsRequest = new LeadsRequest()
  lsmExists: boolean = false
  contactExists: boolean = false
  riskExists: boolean = false
  incomeExists: boolean = false
  date1Exists: boolean = false
  date2Exists: boolean = false
  isvalid: boolean = false
  name: any
  isVisible: boolean = false
  editable: boolean = false
  totalsum: number = 0
  reqtotal: number = 0
  leadId: any
  type: any
  constructor(
    public LeadGenerationService: LeadGenerationService,
    public headernavservice: headernavService,
    public router: Router,
    public route: ActivatedRoute,
  ) {
    this.headernavservice.toggle(true)
    this.userid = localStorage.getItem('userid')
    this.customerid = localStorage.getItem('customerid')
    this.name = localStorage.getItem('name')
    if (this.name != null && this.name != 'undefined') {
      this.headernavservice.updateUserName(this.name)
    }
  }
  ngOnInit(): void {
    this.leadsreqdisplay = new LeadsRequest()
    this.loading = true
    this.sub = this.route.params.subscribe((params) => {
      if (params['id'] != null && params['id'] != 'undefined') {
        let arrays: string[] = params['id'].split(',')
        this.id = arrays[0]
        this.type = params['type']
      }
    })
    if (this.id != null && this.id != 'undefined') {
      this.mode == 'View'
      this.readony = true
      this.responsetype = 'Table'
      if (this.type == 'update') {
        this.leadreq = JSON.parse(localStorage.getItem('leadrequestupdates'))
        this.leadreq.type = 'UPDATE'
        this.LeadGenerationService.getLeadsCount(this.leadreq).subscribe(
          (response) => {
            this.loading = false
            this.mode = 'View'
            this.readony = true
            this.leadres = response
            if (this.leadres == null) {
              this.warningMesage = 'No Records available for the given request.'
              document.getElementById('opennolead').click()
              if (
                this.leadres.isinvoiceraised == true ||
                this.leadres.isProfileRaised == true
              ) {
                this.isVisible = true
                this.editable = false
              }
            }
            if (
              this.leadres.isinvoiceraised == true ||
              this.leadres.isProfileRaised == true
            ) {
              this.isVisible = true
              this.editable = false
            }
            this.tableresponse = this.leadres.tableresponse
            this.geninfo = this.leadres.morrisGenders
            this.maritalinfo = this.leadres.morrisMaritalStaus
            this.leadsreqdisplay = JSON.parse(this.leadres.leadInput)
            console.log(this.leadsreqdisplay)
            this.leadId = this.id
            this.tableresponse.forEach((obj) => {
              this.totalsum += obj.totalSum
            })
            this.tableresponse.forEach((obj) => {
              this.reqtotal += obj.requiredCount
            })
            if (this.leadsreqdisplay.alloylst.length > 0)
              this.contactExists = true
            if (this.leadsreqdisplay.inclst.length > 0) this.incomeExists = true
            if (this.leadsreqdisplay.lsmlst.length > 0) this.lsmExists = true
            if (this.leadsreqdisplay.risklst.length > 0) this.riskExists = true
          },
        )
      } else {
        this.LeadGenerationService.getLeadtablevalue(this.id).subscribe(
          (res) => {
            this.leadres = res
            this.loading = false
            if (
              this.leadres.isinvoiceraised == true ||
              this.leadres.isProfileRaised == true
            ) {
              this.isVisible = true
              this.editable = false
            }
            this.leadres = JSON.parse(this.leadres.leadtableresponse)
            this.tableresponse = this.leadres.tableresponse
            this.leadId = this.id
            this.tableresponse.forEach((obj) => {
              this.totalsum += obj.totalSum
            })
            this.tableresponse.forEach((obj) => {
              this.reqtotal += obj.requiredCount
            })
            this.leadsreqdisplay = JSON.parse(res.leadInput)
            if (this.leadsreqdisplay.alloylst.length > 0)
              this.contactExists = true
            if (this.leadsreqdisplay.inclst.length > 0) this.incomeExists = true
            if (this.leadsreqdisplay.lsmlst.length > 0) this.lsmExists = true
            if (this.leadsreqdisplay.risklst.length > 0) this.riskExists = true
          },
        )
      }
    } else {
      this.responsetype = 'Add'
      this.leadreq = JSON.parse(localStorage.getItem('leadrequest'))
      this.isVisible = true
      this.leadreq.type = 'ADD'
      this.LeadGenerationService.getLeadsCount(this.leadreq).subscribe(
        (response) => {
          this.loading = false
          this.mode = 'View'
          this.readony = true
          this.leadres = response
          if (this.leadres == null) {
            this.warningMesage = 'No Records available for the given request.'
            document.getElementById('opennolead').click()
            if (
              this.leadres.isinvoiceraised == true ||
              this.leadres.isProfileRaised == true
            ) {
              this.isVisible = true
              this.editable = false
            }
          }
          this.editable = false
          if (
            this.leadres.isinvoiceraised == true ||
            this.leadres.isProfileRaised == true
          ) {
            this.isVisible = true
            this.editable = false
          }
          this.tableresponse = this.leadres.tableresponse
          this.geninfo = this.leadres.morrisGenders
          this.maritalinfo = this.leadres.morrisMaritalStaus
          this.leadsreqdisplay = JSON.parse(this.leadres.leadInput)
          this.leadId = this.leadres.leadId
          this.tableresponse.forEach((obj) => {
            this.totalsum += obj.totalSum
          })
          this.tableresponse.forEach((obj) => {
            this.reqtotal += obj.requiredCount
          })
          if (this.leadsreqdisplay.alloylst.length > 0)
            this.contactExists = true
          if (this.leadsreqdisplay.inclst.length > 0) this.incomeExists = true
          if (this.leadsreqdisplay.lsmlst.length > 0) this.lsmExists = true
          if (this.leadsreqdisplay.risklst.length > 0) this.riskExists = true
        },
      )
    }
  }
  list() {
    this.router.navigate(['leadGeneration'])
  }
  ngOnDestroy() {
    localStorage.removeItem('leadrequest')
    localStorage.removeItem('leadrequestupdates')
  }
  Edit() {
    this.mode = 'Edit'
    this.readony = false
    this.isvalid = true
  }
  save() {
    this.leadres.tableresponse = this.tableresponse
    if (this.id != null && this.id != 'undefined' && this.id != 'update')
      this.leadres.leadId = this.id
    else this.leadres.leadId = this.leadres.leadId

    this.LeadGenerationService.updateLeads(this.leadres).subscribe(
      (response) => {
        this.messageres = response
        if (this.messageres.isSuccess == true) {
          this.warningMesage = 'Saved Successfully.'
          this.mode = 'View'
          this.readony = true
          document.getElementById('openPopUpLead').click()
        } else {
          this.warningMesage = this.messageres.message
          document.getElementById('openPopUpLead').click()
        }
      },
    )
  }
  validatecount(available: any, changecount: any) {
    this.isvalid = false
    this.totalsum = 0

    if (changecount < 0) {
      this.warningMesage = 'Entered Number should not be a negetive value.'
      document.getElementById('openPopUpLead').click()
    }

    if (changecount > available) {
      this.warningMesage =
        'The edited count should not be greater than Available count.'
      document.getElementById('openPopUpLead').click()
    } else {
      this.isvalid = true
    }

    this.tableresponse.forEach((obj) => {
      this.totalsum += obj.totalSum
    })

    this.tableresponse.forEach((obj) => {
      this.reqtotal += obj.requiredCount
    })
  }
  isNumberKey(evt) {
    var charCode = evt.which ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      this.warningMesage = 'Entered number should not be a decimal value.'
      return false
    } else {
      return true
    }
  }

  generateinvoice() {
    this.loading = true
    this.LeadGenerationService.generateProformaInvoice(
      this.userid,
      this.id,
    ).subscribe((res) => {
      this.loading = false
      if (res.isSuccess) {
        this.warningMesage = 'Invoice Generated.'
        document.getElementById('opennolead').click()
      } else {
        this.warningMesage = 'Contact IDAS Admin for Invoice.'
        document.getElementById('openPopUpLead').click()
      }
    })
  }
  cancelLead() {
    this.loading = true
    this.LeadGenerationService.cancelLead(this.id).subscribe((res) => {
      this.loading = false
      if (res == 'Lead has been cancelled') {
        this.warningMesage = 'Lead has been cancelled.'
        document.getElementById('opennolead').click()
      } else {
        this.warningMesage = 'Contact IDAS Admin for cancel Lead.'
        document.getElementById('openPopUpLead').click()
      }
    })
  }
  backToForm() {
    if (this.id != null && this.id != 'undefined' && this.id != 'update')
      this.leadId = this.id
    else this.leadId = this.leadres.leadId
    console.log(this.leadsreqdisplay)
    localStorage.setItem('leadinput', JSON.stringify(this.leadsreqdisplay))
    this.router.navigate(['leadGeneration/process', this.id])
  }
}
