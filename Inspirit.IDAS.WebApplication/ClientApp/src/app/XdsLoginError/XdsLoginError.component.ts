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
import { Tree } from '@angular/router/src/utils/tree';


declare const grecaptcha: any
declare const window: any

@Component({
  selector: 'xds-login-error',
  templateUrl: './XdsLoginError.component.html',
})
export class XdsLoginErrorComponent implements OnInit {
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
    this._loginRequest = new LoginRequest()
    this._loginRespose = new LoginReponse()
    this._loginRespose.errorMessage = ''

  }

  ngOnInit() {
    console.log('in the path');
    
  }
  
}
