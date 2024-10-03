import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import {
  SecurityService, SignUpRequest,
  LookupData, SignUpResponse, Customer
} from '../services/services';
import { EventEmitter } from 'events';
import { headernavService } from '../header-nav/header-nav.service';
import { FooterService } from '../footer/footer.service';

@Component({
  selector: 'app-usersignup',
  templateUrl: './usersignup.component.html'
})
export class UsersignupComponent {
  errorMessage: any = "";
  errorMessagetradingName: any = "";
  errorMessageregistrationName: any = "";
  errorMessageregistrationNumber: any = "";
  errorMessagevatNumber: any = "";
  errorMessagetypeOfBusiness: any = "";
  errorMessagetelephoneNumber: any = "";
  errorMessagebillingEmail: any = "";
  errorMessagebillingType: any = "";
  errorMessagebranchLocation: any = "";
  errorMessagephysicalAddress: any = "";
  errorMessagepostalAddress: any = "";
  errorMessagecustOwnIDNumber: any = "";
  errorMessageaccountDeptContactPerson: any = "";
  errorMessageaccountDeptTelephoneNumber: any = "";
  errorMessageaccountDeptFaxNumber: any = "";
  errorMessageaccountDeptEmail: any = "";
  errorMessageauthIDNumber: any = "";
  errorMessageauthPosition: any = "";
  errorMessageauthFirstName: any = "";
  errorMessageauthSurName: any = "";
  errorMessageauthEmail: any = "";
  authCellNumber: any = "";
  billEmailadress: any = "";
  emailid: any = "";
  iDNumber: any = "";
  title: any = "";
  lastName: any = "";
  firstName: any = "";
  contactNumber: any = "";
  

//purpose
  purposeA : boolean = false;
  purposeB: boolean = false;
  purposeC: boolean = false;
  purposeD: boolean = false;
  purposeE: boolean = false;
  purpose: string;

  loading: boolean = false;
  message: any;

