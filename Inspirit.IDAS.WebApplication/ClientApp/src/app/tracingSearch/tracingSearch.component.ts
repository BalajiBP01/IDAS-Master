import { Component, Inject, OnInit, OnDestroy } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router, ActivatedRoute } from '@angular/router'
import { debug, isNullOrUndefined } from 'util'
import { FormGroup, FormBuilder, Validators } from '@angular/forms'
import {
  TracingService,
  ConsumerSearchRequest,
  CompanySearchRequest,
  AddressSearchRequest,
  SecurityService,
  LookupData,
} from '../services/services'
import { DatePipe } from '@angular/common'

import { headernavService } from '../header-nav/header-nav.service'
import { debounce } from 'rxjs/operator/debounce'
import { nearer } from 'q'

@Component({
  selector: 'tracingSearch',
  templateUrl: './tracingSearch.component.html',
})
export class TracingSearchComponent implements OnInit {
  private sub: any
  id: any
  profileType = 1
  points: any
  customerid: any
  userid: any
  errormsg: any
  globalSearch: any
  searchTime: string
  name: any
  isuserexists: any
  istrailuser: any
  loading: boolean = false
  IsRestrictedCustomer: any
  IsRestrictedCustomerUser: any
  LastPasswordResetDate: string
  userForm: any
  numOfDays: number
  // krishna start 
  isXDS: any
  userName: any
  // krishna end
  enquiryReasons: LookupData[]
  _tracingRequest: ConsumerSearchRequest = new ConsumerSearchRequest()
  _companyRequest: CompanySearchRequest = new CompanySearchRequest()
  _addressRequest: AddressSearchRequest = new AddressSearchRequest()

  public onPageChange(event) {}
  constructor(
    public router: Router,
    public route: ActivatedRoute,
    public http: HttpClient,
    public tracingService: TracingService,
    public headernavService: headernavService,
    public datePipe: DatePipe,
    public formBuilder: FormBuilder,
    public securityService: SecurityService,
  ) {
    this.customerid = localStorage.getItem('customerid')
    this.userid = localStorage.getItem('userid')
    this.istrailuser = localStorage.getItem('trailuser')
    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end

    // console.log('customerid: ' + this.customerid)
    // console.log('userid: ' + this.userid)
  }

