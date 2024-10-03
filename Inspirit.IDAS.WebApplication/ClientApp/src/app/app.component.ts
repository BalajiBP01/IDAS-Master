import { Component, Compiler } from '@angular/core'
import { UserIdleService } from 'angular-user-idle'
import { SecurityService } from './services/services'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  isNavVisible: boolean = false
  title = 'app'
  count: any
  customeruserid: any
  loading: boolean = false
  trailuser: any
  istrailusr: boolean = false

  constructor(
    public userIdle: UserIdleService,
    public securityService: SecurityService,
    private _compiler: Compiler,
  ) {
    this._compiler.clearCache()
    this.userIdle.startWatching()
    // Start watching when user idle is starting.
    this.userIdle.onTimerStart().subscribe((count) => {
      this.count = count
    })

    // Start watch when time is up.
    this.userIdle.onTimeout().subscribe(() => {
      this.logout()
    })
  }

  setNavVisibility(isVisible: boolean) {
    this.isNavVisible = isVisible
  }
  logout() {
    this.loading = true
    this.customeruserid = localStorage.getItem('userid')
    this.trailuser = localStorage.getItem('trailuser')
    if (this.trailuser == 'YES') this.istrailusr = true
    else this.istrailusr = false
    this.securityService
      .logout(this.customeruserid, this.istrailusr)
      .subscribe((response) => {
        if (response != null) {
          localStorage.removeItem('username')
          localStorage.removeItem('userid')
          localStorage.removeItem('customerid')
          localStorage.removeItem('trailuser')
          localStorage.removeItem('LastPasswordResetDate')
          localStorage.removeItem('IsRestrictedCustomerUser')
          localStorage.removeItem('IsRestrictedCustomer')
          this.customeruserid = null
          this.loading = false
          var origin = window.location.origin
          window.location.href = origin + '/Home'
        }
      })
  }
}
