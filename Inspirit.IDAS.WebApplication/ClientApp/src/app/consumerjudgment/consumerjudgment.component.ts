import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { ConsumerjudgementDetailComponent } from '../consumerjudgementdetail/consumerjudgementdetail.component';
import { log } from 'util';
import { CommercialJudgement } from '../services/services';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'consumerjudgment',
  templateUrl: './consumerjudgment.component.html'
})
export class ConsumerjudgmentComponent {
  dtOptions: DataTables.Settings = {};
  @Input() consumerjudgementlist: any;
  @Input() commercialJudgement: any;
  @Input() pagename: any;
  results: any;
  constructor(private modalService: NgbModal,public headerservie: headernavService) {
  
  }

  ngOnInit(): void {
    this.headerservie.toggle(true);
      this.dtOptions = {
        pagingType: 'full_numbers',
        scrollX: true,
        order: [5, "desc"],
        language: {
          search: "Filter:"
        }
    };
    if (this.pagename == "profile") {
      this.results = this.consumerjudgementlist;
    } else if (this.pagename == "company") {
      this.results = this.commercialJudgement;
    }
    
  }

  showdetail(id: any) {
      if (this.pagename == "profile")
      {
          const modalRef = this.modalService.open(ConsumerjudgementDetailComponent, { size: 'lg' });
          modalRef.componentInstance.sales = this.consumerjudgementlist.find(x => x.id == id);
      }
      else if (this.pagename == "company")
      {
          const modalRef = this.modalService.open(ConsumerjudgementDetailComponent, { size: 'lg' });
        modalRef.componentInstance.company = this.commercialJudgement.find(x => x.commercialJudgmentID == id);
      }
  }
}


