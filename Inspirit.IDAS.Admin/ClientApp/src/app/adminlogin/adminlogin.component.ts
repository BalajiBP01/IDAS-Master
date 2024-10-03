import { Component, Inject, Injectable, Output, ElementRef, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import { SecurityService, LoginRequest, LoginReponse } from '../services/services';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import { HeaderService } from '../header-nav/headerService';
import 'rxjs/add/operator/map';
import { Observable } from 'rxjs/Observable';
import { EventEmitter } from 'events';

@Component({
    selector: 'app-adminlogin',
    templateUrl: './adminlogin.component.html'
})
export class AdminLoginComponent implements OnInit {

    points: any;
    public userName: string;
    _request: LoginRequest = new LoginRequest();
    _response: LoginReponse = new LoginReponse();
   
    
    errorMessage: string = "";
    loading : boolean = false;

  constructor(public router: Router, public securityService: SecurityService, public asideNavService: AsideNavService, public headerService: HeaderService, public elementRef: ElementRef) {
    
      this.asideNavService.toggle(false);
        this.headerService.updateheader(false);
    }
  ngOnInit() {
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor = '#242323';
  }
  validation() {
    this._response.isSucsess = true;
    this.errorMessage = null;
  }
  login() {
    var element = <HTMLInputElement>document.getElementById("login_button");
    element.disabled = true;
this.loading = true;
    var origin = window.location.origin;
        this.securityService.login(this._request).subscribe((result) => {
            this._response = result;
            if (this._response.isSucsess) {
                //user ID
                localStorage.setItem('userid', this._response.userid);
                //user Name
                localStorage.setItem('username', this._response.fullName);
                this.loading = false;
              window.location.href = origin + "/dashboard";
            } else {
              this.errorMessage = this._response.errorMessage;
this.loading = false;
              var element = <HTMLInputElement>document.getElementById("login_button");
              element.disabled = false;
            }
        });
  }
  ngOnDestroy() {
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor = '#d9d9d9';
  }
}





