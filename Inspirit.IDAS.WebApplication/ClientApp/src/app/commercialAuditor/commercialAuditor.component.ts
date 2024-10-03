import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { CompanySearchRequest, CompanyService, AuditorAddressVM } from '../services/services';
import { CommercialAuditorDetailComponent } from '../commercialAuditorDetail/commercialAuditorDetail.component';
import { headernavService } from '../header-nav/header-nav.service';



@Component({
  selector: 'commercialAuditor',
  templateUrl: './commercialAuditor.component.html'
})
export class CommercialAuditorComponent {
  dtOptions: DataTables.Settings = {};
  result: any;
  type: string;
  @Input() Auditors: any;
  address: AuditorAddressVM[];
  load: boolean = false;
  istrailuser: boolean = false;

  public _request: CompanySearchRequest = new CompanySearchRequest();

  constructor(private modalService: NgbModal, public router: Router, public service: CompanyService, public headerservice: headernavService) {
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
      order: [2, "desc"],
      language: {
        search: "Filter:"
      },
    };
    this.result = this.Auditors;
  }
  showauditormodel(id: any) {
    this.load = true;
   
    this.service.getAuditorDetail(id).subscribe((response) => {
      if (this.istrailuser == false) {
        this.address = response.auditoraddresess;
      }
      });
    
    this.load = false;
    const modalRef = this.modalService.open(CommercialAuditorDetailComponent, { size: 'lg' });
    modalRef.componentInstance.auditor = this.result.find(x => x.commercialAuditorID == id);
    modalRef.componentInstance.auditorAddress = this.address;


  }
}
