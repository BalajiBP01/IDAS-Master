import { Component, OnInit, OnDestroy } from '@angular/core'
import {
  HttpClient,
  HttpRequest,
  HttpEventType,
  HttpResponse,
} from '@angular/common/http'
import { Router } from '@angular/router'
import { ActivatedRoute } from '@angular/router'
import { debug, isNullOrUndefined } from 'util'
import {
  Customer,
  CustomersVm,
  CustomerService,
  CustomerCrudResponse,
  UserService,
  UserPermission,
  LookupData,
} from '../../services/services'
import { EventEmitter } from 'events'
import { AsideNavService } from '../../aside-nav/aside-nav.service'
import { createElementCssSelector } from '@angular/compiler'
import {
  NgbModal,
  NgbActiveModal,
  NgbModalOptions,
} from '@ng-bootstrap/ng-bootstrap'
import { PopupComponent } from '../../popup/popup.component'
import { PopupService } from '../../popup/popupService'
import { forEach } from '@angular/router/src/utils/collection'

@Component({
  selector: 'app-customerdetails',
  templateUrl: './customerdetails.component.html',
})
export class customerdetailComponent implements OnInit, OnDestroy {
  message: string
  progress: number
  loading: boolean = false

  custObj: CustomersVm = new CustomersVm()
  id: any
  errorMessage: any
  errorMessageValue: any = ''
  emailPattern: any
  private sub: any
  mode: string = 'view'
  reponse: CustomerCrudResponse
  readonly: boolean = false
  showStatusUpdate: boolean = false
  statusOption: string
  hasFile: any
  currentObj: Customer = new Customer()
  userid: any
  typeOfBusiness: LookupData[]
  enquiryReasons: LookupData[]
  success: boolean
  userper: UserPermission = new UserPermission()
  xds = false;   //padmavati

