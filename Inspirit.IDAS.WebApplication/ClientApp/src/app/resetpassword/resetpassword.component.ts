import { Component, Inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { SecurityService, LoginReponse } from '../services/services'
import { debug, isNullOrUndefined } from 'util'
import { headernavService } from '../header-nav/header-nav.service'

@Component({
  selector: 'app-resetpassword',
  templateUrl: './resetpassword.component.html',
})
export class ResetPasswordComponent {
  email: string
  password: string
  conformpassword: string
  code: string
  errormsg: string = ''
  isSuc: boolean
  username: any
  oldpassword: any
  isuserExists: any
  passwordLength: number
  hasNumber: boolean
  hasUpperCase: boolean
  hasLowerCase: boolean
  hasSpecialChars: boolean

  loginsucc: boolean = false

  _loginresponse: LoginReponse = new LoginReponse()
  constructor(
    public router: Router,
    public securityService: SecurityService,
    public headernavService: headernavService,
  ) {
    this.headernavService.toggle(true)
  }
  ngOnInit() {
    this.passwordLength = this._loginresponse.passwordLength
    this.hasNumber = this._loginresponse.hasNumber
    this.hasUpperCase = this._loginresponse.hasUpperCase
    this.hasLowerCase = this._loginresponse.hasLowerCase
    this.hasSpecialChars = this._loginresponse.hasSpecialChars
  }
  resetPassword() {
    this.isuserExists = localStorage.getItem('userid')
    if (this.isuserExists != null && this.isuserExists != 'undefined') {
      this.code = localStorage.getItem('username')
      if (this.password == this.conformpassword) {
        this.securityService
          .resetPassword(this.code, this.oldpassword, this.password)
          .subscribe((result) => {
            this._loginresponse = result
            if (this._loginresponse.isSucsess) {
              this.loginsucc = true
              // this.errormsg = "Your Password is changed successfully please login with your new password";
              this.errormsg = this._loginresponse.errorMessage
              document.getElementById('error').click()
            } else {
              this.loginsucc = false
              //this.errormsg = 'Reset password failed.'
              this.errormsg = this._loginresponse.errorMessage
              this.passwordLength = this._loginresponse.passwordLength
              this.hasNumber = this._loginresponse.hasNumber
              this.hasUpperCase = this._loginresponse.hasUpperCase
              this.hasLowerCase = this._loginresponse.hasLowerCase
              this.hasSpecialChars = this._loginresponse.hasSpecialChars
              if (this.errormsg.length > 0) {
                document.getElementById('error').click()
              }
            }
          })
      } else {
        this.loginsucc = false
        this.errormsg = "Password and Confirm password didn't match"
        document.getElementById('error').click()
        return
      }
    } else {
      this.router.navigate(['/login'])
    }
  }
  loginpath() {
    this.router.navigate(['/login'])
  }
}
