import { Component, Inject , OnDestroy} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SecurityService, LoginRequest, LoginReponse } from '../services/services';
import { debug, isNullOrUndefined } from 'util';
import { headernavService } from '../header-nav/header-nav.service';
import { FooterService } from '../footer/footer.service';

@Component({
  selector: 'app-forgetpassword',
  templateUrl: './forgetpassword.component.html'
})
export class ForgetPasswordComponent {
  emailPattern: any;
  public _forgetRequest: LoginRequest;
  public _forgetRespose: LoginReponse;
  keypress: boolean = false;
  loading: boolean = false;
  constructor(public router: Router, public securityService: SecurityService, public headernavService: headernavService, public footerService: FooterService) {
    this._forgetRequest = new LoginRequest();
    this._forgetRequest.password = "samplepassword";
    this._forgetRespose = new LoginReponse();
    this._forgetRespose.isSucsess = true;
    this._forgetRespose.errorMessage = "Please enter existing username";
    this.headernavService.toggle(false);
    this.footerService.updatefooter(false);
  }
  code()
  {
    this.loading = true;
    if (this.keypress == false) {
      this.keypress = true;
      if (isNullOrUndefined(this._forgetRequest.userName) || this._forgetRequest.userName == "") {
        this.loading = false;
        this._forgetRespose.isSucsess = false;
        this.keypress = false;
        return false;
      }
      this.securityService.forgotPassword(this._forgetRequest.userName).subscribe((result) => {
        this._forgetRespose = result;
        this.loading = false;
        if (this._forgetRespose.isSucsess) {
          document.getElementById('openModalButton').click();
          this.loading = false;
        }
        else {
          this.keypress = false;
        }
      });
    }
  }
  ok() {
    this.router.navigate(['/login']);
  }
}