  errorMessageValue: any;
  emailPattern: any;
  mobilenumberPattern: any;
  errorEmail: any;
  inp: string = "";
  errorMobile: any;
  stringvalue: string;
  _signupRequest: SignUpRequest = new SignUpRequest();
  _signupResponse: SignUpResponse = new SignUpResponse();
  _business: any;
  public typeOfBusiness: LookupData[];
    lookupList: LookupData[];
  constructor(public router: Router, public securityService: SecurityService,
    public headernavService: headernavService, public footerService: FooterService) {
    this.securityService.getLookupDatas().subscribe((resp) => {
      this.typeOfBusiness = resp;
    });
    this._signupRequest.customer = new Customer();

    this._signupRequest.title = "0";
    this._signupRequest.customer.typeOfBusiness = "0";
    this.headernavService.toggle(false);
    this.footerService.updatefooter(false);
  }
  onKey(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;

    var keynum;
    var keychar;
    var charcheck = /[a-zA-Z0-9]/;
    if (window.event) // IE
    {
      keynum = e.keyCode;
    }
    else {
      if (e.which) // Netscape/Firefox/Opera
      {
        keynum = e.which;
      }
      else return true;
    }

    keychar = String.fromCharCode(keynum);
    return charcheck.test(keychar);
    
  }
  addressonKey(e: any) {
    var str = e.target.value;
    var charCode = (e.which) ? e.which : e.keyCode;
    if (charCode == 8) return true;

    var keynum;
    var keychar;
    var charcheck = /[a-zA-Z0-9,]/;
    if (e.which == 13) {
      var s = $(this).val();
      $(this).val(s + "\n");
    }
    if (window.event) {
      keynum = e.keyCode;
    }
    else {
      if (e.which) {
        keynum = e.which;
      }
      else return true;
    }

    keychar = String.fromCharCode(keynum);
    return charcheck.test(keychar);

  }
  savesignup() {

    this.loading = true;

    $('#test1').css("border-color", "#bbbbbb");
    $('#test2').css("border-color", "#bbbbbb");
    $('#test3').css("border-color", "#bbbbbb");
    $('#test4').css("border-color", "#bbbbbb");
    $('#test5').css("border-color", "#bbbbbb");
    $('#test6').css("border-color", "#bbbbbb");
    $('#test7').css("border-color", "#bbbbbb");
    $('#test8').css("border-color", "#bbbbbb");
    $('#test9').css("border-color", "#bbbbbb");
    $('#test10').css("border-color", "#bbbbbb");
    $('#test11').css("border-color", "#bbbbbb");
    $('#test12').css("border-color", "#bbbbbb");
    $('#test13').css("border-color", "#bbbbbb");
    $('#test14').css("border-color", "#bbbbbb");
    $('#test15').css("border-color", "#bbbbbb");
    $('#test16').css("border-color", "#bbbbbb");
    $('#test17').css("border-color", "#bbbbbb");
    $('#test18').css("border-color", "#bbbbbb");
    $('#test19').css("border-color", "#bbbbbb");
    $('#test20').css("border-color", "#bbbbbb");
    $('#test21').css("border-color", "#bbbbbb");
    $('#test22').css("border-color", "#bbbbbb");
    $('#test23').css("border-color", "#bbbbbb");
    $('#test24').css("border-color", "#bbbbbb");
    $('#test25').css("border-color", "#bbbbbb");
    $('#test26').css("border-color", "#bbbbbb");
    $('#test27').css("border-color", "#bbbbbb");
    $('#test28').css("border-color", "#bbbbbb");
    $('#test29').css("border-color", "#bbbbbb");


    this.errorMessage = "";
    this.errorMessagetradingName = "";
    this.errorMessageregistrationName = "";
    this.errorMessageregistrationNumber = "";
    this.errorMessagevatNumber = "";
    this.errorMessagetypeOfBusiness = "";
    this.errorMessagetelephoneNumber = "";
    this.errorMessagebillingEmail = "";
    this.errorMessagebillingType = "";
    this.errorMessagebranchLocation = "";
    this.errorMessagephysicalAddress = "";
    this.errorMessagepostalAddress = "";
    this.errorMessagecustOwnIDNumber = "";
    this.errorMessageaccountDeptContactPerson = "";
    this.errorMessageaccountDeptTelephoneNumber = "";
    this.errorMessageaccountDeptFaxNumber = "";
    this.errorMessageaccountDeptEmail = "";
    this.errorMessageauthIDNumber = "";
    this.errorMessageauthPosition = "";
    this.errorMessageauthFirstName = "";
    this.errorMessageauthSurName = "";
    this.errorMessageauthEmail = "";
    this.authCellNumber = "";
    this.billEmailadress = "";
    this.emailid = "";
    this.iDNumber = "";
    this.title = "";
    this.lastName = "";
    this.firstName = "";
    this.contactNumber = "";

    this.errorMessageValue = "";
    this.errorMessage = false;
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/;

    //Company Information
    if (isNullOrUndefined(this._signupRequest.customer.tradingName)
      || this._signupRequest.customer.tradingName == "") {
      this.loading = false;
      this.errorMessagetradingName = "Trading Name is required";
      $('#test1').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.registrationName)
      || this._signupRequest.customer.registrationName == "") {
      this.loading = false;
      this.errorMessageregistrationName = "Registration Name is required";
      $('#test2').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.registrationNumber)
      || this._signupRequest.customer.registrationNumber == "") {
      this.loading = false;
      this.errorMessageregistrationNumber = "Registration Number is required";
      $('#test3').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.typeOfBusiness)
      || this._signupRequest.customer.typeOfBusiness == "0") {
      this.loading = false;
      this.errorMessagetypeOfBusiness = "Please Select Type of Business";
      $('#test5').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if ((isNullOrUndefined(this._signupRequest.customer.telephoneNumber)
      || this._signupRequest.customer.telephoneNumber == ""
      || this._signupRequest.customer.telephoneNumber.length != 10)) {
      this.loading = false;
      this.errorMessagetelephoneNumber = "Enter valid customer telephone number";
      $('#test6').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if ((isNullOrUndefined(this._signupRequest.customer.billingEmail)
      || this._signupRequest.customer.billingEmail == "")
      || (!this._signupRequest.customer.billingEmail.match(this.emailPattern))) {
      this.loading = false;
      this.errorMessagebillingEmail = "Customer billing email is invalid";
      $('#test7').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if ((isNullOrUndefined(this._signupRequest.customer.billingType)
      || this._signupRequest.customer.billingType == "")) {
      this.loading = false;
      this.errorMessagebillingType = "Customer billing type is required";
      $('#test8').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if ((isNullOrUndefined(this._signupRequest.customer.branchLocation)
      || this._signupRequest.customer.branchLocation == "")) {
      this.loading = false;
      this.errorMessagebranchLocation = "Customer branch location is required";
      $('#test9').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

  

    else if (isNullOrUndefined(this._signupRequest.customer.postalAddress)
      || this._signupRequest.customer.postalAddress == "") {
      this.loading = false;
      this.errorMessagepostalAddress = "Postal Address is required";
      $('#test11').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.custOwnIDNumber)
      || this._signupRequest.customer.custOwnIDNumber == ""
      || this._signupRequest.customer.custOwnIDNumber.length != 13) {
      this.loading = false;
      this.errorMessagecustOwnIDNumber = "Customer Owner IDNumber 13 digits is required";
      $('#test12').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    else if (isNullOrUndefined(this._signupRequest.customer.physicalAddress)
      || this._signupRequest.customer.physicalAddress == "") {
      this.loading = false;
      this.errorMessagephysicalAddress = "Physical Address is required";
      $('#test10').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    //Company Information


    //Account Department Information
    else if (isNullOrUndefined(this._signupRequest.customer.accountDeptContactPerson)
      || this._signupRequest.customer.accountDeptContactPerson == "") {
      this.loading = false;
      this.errorMessageaccountDeptContactPerson = "Account contact person is required";
      $('#test13').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.accountDeptTelephoneNumber)
      || this._signupRequest.customer.accountDeptTelephoneNumber == ""
      || this._signupRequest.customer.accountDeptTelephoneNumber.length != 10) {
      this.loading = false;
      this.errorMessageaccountDeptTelephoneNumber = "Enter valid account telephone number";
      $('#test14').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.accountDeptFaxNumber)
      || this._signupRequest.customer.accountDeptFaxNumber == "") {
      this.loading = false;
      this.errorMessageaccountDeptFaxNumber = "Account fax number is required";
      $('#test15').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.accountDeptEmail)
      || this._signupRequest.customer.accountDeptEmail == ""
      || (!this._signupRequest.customer.accountDeptEmail.match(this.emailPattern))) {
      this.loading = false;
      this.errorMessageaccountDeptEmail = "Account email Id is inavlid";
      $('#test16').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    //Account Department Information


    //Authentication Information
    else if (isNullOrUndefined(this._signupRequest.customer.authIDNumber)
      || this._signupRequest.customer.authIDNumber == ""
      || this._signupRequest.customer.authIDNumber.length != 13) {
      this.loading = false;
      this.errorMessageauthIDNumber = "Authentication ID number 13 digits is required";
      $('#test17').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.authPosition)
      || this._signupRequest.customer.authPosition == "") {
      this.loading = false;
      this.errorMessageauthPosition = "Authentication position is required";
      $('#test18').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.authFirstName)
      || this._signupRequest.customer.authFirstName == "") {
      this.loading = false;
      this.errorMessageauthFirstName = "Authentication first name is required";
      $('#test19').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.authSurName)
      || this._signupRequest.customer.authSurName == "") {
      this.loading = false;
      this.errorMessageauthSurName = "Authentication surame is required";
      $('#test20').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.authEmail)
      || this._signupRequest.customer.authEmail == "" ||
      !this._signupRequest.customer.authEmail.match(this.emailPattern)) {
      this.loading = false;
      this.errorMessageauthEmail = "Authentication valid email is required";
      $('#test21').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

    else if (isNullOrUndefined(this._signupRequest.customer.authCellNumber)
      || this._signupRequest.customer.authCellNumber == ""
      || this._signupRequest.customer.authCellNumber.length != 10) {
      this.loading = false;
      this.authCellNumber = "Please enter valid authentication cell number";
      $('#test22').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    //Authentication Information
      //Personal Information
    else if (isNullOrUndefined(this._signupRequest.firstName)
      || this._signupRequest.firstName == "") {
      this.loading = false;
      this.firstName = "First Name is required";
      $('#test23').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    else if (isNullOrUndefined(this._signupRequest.lastName)
      || this._signupRequest.lastName == "") {
      this.loading = false;
      this.lastName = "Last Name is required";
      $('#test24').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    else if (isNullOrUndefined(this._signupRequest.title)
      || this._signupRequest.title == "0") {
      this.loading = false;
      this.title = "Please Select Title";
      $('#test25').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }

   
    else if ((isNullOrUndefined(this._signupRequest.emailid)
      || this._signupRequest.emailid == "")
      || (!this._signupRequest.emailid.match(this.emailPattern))) {
      this.loading = false;
      this.emailid = "Please Enter Valid Email Id";
      $('#test27').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    
    else if (this._signupRequest.billEmailadress != undefined) {
      if (!this._signupRequest.billEmailadress.match(this.emailPattern)) {
        this.loading = false;
        this.billEmailadress = "Please Enter Valid Billing Email Id";
        $('#test28').css("border-color", "rgb(201, 76, 76)");
        this.errorMessage = true;
      }
    }
    else if (isNullOrUndefined(this._signupRequest.contactNumber)
      || this._signupRequest.contactNumber == "") {
      this.loading = false;
      this.contactNumber = "Please enter valid personal admin contact number";
      $('#test29').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    else if (this._signupRequest.iDNumber != undefined) {
      if (this._signupRequest.iDNumber.length != 13) {
        this.loading = false;
        this.iDNumber = "Admin IDNumber should be of 13 digit";
        $('#test26').css("border-color", "rgb(201, 76, 76)");
        this.errorMessage = true;
      }
    }

    else if (isNullOrUndefined(this._signupRequest.iDNumber)
      || this._signupRequest.iDNumber == ""
      || this._signupRequest.iDNumber.length != 13) {
      this.loading = false;
      this.iDNumber = "Admin IDNumber 13 digits is required";
      $('#test26').css("border-color", "rgb(201, 76, 76)");
      this.errorMessage = true;
    }
    //Personal Information    

 
    if (this.errorMessage == false)
    this.purpose = "";
    if (this.purposeA == true)
        this.purpose = "a";
    if (this.purposeB == true) {
      if (this.purpose != null && this.purpose != "undefined")
        this.purpose = this.purpose + "," + "b";
      else 
        this.purpose = "b";
    }
    if (this.purposeC == true) {
      if (this.purpose != null && this.purpose != "undefined")
        this.purpose = this.purpose + "," + "c";
      else
        this.purpose = "c";
    }
    if (this.purposeD == true) {
      if (this.purpose != null && this.purpose != "undefined")
        this.purpose = this.purpose + "," + "d";
      else
        this.purpose = "d";
    }
    if (this.purposeE == true) {
      if (this.purpose != null && this.purpose != "undefined")
        this.purpose = this.purpose + "," + "e";
      else
        this.purpose = "e";
    }

    this._signupRequest.customer.purpose = this.purpose;
    this.securityService.registrationNumberVerify(this._signupRequest).subscribe((result) => {
      if (!result.isSucsess) {
        if (result.message != null && result.message != "undefined")
          this.message = result.message;
        this.loading = false;
        document.getElementById('error').click();
        return;
      }
      else
        this.securityService.getDsa(this.inp).subscribe((result) => {
          if (!result.isSuccess) {
            if (result.message != null && result.message != "undefined")
              this.message = result.message;
            this.loading = false;
            document.getElementById('error').click();
            return;
          }
          this.loading = false;
          this._signupRequest.dsaVersion = result.version.toString();
          localStorage.setItem('dsafiles', result.fileName);
          debugger;
          this._signupRequest.displayBusinessName = this.typeOfBusiness.find(t => t.value == this._signupRequest.customer.typeOfBusiness).text;
          localStorage.setItem('signupdetails', JSON.stringify(this._signupRequest));
          this.router.navigate(['/dsa']);
        });
    });
  }
  cancel() {
    var origin = window.location.origin;
    window.location.href = origin + "/Home";
  }
}
