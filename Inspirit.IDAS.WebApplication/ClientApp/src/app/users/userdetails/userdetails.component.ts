import { Component, OnInit, OnDestroy } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { ActivatedRoute } from '@angular/router'
import {
  CustomerUser,
  UserService,
  CrudUserResponse,
} from '../../services/services'
import { headernavService } from '../../header-nav/header-nav.service'

@Component({
  selector: 'app-userdetails',
  templateUrl: './userdetails.component.html',
})
export class userdetailsComponent implements OnInit, OnDestroy {
  currentObj: CustomerUser = new CustomerUser()
  id: any
  private sub: any
  mode: string = 'view'
  reponse: CrudUserResponse
  readonly: boolean = false
  customerId: any
  customerUserId: any
  name: any
  isuserExists: any
  loading: boolean

  constructor(
    public router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    public headernavService: headernavService,
  ) {}
  ngOnInit(): void {
    this.loading = true

    this.isuserExists = localStorage.getItem('userid')
    if (this.isuserExists != null && this.isuserExists != 'undefined') {
      this.headernavService.toggle(true)
      this.name = localStorage.getItem('name')
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name)
      }

      this.customerId = localStorage.getItem('customerid')

      this.sub = this.route.queryParams.subscribe((params) => {
        this.id = params['id']
      })

      if (typeof this.id == 'undefined' || typeof this.id == null) {
        this.mode = 'add'

        this.currentObj = new CustomerUser()
        this.readonly = false
      } else {
        this.mode = 'view'
        this.userService.view(this.id).subscribe((result) => {
          this.currentObj = result
          this.readonly = true
          this.loading = false
        })
      }
    } else {
      this.router.navigate(['/login'])
    }
  }

  ngOnDestroy() {
    this.sub.unsubscribe()
  }

  save() {
    if (this.mode == 'add') {
      this.currentObj.customerId = this.customerId
      this.userService.insert(this.currentObj).subscribe((result) => {
        this.reponse = result
      })
      this.router.navigate(['userlist'])
    } else if (this.mode == 'edit') {
      {
        this.userService.update(this.currentObj).subscribe((result) => {
          this.reponse = result
        })
        this.router.navigate(['userlist'])
      }
    }
  }
  edit() {
    this.mode = 'edit'
    this.readonly = false
    this.userService.view(this.id).subscribe((result) => {
      this.currentObj = result
    })
  }
  delete() {
    this.userService.delete(this.id).subscribe((result) => {
      this.router.navigate(['userlist'])
    })
  }

  list() {
    this.router.navigate(['userlist'])
  }
  changeStatus() {
    if (this.currentObj.status == 'Pending') {
      this.currentObj.status = 'Active'
    } else if (this.currentObj.status == 'Active') {
      this.currentObj.status = 'Inactive'
    } else if (this.currentObj.status == 'Inactive') {
      this.currentObj.status = 'Active'
    }
    this.router.navigate(['userlist'])
  }
}