  onKey(e: any) {
    var str = e.target.value 
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /[a-zA-Z0-9]/
    if (window.event) {
      // IE
      keynum = e.keyCode
    } else {
      if (e.which) {
        // Netscape/Firefox/Opera
        keynum = e.which
      } else return true
    }

    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }
  RegNumberonKey(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /[a-zA-Z0-9/]/
    if (window.event) {
      // IE
      keynum = e.keyCode
    } else {
      if (e.which) {
        // Netscape/Firefox/Opera
        keynum = e.which
      } else return true
    }

    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }
  AddressonKey(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /[a-zA-Z0-9, ]/
    if (window.event) {
      // IE
      keynum = e.keyCode
    } else {
      if (e.which) {
        // Netscape/Firefox/Opera
        keynum = e.which
      } else return true
    }

    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }
  isNumberKey(evt) {
    var charCode = evt.which ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57)) return false
    return true
  }
  onlyAlphabets(e: any) {
    var str = e.target.value
    var charCode = e.which ? e.which : e.keyCode
    if (charCode == 8) return true

    var keynum
    var keychar
    var charcheck = /[a-zA-Z- ]/
    if (window.event) {
      // IE
      keynum = e.keyCode
    } else {
      if (e.which) {
        // Netscape/Firefox/Opera
        keynum = e.which
      } else return true
    }

    keychar = String.fromCharCode(keynum)
    return charcheck.test(keychar)
  }

  ngOnInit(): void {
    this.customerid = localStorage.getItem('customerid')
    this.userid = localStorage.getItem('userid')
    this.numOfDays = parseInt(localStorage.getItem('numOfDays'))

    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end


    //Is Restricted Customer
    this.tracingService
      .getIsRestrictedCustomer(this.customerid)
      .subscribe((result) => {
        this.IsRestrictedCustomer = result
        localStorage.setItem('IsRestrictedCustomer', this.IsRestrictedCustomer)
      })

    //Is Restricted Customer User
    this.tracingService
      .getIsRestrictedCustomerUser(this.userid)
      .subscribe((result) => {
        this.IsRestrictedCustomerUser = result
        localStorage.setItem(
          'IsRestrictedCustomerUser',
          this.IsRestrictedCustomerUser,
        )
      })

    this.tracingService.getEnquiryReasonLookupDatas().subscribe((resp) => {
      console.log(resp)
      this.enquiryReasons = resp
    })

    this.tracingService
      .getCustomerEnquiryReason(this.customerid)
      .subscribe((result) => {
        console.log(result)
        this._tracingRequest.enquiryReason = (result === null) ? Number(this.enquiryReasons['0'].value) : result      
        /*this.IsRestrictedCustomerUser = result
        localStorage.setItem(
          'IsRestrictedCustomerUser',
          this.IsRestrictedCustomerUser,
        )*/
      })

    this.userForm = this.formBuilder.group({
      iDNumber: ['', [Validators.required, Validators.minLength(13)]],
    })

    this.loading = true
    this._tracingRequest = new ConsumerSearchRequest()
    this._companyRequest = new CompanySearchRequest()
    this.sub = this.route.queryParams.subscribe((params) => {
      let type = params['type']
      if (type == 'Profile') {
        this.profileType = 1
      } else if (type == 'Company') {
        this.profileType = 2
      } else if (type == 'Address') {
        this.profileType = 3
      }
    })

    this.isuserexists = localStorage.getItem('userid')
    if (this.isuserexists != null && this.isuserexists != 'undefined') {
      this.headernavService.toggle(true)

      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }
      this.customerid = localStorage.getItem('customerid')
      this.userid = localStorage.getItem('userid')

      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          this.loading = false

          this.headernavService.updatePoints(this.points)

        })

      // krishna start
      //if (this.isXDS == 'NO') {
        // existing code 
        
        // existing code
      //}
      //else {
      //  this.points = 100
      //  this.headernavService.updatePoints(this.points)
      //}
      
      // krishna end
    } else {
      this.router.navigate(['/login'])
    }

    // krishna start
    if (this.isXDS == 'NO') {
      // existing code
      if (this.numOfDays > 30) {
        document.getElementById('pwdReset').click()
      }
      // existing code
    }
    // krishna end

  }

  //get validation form
  get getControl() {
    return this.userForm.controls
  }

  // krishna pending
  getpoints() {
    this.loading = true
    //if (this.isXDS == 'NO') {
      this.tracingService
        .getPoints(this.userid, this.customerid)
        .subscribe((result) => {
          this.points = result
          this.loading = false
          this.headernavService.updatePoints(this.points)
        })
    //}
    //else {
    //  this.points = 100
    //  this.headernavService.updatePoints(this.points)
    //}
  }

  tracingRequestByUser(request) {
    if (request.phoneNumber != null) {
      request.phoneNumber = request.phoneNumber
      request.type = 'Profile'
      request.userId = this.userid
      request.custId = this.customerid
      // krishna start
      request.userName = this.userName
      if (this.isXDS == 'YES') request.isXDS = true
      else request.isXDS = false
      // krishna end
      if (this.istrailuser == 'YES') request.isTrailuser = true
      else request.isTrailuser = false
      this.router.navigate(['tracingSearch/consumerSearchResult'], {
        queryParams: request,
        skipLocationChange: true,
      })
    } else {
      request.type = 'Profile'
      request.custId = this.customerid
      // krishna start
      request.userName = this.userName
      if (this.isXDS == 'YES') request.isXDS = true
      else request.isXDS = false
      // krishna end
      if (this.istrailuser == 'YES') request.isTrailuser = true
      else request.isTrailuser = false
      this.router.navigate(['tracingSearch/consumerSearchResult'], {
        queryParams: this._tracingRequest, // issue whether its used pending _tracingRequest values
        skipLocationChange: true,
      })
    }
  }

  profilesearch() {
    if ((this.points != null && this.points != 'undefined' && this.points > 0) || (this.isXDS == 'YES')) {
      if (this._tracingRequest.enquiryReason == null) {
        this.errormsg = 'Please select the Enquiry Reason for search'
      }
      else if (
        (this._tracingRequest.address != null &&
          this._tracingRequest.address != '') ||
        (this._tracingRequest.dateOfBirth != null &&
          this._tracingRequest.dateOfBirth != '') ||
        (this._tracingRequest.firstname != null &&
          this._tracingRequest.firstname != '') ||
        (this._tracingRequest.fromDate != null &&
          this._tracingRequest.fromDate != '') ||
        (this._tracingRequest.iDNumber != null &&
          this._tracingRequest.iDNumber != '') ||
        (this._tracingRequest.passport != null &&
          this._tracingRequest.passport != '') ||
        (this._tracingRequest.phoneNumber != null &&
          this._tracingRequest.phoneNumber != '') ||
        (this._tracingRequest.surname != null &&
          this._tracingRequest.surname != '') ||
        (this._tracingRequest.toDate != null &&
          this._tracingRequest.toDate != '')
      ) {
        console.log(this._tracingRequest.voucherCode);
        console.log(this._tracingRequest.customerRefNum);
        /** Padmavati code  */

        /*IdNumber validation of min 10 digits*/
        if (this._tracingRequest.iDNumber != null &&
          this._tracingRequest.iDNumber != ' ') {
          if (this._tracingRequest.iDNumber.length > 0 && this._tracingRequest.iDNumber.length < 10) {
            this.errormsg = 'ID Number should be min. of 10 digits'
          } else {
            this.tracingRequestByUser(this._tracingRequest);
          }
        }


        /** Surname and DOB are manadatory if passport has been added*/
        else if (this._tracingRequest.passport != null &&
          this._tracingRequest.passport != ' ') {
          var dobval = false;
          var surnamenameval = false;

          if (this._tracingRequest.dateOfBirth == null ||
            this._tracingRequest.dateOfBirth == ' ') {
            this.errormsg = 'Plese enter Date Of Birth'
          } else {
            dobval = true;
          }

          if ((this._tracingRequest.surname == null ||
            this._tracingRequest.surname == ' ')) {
            this.errormsg = 'Plese enter Surname'
          } else {
            surnamenameval = true;
          }

          if (dobval == true && surnamenameval == true) {
            this.tracingRequestByUser(this._tracingRequest);
            //alert('can search');
          }
          return false;
        }

        /*Padmavati code
        * First name and surname are manadatory if DOB has been added*/
        else if (this._tracingRequest.dateOfBirth != null &&
          this._tracingRequest.dateOfBirth != ' ') {
          var firstnameval = false;
          var surnamenameval = false;

          if ((this._tracingRequest.surname == null ||
            this._tracingRequest.surname == ' ')) {
            this.errormsg = 'Plese enter Surname'
          } else {
            surnamenameval = true;
          }

          if (this._tracingRequest.firstname == null ||
            this._tracingRequest.firstname == ' ') {
            this.errormsg = 'Plese enter First Name'
          } else {
            firstnameval = true;
          }

          if (firstnameval == true && surnamenameval == true) {
            alert('can search');
            this.tracingRequestByUser(this._tracingRequest);
          }
          return false;
        }


        /*Padmavati code
        * Surname is manadatory if Address has been added*/
        else if (this._tracingRequest.address != null &&
          this._tracingRequest.address != ' ') {
          var surnamenameval = false;

          if ((this._tracingRequest.surname == null ||
            this._tracingRequest.surname == ' ')) {
            this.errormsg = 'Plese enter Surame'
          } else {
            surnamenameval = true;
          }

          if (surnamenameval == true) {
            alert('can search');
            this.tracingRequestByUser(this._tracingRequest);
          }
          return false;
        }

        else {
          this.tracingRequestByUser(this._tracingRequest);
        }

        /*if (this._tracingRequest.phoneNumber != null) {
          this._tracingRequest.phoneNumber = this._tracingRequest.phoneNumber
          this._tracingRequest.type = 'Profile'
          this._tracingRequest.userId = this.userid
          this._tracingRequest.custId = this.customerid
          if (this.istrailuser == 'YES') this._tracingRequest.isTrailuser = true
          else this._tracingRequest.isTrailuser = false
          this.router.navigate(['tracingSearch/consumerSearchResult'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
        } else {
          this._tracingRequest.type = 'Profile'
          this._tracingRequest.custId = this.customerid
          if (this.istrailuser == 'YES') this._tracingRequest.isTrailuser = true
          else this._tracingRequest.isTrailuser = false
          this.router.navigate(['tracingSearch/consumerSearchResult'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
        }*/
      } else if (
        this.IsRestrictedCustomer == 1 ||
        this.IsRestrictedCustomerUser == 1
      ) {
        // pending passport validation
        if (this._tracingRequest.iDNumber == undefined) {
          this.errormsg = 'Plese enter ID Number/Passport Number'
        }
      } else if (
        this.IsRestrictedCustomer == 0 &&
        this.IsRestrictedCustomerUser == 0
      ) {
        this.errormsg = 'Plese enter any of the search criteria'
      }
    } else {
      this.errormsg = "You don't have credits"
    }
  }


  companysearch() {
    if ((this.points != null && this.points != 'undefined' && this.points > 0)|| (this.isXDS == 'YES')) {
      if (
        (this._companyRequest.companyName != null &&
          this._companyRequest.companyName != ' ') ||
        (this._companyRequest.companyRegNumber != null &&
          this._companyRequest.companyRegNumber != ' ')
      ) {
        this._companyRequest.type = 'Company'
        this._companyRequest.custId = this.customerid

        // krishna start pending
        //_companyRequest.userName = this.userName
        //if (this.isXDS == 'YES') _companyRequest.isXDS = true
        //else _companyRequest.isXDS = false
      // krishna end

        if (this.istrailuser == 'YES') this._companyRequest.isTrailuser = true
        else this._companyRequest.isTrailuser = false
        this.router.navigate(['tracingSearch/commercialSearchResult'], {
          queryParams: this._companyRequest,
          skipLocationChange: true,
        })
      } else {
        this.errormsg = 'Plese enter any of the search criteria'
      }
    } else {
      this.errormsg = "You don't have credits"
    }
  }
  address_search() {
    if ((this.points != null && this.points != 'undefined' && this.points > 0)|| (this.isXDS == 'YES')) {
      if (
        (this._addressRequest.address1 != null &&
          this._addressRequest.address1 != ' ') ||
        (this._addressRequest.address2 != null &&
          this._addressRequest.address2 != ' ') ||
        (this._addressRequest.address3 != null &&
          this._addressRequest.address3 != ' ') ||
        (this._addressRequest.address4 != null &&
          this._addressRequest.address4 != ' ') ||
        (this._addressRequest.postalCode != null &&
          this._addressRequest.postalCode != ' ')
      ) {
        this.router.navigate(['tracingSearch/addressSearchResult'], {
          queryParams: this._addressRequest,
          skipLocationChange: true,
        })
      } else {
        this.errormsg = 'Plese enter any of the search criteria'
      }
    } else {
      this.errormsg = "You don't have credits"
    }
  }
  submit2() {
    alert('test')
  }

  submit(idnumber) {
    var provinceind = 1 ///$(ProvList).val();
    //var a = x.options[x.selectedIndex];
    //alert(x);
    //alert("Test");
    $.ajax({
      url: 'tracingSearchTest/addressSearchResultTest',
      data: { province: provinceind },
      success: function (data) {
        alert(data)
      },
      statusCode: {
        404: function (content) {
          alert('cannot find resource')
        },
        500: function (content) {
          alert('internal server error')
        },
      },
      error: function (req, status, errorObj) {
        // handle status === "timeout"
        // handle other errors
      },
    })

    alert('test complete')
  }
  searchauto() {
    $('#searchCityOfBirth').on('focusout', function () {
      $.ajax({
        url: '/tracingSearch/GetProvinceOfBirth',
        type: 'GET',
        dataType: 'json',
        data: { fetch: $('#searchCityOfBirth').val() },
        success: function (query) {
          $('#searchCityOfBirth').val(query[0])
        },
      })
    })
  }

  // pending globalsearch xds no validation for points
  GlobalSearch() {
    if ((this.points != null && this.points != 'undefined' && this.points > 0) || (this.isXDS == 'YES')) {
      if (
        this.globalSearch != null &&
        this.globalSearch != undefined &&
        this.globalSearch != ' '
      ) {
        if (this.profileType == 1) {
          this._tracingRequest.type = 'Profile'
          this._tracingRequest.userId = this.userid
          this._tracingRequest.custId = this.customerid
          this._tracingRequest.globalSearch = this.globalSearch
          // krishna start
          this._tracingRequest.userName = this.userName
          if (this.isXDS == 'YES') this._tracingRequest.isXDS = true
          else this._tracingRequest.isXDS = false
          // krishna end
          if (this.istrailuser == 'YES') this._tracingRequest.isTrailuser = true
          else this._tracingRequest.isTrailuser = false
          this.router.navigate(['tracingSearch/consumerSearchResult'], {
            queryParams: this._tracingRequest,
            skipLocationChange: true,
          })
        } else {
          this._companyRequest.globalSearch = this.globalSearch
          this._tracingRequest.custId = this.customerid
          this._companyRequest.type = 'Company'

          // krishna start pending
          //this._companyRequest.userName = this.userName
          //if (this.isXDS == 'YES') _companyRequest.isXDS = true
          //else _companyRequest.isXDS = false
          // krishna end

          if (this.istrailuser == 'YES') this._companyRequest.isTrailuser = true
          else this._companyRequest.isTrailuser = false
          // issue code below _companyRequest or _tracingRequest
          this._tracingRequest.custId = this.customerid
          this.router.navigate(['tracingSearch/commercialSearchResult'], {
            queryParams: this._companyRequest,
            skipLocationChange: true,
          })
        }
      }
    } else {
      this.errormsg = "You don't have credits"
    }
  }

  resetPasswordRoute() {
    this.router.navigate(['resetpassword'])
  }
}
