import {
  Component,
  Inject,
  Injectable,
  Output,
  OnInit,
  ElementRef,
} from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router, ActivatedRoute } from '@angular/router'
import { debug, isNullOrUndefined } from 'util'
import {
  SecurityService,
  XdsLoginRequest,
  LoginReponse,
  TracingService,
} from '../services/services'
import { HeaderNavComponent } from '../header-nav/header-nav.component'
import 'rxjs/add/operator/map'
import { Observable } from 'rxjs/Observable'
import { EventEmitter } from 'events'
import { headernavService } from '../header-nav/header-nav.service'
import { debounce } from 'rxjs/operator/debounce'
import { FooterService } from '../footer/footer.service'
import { environment } from '../../environments/environment'
import { RecaptchaService } from '../services/recaptcha.service'
import { waitForMap } from '@angular/router/src/utils/collection';
import { Tree } from '@angular/router/src/utils/tree';


declare const grecaptcha: any
declare const window: any

@Component({
  selector: 'xds-login',
  templateUrl: './XdsLogin.component.html',
})
export class XdsLoginComponent implements OnInit {
  _loginRequest: XdsLoginRequest = new XdsLoginRequest()
  _loginRespose: LoginReponse = new LoginReponse()
  points: any
  public userName: string
  Client_Logo: any
  failure: boolean = false
  isidasuser: boolean = false
  loading: boolean = false
  LastPasswordResetDate: any
  _userID: string
  btn: any
  captcharesponse: any
  tokenData: string
  recapScore: any
  recapAction: any
  recapIsSuccess: boolean
  companyName: any
  cName: string
  token: string
  sub: any;
  // randomNumOne: number
  // randomNumTwo: number
  // resultAnswer: number

  constructor(
    public router: Router,
    public route: ActivatedRoute,
    public securityService: SecurityService,
    public headernavService: headernavService,
    public tracingService: TracingService,
    public footerService: FooterService,
    public elementRef: ElementRef,
    public recaptchaService: RecaptchaService,
  ) {
    this.headernavService.toggle(false)
    this.footerService.updatefooter(false)
    this._loginRequest = new XdsLoginRequest()
    this._loginRespose = new LoginReponse()
    this._loginRespose.errorMessage = ''

  }

  ngOnInit() {
    console.log('in the path');
    this.sub = this.route.params.subscribe(params => {
      this.token = params['token'];
      console.log("initialised");
      console.log('token : ' + this.token);
      localStorage.setItem('token', this.token);
      //can call XDSLOGIN same as login call in login() function of component
      this.login();
    });
    
  }
  // krishna
  login() {
    console.log("login called")
    var Id = ''
    var tokenData2 = ''
    this.loading = true
    this._loginRespose.errorMessage = null
    //var element = <HTMLInputElement>document.getElementById('login_button')
    //element.disabled = true

    this._loginRespose.errorMessage = 'Login Processing...'
    var origin = window.location.origin
    //this.getToken()
    if (
      this.token == null ||
      this.token == 'undefined' 
    ) {
      this._loginRespose.isSucsess = false
      this.loading = false
      this._loginRespose.errorMessage = 'Token is required'
      window.location.href = origin + '/xdsloginerror'

      //var element = <HTMLInputElement>document.getElementById('login_button')
      //element.disabled = false
    } else {
      console.log("service called")
      this.securityService.Xdslogin(this.token).subscribe((result) => {
      //this.securityService.Xdslogin(this._loginRequest).subscribe((result) => {
        this._loginRespose = result
        if (this._loginRespose.isSucsess) {
          this.userName = this._loginRespose.userName
          this.headernavService.updateUserName(this.userName)

          // alert(localStorage.getItem('username'))
          //alert(this._loginRespose.client_logo)


          //console.log(this._loginRespose.userIpaddress)
          //user mail id
          //localStorage.setItem('username', this._loginRequest.userName)

          //sessionStorage.setItem('username1', this._loginRequest.userName)
          //user guid
          localStorage.setItem('userid', this._loginRespose.userID)
          // krishna verification pending start
          localStorage.setItem('username', this._loginRespose.userName)

          sessionStorage.setItem('username1', this._loginRespose.userName)
          // krishna verification pending end
          Id = this._loginRespose.userID
          //company guid
          localStorage.setItem('customerid', this._loginRespose.customerID)
          //company Name
          localStorage.setItem('company', this._loginRespose.company)


          //logo
          // localStorage.setItem(key, value)
          localStorage.setItem('client_logo', this._loginRespose.client_logo)


          //user name
          localStorage.setItem('name', this._loginRespose.userName)



          //to verify trailuser
          if (this._loginRespose.isTrialuser == true) {
            localStorage.setItem('trailuser', 'YES')
          } else {
            localStorage.setItem('trailuser', 'NO')
          }
          this.headernavService.updateloader(true)
          //to know the idas admin user
          if (this._loginRespose.isIdasUser == true)
            localStorage.setItem('idasuser', 'YES')
          else localStorage.setItem('idasuser', 'NO')

          // krishna start
          // we can directly set localStorage.setItem('isXDS', 'NO')
          if (this._loginRespose.isXDS == true)
            localStorage.setItem('isXDS', 'YES')
          else localStorage.setItem('isXDS', 'NO')

          // krishna end


          this.loading = false

          window.location.href = origin + '/dashboard'

          
        } else if (
          this._loginRespose.errorMessage ==
          'Found another active session, kindly try after 5 minutes or contact administrator.'
        ) {
          //this.loading = false
          //document.getElementById('loginerror').click()
          //var element = <HTMLInputElement>(
          //  document.getElementById('login_button')
          //)
          //element.disabled = false
          window.location.href = origin + '/xdsloginerror'
        } else {
          window.location.href = origin + '/xdsloginerror'
          //this.loading = false
          //var element = <HTMLInputElement>(
          //  document.getElementById('login_button')
          //)
          //element.disabled = false
        }
      })
    }
  }


  ngOnDestroy() {
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor =
      '#f2f3f8'
  }
  validation() {
    this._loginRespose.errorMessage = null
  }
  
}
