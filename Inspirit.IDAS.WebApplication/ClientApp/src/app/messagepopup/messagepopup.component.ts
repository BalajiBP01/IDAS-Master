import { Component, Inject, OnChanges } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ApplicationMessages,SecurityService } from '../services/services';


@Component({
  selector: 'messagepopupComponent',
  templateUrl: './messagepopup.component.html'
})
export class MessagePopupComponent implements OnChanges {
  title: "INSPIRIT IDAS Says:"
  messages: ApplicationMessages[];
  isconfirm: boolean;
  userid: any;
   
  constructor(public _myModal: NgbActiveModal, public securityService: SecurityService) {
    this.userid = localStorage.getItem('userid');
  }
  ngOnChanges() {
  }
  close() {
    this.securityService.removeAppMessages(this.userid).subscribe((res) => {
    });
    this._myModal.dismiss();
  }
}

