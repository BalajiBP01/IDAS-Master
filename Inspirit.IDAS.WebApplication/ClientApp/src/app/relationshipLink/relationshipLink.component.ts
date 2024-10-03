import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ConsumerSearchRequest, TracingService } from '../services/services';
import { AddressDetailComponent } from '../addressdetail/addressdetail';
import { window } from 'rxjs/operators';
import { RelationshipService } from './RelationshipService';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'relationshipLink',
  templateUrl: './relationshipLink.component.html'
})
export class RelationshipLinkComponent {
  dtOptions: DataTables.Settings = {};
  result: any = [];
  userid: any;
  customerid: any;
  type: string;
  isXDS: any;
  points: any = 0;
  @Input() relationshipLinkList: any;
  public _request: ConsumerSearchRequest = new ConsumerSearchRequest();

  constructor(private modalService: NgbModal, public router: Router, public relationshipService: RelationshipService,public tracingService: TracingService , public headerservice: headernavService) {
  }
  ngOnInit(): void {
    this.headerservice.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.customerid = localStorage.getItem('customerid');
    this.isXDS = localStorage.getItem('isXDS');
    this.dtOptions = {
      pagingType: 'full_numbers',
      scrollX: true,
      order: [5, "asc"],
      language: {
        search: "Filter:"
      }
    };
    this.result = this.relationshipLinkList;
  }
  // points pending
  showModal(id: any, idno: any) {
    $('#content').css("display", "none");
    //if (this.isXDS == 'NO') {
      this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
        this.points = result;
        if (this.points > 0) {
          this.relationshipService.gotoProfile(idno);
        } else {
          $('#content').css("display", "block");
          document.getElementById('nopoints').click();
          return;
        }
      });
    //}
  }
}
