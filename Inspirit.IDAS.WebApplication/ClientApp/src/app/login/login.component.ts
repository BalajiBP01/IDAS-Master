import {
  Component,
  Inject,
  Injectable,
  Output,
  OnInit,
  ElementRef,
} from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { debug, isNullOrUndefined } from 'util'
import {
  SecurityService,
  LoginRequest,
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

declare const grecaptcha: any
declare const window: any

@Component({
  selector: 'login',
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  _loginRequest: LoginRequest = new LoginRequest()
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
  // randomNumOne: number
  // randomNumTwo: number
  // resultAnswer: number

  constructor(
    public router: Router,
    public securityService: SecurityService,
    public headernavService: headernavService,
    public tracingService: TracingService,
    public footerService: FooterService,
    public elementRef: ElementRef,
    public recaptchaService: RecaptchaService,
  ) {
    this.headernavService.toggle(false)
    this.footerService.updatefooter(false)
    this._loginRequest = new LoginRequest()
    this._loginRespose = new LoginReponse()
    this._loginRespose.errorMessage = ''

  }

  ngOnInit() {
   // this.addScriptSrc()
    //this.btn = document.getElementById('btn_recaptcha')
  

    // this.randomNumOne = Math.floor(Math.random() * 10) + 1
    // this.randomNumTwo = Math.floor(Math.random() * 5) + 1
    // this.resultAnswer = this.randomNumOne + this.randomNumTwo

    // console.log('this.randomNumOne: ' + this.randomNumOne)
    // console.log('this.randomNumTwo: ' + this.randomNumTwo)
    // console.log('result = ' + this.resultAnswer)



    this.headernavService.toggle(false)
    this.footerService.updatefooter(false)
    let scrWidth = window.innerWidth
    console.log(scrWidth)
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor =
      '#242323'
  }
  ngOnDestroy() {
    this.elementRef.nativeElement.ownerDocument.body.style.backgroundColor =
      '#f2f3f8'
  }
  validation() {
    this._loginRespose.errorMessage = null
  }
  login() {
    var Id = ''
    var tokenData2 = ''
    this.loading = true
    this._loginRespose.errorMessage = null
    var element = <HTMLInputElement>document.getElementById('login_button')
    element.disabled = true
    
    this._loginRespose.errorMessage = 'Login Processing...'
    var origin = window.location.origin
    //this.getToken()
    if (
      this._loginRequest.userName == null ||
      this._loginRequest.userName == 'undefined' ||
      this._loginRequest.password == null ||
      this._loginRequest.password == 'undefined'
    ) {
      this._loginRespose.isSucsess = false
      this.loading = false
      this._loginRespose.errorMessage = 'UserName and Password is required'
      var element = <HTMLInputElement>document.getElementById('login_button')
      element.disabled = false
    } else {
      this.securityService.login(this._loginRequest).subscribe((result) => {
        this._loginRespose = result
        if (this._loginRespose.isSucsess) {
          this.userName = this._loginRespose.userName
          this.headernavService.updateUserName(this.userName)

          // alert(localStorage.getItem('username'))
          //alert(this._loginRespose.client_logo)


          //console.log(this._loginRespose.userIpaddress)
          //user mail id
          localStorage.setItem('username', this._loginRequest.userName)

          sessionStorage.setItem('username1', this._loginRequest.userName)
          //user guid
          localStorage.setItem('userid', this._loginRespose.userID)
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

          //window.grecaptcha.ready(() => {
          //  //request for recaptcha token
          //  window.grecaptcha
          //    .execute(environment.recaptcha.siteKey, {
          //      action: 'loginpage',
          //    })
          //    .then((token) => {
          //      tokenData2 = token
          //      console.log(token)
          //      this.recaptchaService
          //        .getRecaptchaResponse(tokenData2)
          //        .subscribe((data) => {
          //          console.log(data)
          //          this.recapScore = data['score']
          //          this.recapAction = data['action']
          //          this.recapIsSuccess = data['success']

          //          //this.recapScore = 0.4
          //          if (this.recapScore > 0.5 && this.recapIsSuccess == true) {
          //            if (this._loginRespose.showDSA) {
          //              this.router.navigate(['/dsa'])
          //            } else {
          //              window.location.href = origin + '/dashboard'
          //            }
          //          } else {
          //            this._loginRespose.errorMessage =
          //              'Please Contact Administrator'
          //          }
          //        })

          //    })
          //})
        } else if (
          this._loginRespose.errorMessage ==
          'Found another active session, kindly try after 5 minutes or contact administrator.'
        ) {
          this.loading = false
          document.getElementById('loginerror').click()
          var element = <HTMLInputElement>(
            document.getElementById('login_button')
          )
          element.disabled = false
        } else {
          this.loading = false
          var element = <HTMLInputElement>(
            document.getElementById('login_button')
          )
          element.disabled = false
        }
      })
    }
  }

  //getToken() {
  //  var tokenData2 = ''
  //  window.grecaptcha.ready(() => {
  //    //request for recaptcha token
  //    window.grecaptcha
  //      .execute(environment.recaptcha.siteKey, {
  //        action: 'loginpage',
  //      })
  //      .then((token) => {
  //        tokenData2 = token
  //        this.recaptchaService
  //          .getRecaptchaResponse(tokenData2)
  //          .subscribe((data) => {
  //            //console.log(data)
  //            this.recapScore = data['score']
  //            this.recapAction = data['action']
  //            this.recapIsSuccess = data['success']
  //            console.log('score: ' + this.recapScore)
  //            console.log('action: ' + this.recapAction)
  //            console.log('success: ' + this.recapIsSuccess)
  //          })
  //      })
  //  })
  //}

  //adding recaptcha script
  //addScriptSrc() {
  //  let script: any = document.getElementById('script')
  //  if (!script) {
  //    script = document.createElement('script')
  //  }
  //  const body = document.body
  //  script.innerHTML = ''
  //  script.src = `https://www.google.com/recaptcha/api.js?render=${environment.recaptcha.siteKey}`
  //  script.async = false
  //  script.defer = true
  //  body.appendChild(script)
  //}
}
