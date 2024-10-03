import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { DirectorShip, DirectorTelephoneVM, DirectorAddressVm } from '../services/services';
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'app-directordetialsearchresult',
  templateUrl: './directordetialsearchresult.component.html'
})

export class DirectorDetialSearchresultComponent implements OnInit {
  dtOptions: DataTables.Settings = {};
  contactinfo: boolean = false;
  addressinfo: boolean = false;
  directors: DirectorShip;
  directoraddress: DirectorAddressVm[];
  directortelephone: DirectorTelephoneVM[];

  constructor(public _myModel: NgbActiveModal, public headerservice: headernavService) {
  }

  ngOnInit(): void {
    this.headerservice.toggle(true);
    this.dtOptions = {
      pagingType: 'full_numbers',
      scrollX: true,
      order: [3, "desc"],
      language: {
        search: "Filter:"
      }
    };
    this.directors.directoraddresses = this.directoraddress;
    this.directors.directortelephones = this.directortelephone;
    if (this.directors.directoraddresses.length > 0)
      this.addressinfo = true;
    if (this.directors.directortelephones.length > 0)
      this.contactinfo = true;
  }

  close() {
    this._myModel.dismiss();
  }
}


