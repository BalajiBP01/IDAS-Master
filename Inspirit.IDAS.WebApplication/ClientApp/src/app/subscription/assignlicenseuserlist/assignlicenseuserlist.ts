import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { SubscriptionService, SubscriptionLicenceVm, SubscriptionLicenceRequest } from '../../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'assignlicenseuserlist',
  templateUrl: './assignlicenseuserlist.html'
})
export class SubscriptionLicenceUserListComponent {
  users: SubscriptionLicenceVm[];
  asignedUsers: SubscriptionLicenceRequest = new SubscriptionLicenceRequest();
  isCheck: boolean = false;
  constructor(public router: Router, public _myModel: NgbActiveModal,
    public subscriptionservice: SubscriptionService) {

  }
  close() {
    this._myModel.close();
  }
  save() {
    this.asignedUsers.subscriptionLicenceVms = this.users;
    this.subscriptionservice.assignLicensetoUsers(this.asignedUsers).subscribe((result) => {
      if (result != "") { alert(result); }
      else {
        this.router.navigate(['Subscriptionsassignedusers']);
        this._myModel.close();
      }
    });
  }
}
