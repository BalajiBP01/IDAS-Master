import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import {
  SecurityService, SignUpRequest,
  SignUpResponse, LookupData
} from '../services/services';
import { DatePipe } from '@angular/common';
import { headernavService } from '../header-nav/header-nav.service';
import { FooterService } from '../footer/footer.service';
import * as html2canvas from 'html2canvas';
import { EventEmitter } from 'events';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'dsa',
  templateUrl: './dsa.component.html'
})
export class DsaComponent implements OnInit, OnDestroy {
  public sub: any;
  loading: boolean = false;
  lookupList: LookupData[];
  _signupRequest: SignUpRequest = new SignUpRequest();
  _signupResponse: SignUpResponse = new SignUpResponse();
  description: string;
  isAgreed: boolean = false;
  emailPattern: any;
  typeOfBusiness: string;
  datetime: string;

  purposeA: boolean = false;
  purposeB: boolean = false;
  purposeC: boolean = false;
  purposeD: boolean = false;
  purposeE: boolean = false;
  purpose: string;

  message: any;
  home: boolean = false;

  constructor(public router: Router, public headernavService: headernavService,
    public securityService: SecurityService, public route: ActivatedRoute, public footerService: FooterService, public datePipe: DatePipe) {
    this.headernavService.toggle(false);
    this.footerService.updatefooter(false);
  }
  ngOnInit(): void {
    var signUpDetails = JSON.parse(localStorage.getItem('signupdetails'));
   
    this._signupRequest = signUpDetails;
    this.purpose = this._signupRequest.customer.purpose;
    if (this.purpose != null && this.purpose != "undefined") {
      if (this.purpose.includes("a"))
        this.purposeA = true;
      if (this.purpose.includes("b"))
        this.purposeB = true;
      if (this.purpose.includes("c"))
        this.purposeC = true;
      if (this.purpose.includes("d"))
        this.purposeD = true;
      if (this.purpose.includes("e"))
        this.purposeE = true;
    }
    this.datetime = this.datePipe.transform(new Date(), 'yyyy-MM-dd h:mm:ss');

    this._signupRequest.toMail = this._signupRequest.customer.authEmail;
    this.securityService.getDsa("").subscribe((response) => {
      this.description = response.description;
    });


    this.generateBase64String();
  }
  accept() {    
    this.emailPattern = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
    if (!this._signupRequest.toMail.match(this.emailPattern)
      || isNullOrUndefined(this._signupRequest.toMail)
      || this._signupRequest.toMail == "") {
      this.message = 'Invalid Email';
      document.getElementById('error').click();
      return;
    }
    this.loading = true;    
    this.loading = true;
    this.securityService.signUp(this._signupRequest).subscribe((result) => {
      this._signupResponse = result;
      this.loading = false;
      if (this._signupResponse.isSucsess) {
        if (this._signupResponse.message != null && this._signupResponse.message != "undefined") {
          this.home = true;
          this.message = this._signupResponse.message;
          document.getElementById('error').click();
        }
      }
      else {
        if (this._signupResponse.message != null && this._signupResponse.message != "undefined")
          this.message = this._signupResponse.message;
        document.getElementById('error').click();
      }
    });
  }
  generateBase64String() {
    this.loading = true;
    let arrHTMLElement: string[] = ["0", "1", "2", "3"];
    this._signupRequest.base64Array = new Array<string>(3);
    this._signupRequest.floats = [6]
    this._signupRequest.floats[0] = 0;//left
    this._signupRequest.floats[1] = 5;//right
    this._signupRequest.floats[2] = 5;//top
    this._signupRequest.floats[3] = 0;//bottom
    this._signupRequest.floats[4] = 600;//width
    this._signupRequest.floats[5] = 950;//height
    arrHTMLElement.forEach((item, index) => {

      var CustomerDetails = document.getElementById('CustomerDetails' + item);
      html2canvas(CustomerDetails, {
        onclone: function (document) {
          document.querySelector("#CustomerDetails" + item).style.display = "block";
        }
      }).then(canvas => {
        this._signupRequest.base64Array[index] = canvas.toDataURL().split(',')[1];
        this._signupRequest.htmlString = this.description;
        if (item == "3") {
          this.loading = false;
        }
      });

    });
  }
  homepath() {
    var origin = window.location.origin;
    window.location.href = origin + "/Home";
  }
  ngOnDestroy() {
    localStorage.removeItem('signupdetails');
  }
}
