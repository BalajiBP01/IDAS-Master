import { Component, Inject, Input } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { TracingService } from '../services/services';
import { headernavService } from '../header-nav/header-nav.service';


@Component({
  selector: 'employmentlist',
  templateUrl: './employmentlist.component.html'
})
export class EmploymentListComponent {
    dtOptions: DataTables.Settings = {};
    result: any;
    @Input() employmentlist: any;
  constructor(public headerservice: headernavService) {
 
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
      this.result = this.employmentlist;
  }
  
}

