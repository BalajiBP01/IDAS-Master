import { Component, OnInit, OnDestroy } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { ActivatedRoute } from '@angular/router'
import { debug, isNullOrUndefined } from 'util'
import {
  CustomerUser,
  CustomerUserService,
  CrudUserResponse,
  CustomerService,
  UserService,
  UserPermission,
  CustomerUserVM,
} from '../../services/services'
import { AsideNavService } from '../../aside-nav/aside-nav.service'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import { PopupComponent } from '../../popup/popup.component'
import { PopupService } from '../../popup/popupService'

@Component({
  selector: 'app-customeruserdetails',
  templateUrl: './customeruserdetails.component.html',
})
export class customeruserdetailsComponent implements OnInit, OnDestroy {
  currObj: CustomerUserVM = new CustomerUserVM()
  currentObj: CustomerUser = new CustomerUser()
  id: any
  errorMessage: any
  errorMessageValue: any
  emailPattern: any
  private sub: any
  mode: string = 'view'
  reponse: CrudUserResponse
  status: boolean
  loading: boolean = false
  readonly: boolean = false
  custid: any
  companyname: string
  userid: any
  userper: UserPermission = new UserPermission()

  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private customerUserService: CustomerUserService,
    public customerService: CustomerService,
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

    if (typeof window !== 'undefined' ? window.localStorage : null) {
      this.custid = localStorage.getItem('customerid')
      this.customerService.getName(this.custid).subscribe((result) => {
        this.companyname = result
      })
    }
  }
  ngOnInit(): void {
    this.sub = this.route.params.subscribe((params) => {
      this.id = params['id']
      if (typeof this.id == 'undefined' || typeof this.id == null) {
        this.mode = 'add'
        this.currObj = new CustomerUserVM()
        this.currentObj = new CustomerUser()
        this.readonly = false
      } else {
        this.mode = 'view'
        this.loading = true
        if (typeof window !== 'undefined' ? window.localStorage : null) {
          this.companyname = localStorage.getItem('custname')
        }
        this.customerUserService.view(this.id).subscribe((result) => {
          this.currObj = result
          this.loading = false
          this.currentObj = this.currObj.customeruser
          if (this.currentObj.status == 'Pending') {
            this.status = true
          }
          this.readonly = true
        })
      }
    })
  }
  onKeynumber(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /^[0-9]+$/
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

  save() {
    this.loading = true
    this.errorMessageValue = ''
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/
    if (this.currentObj.isAdmin == true) {
      if (
        isNullOrUndefined(this.currentObj.phoneNumber) ||
        this.currentObj.phoneNumber == ''
      ) {
        this.errorMessageValue = 'Please Enter Phone Number'
      } else if (this.currentObj.phoneNumber != undefined) {
        if (this.currentObj.phoneNumber.length != 10) {
          this.errorMessageValue = 'Please Enter Valid Phone Number'
        }
      }
    }
    if (
      isNullOrUndefined(this.currentObj.email) ||
      this.currentObj.email == ''
    ) {
      this.errorMessageValue = 'Please Enter Email Id'
    } else if (this.currentObj.email != undefined) {
      if (!this.currentObj.email.match(this.emailPattern)) {
        this.errorMessageValue = 'Please Enter Valid Email Id'
      }
    }

    if (
      isNullOrUndefined(this.currentObj.idNumber) ||
      this.currentObj.idNumber == ''
    ) {
      this.errorMessageValue = 'ID Number is required'
    }

    if (
      isNullOrUndefined(this.currentObj.title) ||
      this.currentObj.title == ''
    ) {
      this.errorMessageValue = 'Title is required'
    }

    if (
      isNullOrUndefined(this.currentObj.lastName) ||
      this.currentObj.lastName == ''
    ) {
      this.errorMessageValue = 'Last Name is required'
    }
    if (
      isNullOrUndefined(this.currentObj.firstName) ||
      this.currentObj.firstName == ''
      
    ) {
      this.errorMessageValue = 'First Name is required'
    }

    if (this.errorMessageValue != '') {
      this.loading = false
      document.getElementById('openModalButton').click()
      return
    }

    if (this.mode == 'add') {
      this.loading = true
      if (
        this.currentObj.email.includes('.') &&
        this.currentObj.email.includes('@')
      ) {
        this.currObj.idaSuserId = this.userid
        this.currObj.customeruser = this.currentObj
        this.currObj.customeruser.customerId = this.custid
        this.customerUserService.insert(this.currObj).subscribe((result) => {
          this.reponse = result
          this.loading = false
          if (this.reponse.isSuccess) {
            this.router.navigate(['customerlist/customeruserlist', this.custid])
          } else {
            if (
              this.reponse.message != null &&
              this.reponse.message != 'undefined'
            ) {
              this.errorMessageValue = this.reponse.message
              this.loading = false
              document.getElementById('openModalButton').click()
              return
            }
            return
          }
        })
      }
    } else if (this.mode == 'edit') {
      {
        this.currObj.idaSuserId = this.userid
        this.currObj.customeruser = this.currentObj
        this.currObj.customeruser.customerId = this.custid
        this.customerUserService.update(this.currObj).subscribe((result) => {
          this.loading = false
          this.reponse = result
          if (this.reponse.isSuccess) {
            this.router.navigate(['customerlist/customeruserlist', this.custid])
          } else {
            if (
              this.reponse.message != null &&
              this.reponse.message != 'undefined'
            ) {
              this.errorMessageValue = this.reponse.message
              this.loading = false
              document.getElementById('openModalButton').click()
              return
            }
            return
          }
        })
      }
    }
  }
  edit() {
    this.mode = 'edit'
    this.loading = true
    this.readonly = false
    this.customerUserService.view(this.id).subscribe((result) => {
      this.currObj = result
      this.currentObj = this.currObj.customeruser
      this.loading = false
      if (this.reponse.isSuccess)
        this.router.navigate(['customerlist/customeruserlist', this.custid])
      else {
        if (
          this.reponse.message != null &&
          this.reponse.message != 'undefined'
        ) {
          this.errorMessageValue = this.reponse.message
          document.getElementById('openModalButton').click()
          return
        }
        return
      }
    })
  }
  delete() {
    if (typeof window !== 'undefined' ? window.localStorage : null) {
      this.custid = localStorage.getItem('customerid')
    }
    this.customerUserService.delete(this.id).subscribe((result) => {
      this.router.navigate(['customerlist/customeruserlist', this.custid])
    })
  }

  list() {
    if (typeof window !== 'undefined' ? window.localStorage : null) {
      this.custid = localStorage.getItem('customerid')
    }
    this.router.navigate(['customerlist/customeruserlist', this.custid])
  }
  ngOnDestroy() {
    this.sub.unsubscribe()
  }
  changeStatus() {
    this.loading = true
    if (this.currentObj.status == 'Pending') {
      this.currentObj.status = 'Active'
    } else if (this.currentObj.status == 'Active') {
      this.currentObj.status = 'Inactive'
    } else if (this.currentObj.status == 'Inactive') {
      this.currentObj.status = 'Active'
    }
    this.currObj.idaSuserId = this.userid
    this.customerUserService.updateStatus(this.currObj).subscribe((result) => {
      this.reponse = result
      this.loading = false
      if (this.reponse.message != null && this.reponse.message != 'undefined') {
        this.errorMessageValue = this.reponse.message
        this.loading = false
        document.getElementById('openModalButton').click()
        return
      }
    })
    this.router.navigate(['customerlist/customeruserlist', this.custid])
  }
  dash() {
    this.router.navigate(['dashboard'])
  }
}
