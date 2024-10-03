import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Consumerdebtreview } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'consumerdebtdetail',
  templateUrl: './consumerdebtdetail.component.html'
})
export class ConsumerDebtDetailComponent {
  sales: Consumerdebtreview;
  constructor(public _myModel: NgbActiveModal, public headerservice: headernavService) {
    this.headerservice.toggle(true);
  }
  close() {
    this._myModel.dismiss();
  }
}