  //purpose
  purposeA: boolean = false
  purposeB: boolean = false
  purposeC: boolean = false
  purposeD: boolean = false
  purposeE: boolean = false
  purposeF: boolean = false
  purpose: string

  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private customerService: CustomerService,
    private http: HttpClient,
    public userService: UserService,
    public asideNavService: AsideNavService,
    private modalService: NgbModal,
    public popupservice: PopupService,
  ) {
    this.asideNavService.toggle(true)

    this.userid = localStorage.getItem('userid')
    this.userService
      .getPermission(this.userid, 'Customers')
      .subscribe((result) => {
        this.userper = result
        if (this.userper == null || this.userper.viewAction == false) {
          document.getElementById('nopermission').click()
        }
      })
  }
  ngOnInit(): void {
    this.errorMessageValue = ''
    this.customerService.getLookupDatas().subscribe((resp) => {
      this.typeOfBusiness = resp
    })
    this.customerService.getEnquiryReasonLookupDatas().subscribe((resp) => {
      this.enquiryReasons = resp
    })
    this.sub = this.route.params.subscribe((params) => {
      this.id = params['id']
      if (typeof this.id == 'undefined' || typeof this.id == null) {
        this.mode = 'add'
        this.currentObj = new Customer()
        this.readonly = false
      } else {
        this.mode = 'view'
        this.loading = true
        this.customerService.view(this.id).subscribe((result) => {
          this.custObj = result
          this.readonly = true
          this.loading = false
          this.currentObj = this.custObj.customer
          this.purpose = this.custObj.customer.purpose
          if (this.purpose.includes('a')) this.purposeA = true
          if (this.purpose.includes('b')) this.purposeB = true
          if (this.purpose.includes('c')) this.purposeC = true
          if (this.purpose.includes('d')) this.purposeD = true
          if (this.purpose.includes('e')) this.purposeE = true
          if (this.purpose.includes('f')) this.purposeF = true
        })
      }
    })
  }
  dash() {
    this.router.navigate(['dashboard'])
  }
  ngOnDestroy() {
    this.sub.unsubscribe()
  }
  onKey(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true
    var keynum
    var keychar
    var charcheck = /^[A-Za-z0-9- ]+$/
    if (window.event) {
      keynum = e.keyCode
    } else {
      if (e.which) {
        keynum = e.which
      } else return true
    }
    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }
  alphaonKey(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true
    var keynum
    var keychar
    var charcheck = /^[A-Za-z0-9- ]+$/
    if (window.event) {
      keynum = e.keyCode
    } else {
      if (e.which) {
        keynum = e.which
      } else return true
    }
    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }

  numberonKey(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /[0-9]/
    if (window.event) {
      keynum = e.keyCode
    } else {
      if (e.which) {
        keynum = e.which
      } else return true
    }

    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }
  save() {
    this.loading = true
    this.errorMessageValue = ''
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/
    if (
      isNullOrUndefined(this.currentObj.branchLocation) ||
      this.currentObj.branchLocation == ''
    ) {
      this.errorMessageValue = 'branch Location is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingType) ||
      this.currentObj.billingType == ''
    ) {
      this.errorMessageValue = 'Billing type is required'
    }
    if (
      isNullOrUndefined(this.currentObj.physicalAddress) ||
      this.currentObj.physicalAddress == ''
    ) {
      this.errorMessageValue = 'Physical Address is required'
    }
    if (
      isNullOrUndefined(this.currentObj.postalAddress) ||
      this.currentObj.physicalAddress == ''
    ) {
      this.errorMessageValue = 'Postal Address is required'
    }
    if (
      isNullOrUndefined(this.currentObj.enquiryReason) ||
      this.currentObj.enquiryReason == ''
    ) {
      this.errorMessageValue = 'Enquiry Reason is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingType) ||
      this.currentObj.billingType == ''
    ) {
      this.errorMessageValue = 'Billing Type is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingEmail) ||
      this.currentObj.billingEmail == ''
    ) {
      this.errorMessageValue = 'Please Enter Billing Email Id'
    }
    if (this.currentObj.billingEmail != undefined) {
      if (!this.currentObj.billingEmail.match(this.emailPattern)) {
        this.errorMessageValue = 'Please Enter Valid Billing Email Id'
      }
    }
    if (
      isNullOrUndefined(this.currentObj.telephoneNumber) ||
      this.currentObj.telephoneNumber == ''
    ) {
      this.errorMessageValue = 'Please Enter Telephone number'
    }
    if (this.currentObj.telephoneNumber != undefined) {
      if (this.currentObj.telephoneNumber.length != 10) {
        this.errorMessageValue = ' Please Enter Valid Telephone Number '
      }
    }

    if (
      isNullOrUndefined(this.currentObj.accountDeptContactPerson) ||
      this.currentObj.accountDeptContactPerson == ''
    ) {
      this.errorMessageValue = "Account Department's Contact Person is required"
    }
    if (
      isNullOrUndefined(this.currentObj.accountDeptTelephoneNumber) ||
      this.currentObj.accountDeptTelephoneNumber == ''
    ) {
      this.errorMessageValue = "Account Department's Telephone is required"
    }
    if (
      isNullOrUndefined(this.currentObj.accountDeptEmail) ||
      this.currentObj.accountDeptEmail == ''
    ) {
      this.errorMessageValue = "Account Department's Email is required"
    }
    if (
      isNullOrUndefined(this.currentObj.authFirstName) ||
      this.currentObj.authFirstName == ''
    ) {
      this.errorMessageValue = 'Authorised Person First Name is required'
    }
    if (
      isNullOrUndefined(this.currentObj.authSurName) ||
      this.currentObj.authSurName == ''
    ) {
      this.errorMessageValue = 'Authorised Person Sur Name is required'
    }
    if (
      isNullOrUndefined(this.currentObj.authIDNumber) ||
      this.currentObj.authIDNumber == ''
    ) {
      this.errorMessageValue = 'Authorised Person IDNumber is required'
    }
    if (
      isNullOrUndefined(this.currentObj.authPosition) ||
      this.currentObj.authPosition == ''
    ) {
      this.errorMessageValue = 'Authorised Person Position is required'
    }
    if (
      isNullOrUndefined(this.currentObj.authCellNumber) ||
      this.currentObj.authCellNumber == ''
    ) {
      this.errorMessageValue = 'Authorised Person Cell Number is required'
    }

    if (
      isNullOrUndefined(this.currentObj.typeOfBusiness) ||
      this.currentObj.typeOfBusiness == ''
    ) {
      this.errorMessageValue = 'Type of Business is required'
    }
    if (
      isNullOrUndefined(this.currentObj.registrationNumber) ||
      this.currentObj.registrationNumber == ''
    ) {
      this.errorMessageValue = 'Registration Number is required'
    }
    if (
      isNullOrUndefined(this.currentObj.registrationName) ||
      this.currentObj.registrationName == ''
    ) {
      this.errorMessageValue = 'Registration Name is required'
    }
    if (
      isNullOrUndefined(this.currentObj.tradingName) ||
      this.currentObj.tradingName == ''
    ) {
      this.errorMessageValue = 'Trading Name is required'
    }

    if (this.errorMessageValue != '') {
      document.getElementById('openModalButton').click()
      this.loading = false
      return
    }

    if (this.mode == 'add') {
      if (
        this.currentObj.billingEmail.includes('.') &&
        this.currentObj.billingEmail.includes('@')
      ) {
        this.custObj.idaSuserId = this.userid
        this.custObj.customer = this.currentObj
        this.loading = false
        this.purpose = null
        if (this.purposeA == true) this.purpose = 'a'
        if (this.purposeB == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'b'
          else this.purpose = 'b'
        }
        if (this.purposeC == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'c'
          else this.purpose = 'c'
        }
        if (this.purposeD == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'd'
          else this.purpose = 'd'
        }
        if (this.purposeE == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'e'
          else this.purpose = 'e'
        }
        if (this.purposeF == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'f'
          else this.purpose = 'f'
        }
        this.custObj.customer.purpose = this.purpose
        console.log(this);
        this.customerService.insert(this.custObj).subscribe((result) => {
          this.reponse = result
          if (this.reponse.isSuccess) this.router.navigate(['customerlist'])
          else {
            if (
              this.reponse.message != null &&
              this.reponse.message != 'undefined'
            ) {
              this.errorMessageValue = this.reponse.message
              this.loading = false
              document.getElementById('openModalButton').click()
              return
            }
          }
        })
      }
    } else if (this.mode == 'edit') {
      {
        this.custObj.idaSuserId = this.userid
        this.custObj.customer = this.currentObj
        this.purpose = null
        if (this.purposeA == true) this.purpose = 'a'
        if (this.purposeB == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'b'
          else this.purpose = 'b'
        }
        if (this.purposeC == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'c'
          else this.purpose = 'c'
        }
        if (this.purposeD == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'd'
          else this.purpose = 'd'
        }
        if (this.purposeE == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'e'
          else this.purpose = 'e'
        }

        if (this.purposeF == true) {
          if (this.purpose != null && this.purpose != 'undefined')
            this.purpose = this.purpose + ',' + 'f'
          else this.purpose = 'f'
        }
        this.custObj.customer.purpose = this.purpose
        this.customerService.update(this.custObj).subscribe((result) => {
          this.reponse = result
          this.loading = false
          if (this.reponse.isSuccess) this.router.navigate(['customerlist'])
          else {
            if (
              this.reponse.message != null &&
              this.reponse.message != 'undefined'
            ) {
              this.errorMessageValue = this.reponse.message
              this.loading = false
              document.getElementById('openModalButton').click()
              return
            }
          }
        })
      }
    }
  }
  edit() {
    this.mode = 'edit'
    this.readonly = false
    this.loading = false
    this.customerService.view(this.id).subscribe((result) => {
      this.custObj = result
      this.loading = false
      this.currentObj = this.custObj.customer
      this.purpose = this.custObj.customer.purpose
      if (this.purpose.includes('a')) this.purposeA = true
      if (this.purpose.includes('b')) this.purposeB = true
      if (this.purpose.includes('c')) this.purposeC = true
      if (this.purpose.includes('d')) this.purposeD = true
      if (this.purpose.includes('e')) this.purposeE = true
      if (this.purpose.includes('f')) this.purposeF = true
    })
  }
  delete() {
    this.customerService.delete(this.id).subscribe((result) => {
      this.router.navigate(['customerlist'])
    })
  }
  list() {
    this.router.navigate(['customerlist'])
  }
  validations() {
    this.errorMessageValue = ''
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/
    if (
      isNullOrUndefined(this.currentObj.branchLocation) ||
      this.currentObj.branchLocation == ''
    ) {
      this.errorMessageValue = 'branch Location is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingType) ||
      this.currentObj.billingType == ''
    ) {
      this.errorMessageValue = 'Billing type is required'
    }

    if (
      isNullOrUndefined(this.currentObj.physicalAddress) ||
      this.currentObj.physicalAddress == ''
    ) {
      this.errorMessageValue = 'Physical Address is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingType) ||
      this.currentObj.billingType == ''
    ) {
      this.errorMessageValue = 'Billing Type is required'
    }
    if (
      isNullOrUndefined(this.currentObj.billingEmail) ||
      this.currentObj.billingEmail == ''
    ) {
      this.errorMessageValue = 'Please Enter Billing Email Id'
    } else if (this.currentObj.billingEmail != undefined) {
      if (!this.currentObj.billingEmail.match(this.emailPattern)) {
        this.errorMessageValue = 'Please Enter Valid Billing Email Id'
      }
    }
    if (
      isNullOrUndefined(this.currentObj.telephoneNumber) ||
      this.currentObj.telephoneNumber == ''
    ) {
      this.errorMessageValue = 'Please Enter Telephone number'
    } else if (this.currentObj.telephoneNumber != undefined) {
      if (this.currentObj.telephoneNumber.length != 10) {
        this.errorMessageValue = ' Please Enter Valid Telephone Number '
      }
    }
    if (
      isNullOrUndefined(this.currentObj.typeOfBusiness) ||
      this.currentObj.typeOfBusiness == ''
    ) {
      this.errorMessageValue = 'Type of Business is required'
    }
    if (
      isNullOrUndefined(this.currentObj.vatNumber) ||
      this.currentObj.vatNumber == ''
    ) {
      this.errorMessageValue = 'Vat Number is required'
    }
    if (
      isNullOrUndefined(this.currentObj.registrationNumber) ||
      this.currentObj.registrationNumber == ''
    ) {
      this.errorMessageValue = 'Registration Number is required'
    }
    if (
      isNullOrUndefined(this.currentObj.registrationName) ||
      this.currentObj.registrationName == ''
    ) {
      this.errorMessageValue = 'Registration Name is required'
    }
    if (
      isNullOrUndefined(this.currentObj.tradingName) ||
      this.currentObj.tradingName == ''
    ) {
      this.errorMessageValue = 'Trading Name is required'
    }
  }
  changeStatus() {
    this.loading = true
    this.custObj.idaSuserId = this.userid
    if (this.currentObj.status == 'Pending') {
      this.currentObj.status = 'Active'
    } else if (this.currentObj.status == 'Active') {
      this.currentObj.status = 'Inactive'
    } else if (this.currentObj.status == 'Inactive') {
      this.currentObj.status = 'Active'
    }
    this.custObj.customer = this.currentObj
    this.customerService.updateStatus(this.custObj).subscribe((result) => {
      this.reponse = result
      this.loading = false
      if (this.reponse.message != 'Undefined' && this.reponse.message != null) {
        this.errorMessageValue = this.reponse.message
        this.loading = false
        document.getElementById('openModalButton').click()
        return
      }
      this.router.navigate(['customerlist'])
    })
  }

  Exportfile(path: any, name: any) {
    if (path.length === 0) return
    this.customerService
      .downloadPdf(name, this.currentObj.id)
      .subscribe((resp) => {
        console.log(resp.data, resp.fileName)
        this.downloadFile(resp.data, resp.fileName)
      })
  }
  downloadFile(blob, filename: string) {
    var url = window.URL.createObjectURL(blob)
    var link = document.createElement('a')
    link.setAttribute('href', url)
    link.setAttribute('download', filename)
    link.click()
  }
  change(files) {
    this.validations()
    if (files.length === 0) return
    const formData = new FormData()

    for (let file of files) formData.append(file.name, file, this.currentObj.id)

    const uploadReq = new HttpRequest('POST', 'api/Customer', formData, {
      reportProgress: true,
    })
    this.http.request(uploadReq).subscribe((event) => {
      this.edit()
      if (event.type === HttpEventType.UploadProgress) {
        this.progress = Math.round((100 * event.loaded) / event.total)
      } else if (event.type === HttpEventType.Response)
        this.message = event.body.toString()
    })
  }
  remove(filepath: any) {
    let ngbModalOptions: NgbModalOptions = {
      backdrop: 'static',
      keyboard: false,
    }

    const modalRef = this.modalService.open(PopupComponent, ngbModalOptions)
    modalRef.componentInstance.message = 'Are you sure want to delete ?'
    modalRef.componentInstance.isconfirm = true

    this.popupservice.buttonchange.subscribe((credits = this.success) => {
      this.success = credits
      if (this.success == true) {
        this.loading = false
        this.customerService.deleteFile(filepath).subscribe((resp) => {
          this.edit()
        })
      }
    })
  }
}
