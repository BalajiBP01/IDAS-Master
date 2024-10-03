import { Component, Inject, Input, OnChanges  } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ContactDetail } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'contactdetails',
  templateUrl: './contactdetails.html'
})

export class ContactDetailsComponent implements OnChanges {
  
    @Input() contactlist: any ;
  contact: ContactDetail;
  consumer: boolean;
    
    constructor(public _myModel: NgbActiveModal, public headerservice : headernavService ) {
      this.headerservice.toggle(true);
    }
  close()
  {
      this._myModel.dismiss();
  }
  ngOnChanges() {
  }
}
