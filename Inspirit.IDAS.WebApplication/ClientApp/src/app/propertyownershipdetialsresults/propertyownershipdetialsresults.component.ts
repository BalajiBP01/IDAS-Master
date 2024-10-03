import { Component, Inject, Input, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { TracingService, PersonProfileService, Endorsement } from '../services/services';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { DeedsInformationComponent } from '../deedsinformation/deedsinformation';
import { headernavService } from '../header-nav/header-nav.service';

@Component({
  selector: 'propertyownershipdetialsresults',
  templateUrl: './propertyownershipdetialsresults.component.html'
})
export class PropertyOwnerShipDetialsresultComponent {
  dtOptions: DataTables.Settings = {};
  @Input() property: any;
  @Input() pagename: string;
  result: any;
  endorsement: Endorsement = new Endorsement();
  load: boolean = false;
  istrailuser: boolean = false;


  constructor(private modalService: NgbModal, public service: PersonProfileService, public headerservice: headernavService) {


  }

  ngOnInit(): void {
    this.headerservice.toggle(true);
    let usertype = localStorage.getItem('trailuser');
    if (usertype == "YES") {
      this.istrailuser = true;
    } else
      this.istrailuser = false;

    this.dtOptions = {
      pagingType: 'full_numbers',
      scrollX: true,
      order: [1, "desc"],
      language: {
        search: "Filter:"
      }
    };
    this.result = this.property;
  }


  showModal(id: any) {
    this.load = true;
    if (this.pagename == 'profile') {

      this.service.getEndorsementdetail(id).subscribe((response) => {
        if (this.istrailuser == false) {
          this.endorsement = response;
          this.load = false;
          const modalRef = this.modalService.open(DeedsInformationComponent, { size: 'lg' });
          modalRef.componentInstance.deeds = this.property.find(x => x.propertyDeedId == id);
          modalRef.componentInstance.endorsement = this.endorsement;
        } else {
          this.load = false;
          const modalRef = this.modalService.open(DeedsInformationComponent, { size: 'lg' });
          modalRef.componentInstance.deeds = this.property.find(x => x.propertyDeedId == id);
        }
      });

     
      
    }
    else {

      this.service.getEndorsementdetail(id).subscribe((response) => {
        if (this.istrailuser == false) {
          this.endorsement = response;
          this.load = false;
          const modalRef = this.modalService.open(DeedsInformationComponent, { size: 'lg' });
          modalRef.componentInstance.companyedeeds = this.property.find(x => x.propertyDeedId == id);
          modalRef.componentInstance.endorsement = this.endorsement;
        } else {
          this.load = false;
          const modalRef = this.modalService.open(DeedsInformationComponent, { size: 'lg' });
          modalRef.componentInstance.companyedeeds = this.property.find(x => x.propertyDeedId == id);
         
        }
      });

      

    }
  }
}
