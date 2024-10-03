import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import { EventEmitter } from 'events';
import { SecurityService, DonotCallRegistryRequest, DonoCallRegistryResponse } from '../services/services';
import { headernavService } from '../header-nav/header-nav.service';
import { FooterService } from '../footer/footer.service';


@Component({
  selector: 'donotcallregistry',
  templateUrl: './donotcallregistry.component.html'
})

export class DonotCallRegistryComponent {
  errorMessage: any;
  errorMessageValue: any;
  emailPattern: any;
  errorEmail: any;
  isTrue: false;
  message: any;


  _donotcallregistryrequest: DonotCallRegistryRequest;
  _donotcallregisstryresponse: DonoCallRegistryResponse;
  constructor(public router: Router, public callregistryservice: SecurityService, public headernavService: headernavService, public footerService: FooterService) {
    this._donotcallregistryrequest = new DonotCallRegistryRequest();
    this.headernavService.toggle(false);
    this.footerService.updatefooter(false);
  }

  send() {
    this.errorMessageValue = "";
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,10})+$/;
    if (this._donotcallregistryrequest.iDNumber != undefined) {
      if (this._donotcallregistryrequest.iDNumber.length != 13) {
        this.errorMessageValue = " ID Number should be of 13 digit";
      }
    }
    if (isNullOrUndefined(this._donotcallregistryrequest.iDNumber) || this._donotcallregistryrequest.iDNumber == "") {
      this.errorMessageValue = " ID Number is required";
    }
    if ((isNullOrUndefined(this._donotcallregistryrequest.surname) || this._donotcallregistryrequest.surname == "")) {
      this.errorMessageValue = "Enter Name and Surname";
    }
    if (isNullOrUndefined(this._donotcallregistryrequest.phonenumber) || this._donotcallregistryrequest.phonenumber == "") {
      this.errorMessageValue = "Phone Number is required";
    }
    if (this._donotcallregistryrequest.phonenumber != undefined) {
      if (this._donotcallregistryrequest.phonenumber.length != 10) {
        this.errorMessageValue = " Please Enter Valid Telephone Number ";
      }
      if (this._donotcallregistryrequest.emailaddress != undefined) {
        if (!this._donotcallregistryrequest.emailaddress.match(this.emailPattern)) {
          this.errorMessageValue = "Please Enter Valid Email Id";
        }
      }
      }
    if ((isNullOrUndefined(this._donotcallregistryrequest.emailaddress) || this._donotcallregistryrequest.emailaddress == "") || (!this._donotcallregistryrequest.emailaddress.match(this.emailPattern))) {
      { this.errorMessageValue = "Please Enter  Email Address"; }
    }
    if (this.errorMessageValue != "") { this.errorMessage = true;  return false; }    

    this.callregistryservice.callregistry(this._donotcallregistryrequest).subscribe((result) => {
      this._donotcallregisstryresponse = result;
      if (this._donotcallregisstryresponse.isSucsess) {
        this._donotcallregistryrequest = new DonotCallRegistryRequest();
        if (this._donotcallregisstryresponse.errorMessage != null && this._donotcallregisstryresponse.errorMessage != "undefined") {
          this.message = this._donotcallregisstryresponse.errorMessage;
          document.getElementById('home').click();
        }
      }
      else {
        if (this._donotcallregisstryresponse.errorMessage != null && this._donotcallregisstryresponse.errorMessage != "undefined") {
          this.message = this._donotcallregisstryresponse.errorMessage;
          document.getElementById('home').click();
        }
      }
    });
  }

  home() {
    var origin = window.location.origin;
    window.location.href = origin + "/Home";
  }
}
  


