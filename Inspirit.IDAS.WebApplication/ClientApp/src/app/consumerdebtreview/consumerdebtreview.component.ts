import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ConsumerDebtDetailComponent } from '../consumerdebtdetail/consumerdebtdetail.component';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'consumerdebtreview',
  templateUrl: './consumerdebtreview.component.html'
})
export class ConsumerDebtReviewComponent {
    dtOptions: DataTables.Settings = {};
    @Input() consumer: any;
  results: any;

  constructor(private modalService: NgbModal, public headerservice: headernavService) {

  }
  ngOnInit(): void {
    this.headerservice.toggle(true);
      this.dtOptions = {
        pagingType: 'full_numbers',
        scrollX: true,
        order: [4, "desc"],
        language: {
          search: "Filter:"
        },
      };
    this.results = this.consumer;
  }

  showdetail(id: any) {
    const modalRef = this.modalService.open(ConsumerDebtDetailComponent, { size: 'lg' });
    modalRef.componentInstance.sales = this.consumer.find(x => x.consumerDebtReviewID == id);

  }
}
