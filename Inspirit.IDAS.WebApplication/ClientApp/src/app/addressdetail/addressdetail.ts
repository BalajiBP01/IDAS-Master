import { Component, Inject, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { AddressDetail } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'app-addressdetail',
  templateUrl: './addressdetail.html'
})
export class AddressDetailComponent implements OnChanges {

  address: AddressDetail;
  constructor(public _myModal: NgbActiveModal,public headerservice: headernavService) {
    this.headerservice.toggle(true);
  }
  ngOnChanges() {
  }
  close()
  {
      this._myModal.dismiss();
  }
}
