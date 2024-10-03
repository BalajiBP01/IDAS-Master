import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CommercialAuditorVm, AuditorAddressVM } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'app-commercialAuditorDetail',
  templateUrl: './commercialAuditorDetail.component.html'
})
export class CommercialAuditorDetailComponent implements OnInit {
  dtOptions: DataTables.Settings = {};
  auditor: CommercialAuditorVm;
  auditorAddress: AuditorAddressVM[];
  constructor(public _myModal: NgbActiveModal, public headerservice: headernavService) {

  }
  ngOnInit() {
    this.headerservice.toggle(true);
    this.dtOptions = {
      pagingType: 'full_numbers',
      order: [2, "desc"],
      language: {
        search: "Filter:"
      },
    };
    this.auditor.auditoraddresess = this.auditorAddress;

  }
  close() {
    this._myModal.dismiss();
  }
}
