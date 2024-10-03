//import { Component, Inject } from '@angular/core';
//import { HttpClient } from '@angular/common/http';
//import { Router } from '@angular/router';
//import { PropertyDeedDetail, ConsumerSearchRequest, Endorsement } from '../services/services';
//import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
//import { headernavService } from '../header-nav/header-nav.service';


//@Component({
//  selector: 'deedsinformation',
//  templateUrl: './deedsinformation.html'
//})
//export class DeedsInformationComponent {

//  deeds: PropertyDeedDetail;
//  companyedeeds: PropertyDeedDetail;
//  endorsement: Endorsement;

//  sales: PropertyDeedDetail;

//  con_request: ConsumerSearchRequest = new ConsumerSearchRequest();


//  constructor(public _myModel: NgbActiveModal, public router: Router, public headerservice: headernavService) {


//  }
//  ngOnInit() {
//    this.headerservice.toggle(true);
//  }



//  close() {
//    this._myModel.dismiss();
//  }

//  searchperson(idno: any, consumerid: any) {
//    if (consumerid == null) {
//      alert("No data Available");
//    }
//    else {
//      this.con_request.iDNumber = idno;
//      this.con_request.type = "Profile";
//      this.router.navigate(['tracingSearch/personProfile', consumerid]);
//    }

//  }
//}
import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { PropertyDeedDetail, ConsumerSearchRequest, Endorsement } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'deedsinformation',
  templateUrl: './deedsinformation.html'
})
export class DeedsInformationComponent {

  deeds: PropertyDeedDetail;
  companyedeeds: PropertyDeedDetail;
  endorsement: Endorsement;

  sales: PropertyDeedDetail;

  con_request: ConsumerSearchRequest = new ConsumerSearchRequest();


  constructor(public _myModel: NgbActiveModal, public router: Router, public headerservice: headernavService) {


  }
  ngOnInit() {
    this.headerservice.toggle(true);
    console.log(this.deeds.buyerName);
  }



  close() {
    this._myModel.dismiss();
  }

  searchperson(idno: any, consumerid: any) {
    if (consumerid == null) {
      alert("No data Available");
    }
    else {
      this.con_request.iDNumber = idno;
      this.con_request.type = "Profile";
      this.router.navigate(['tracingSearch/personProfile', consumerid]);
    }

  }
}


