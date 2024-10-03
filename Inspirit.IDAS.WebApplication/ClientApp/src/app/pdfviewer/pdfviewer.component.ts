import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { headernavService } from '../header-nav/header-nav.service';
import { EventEmitter } from 'events';

@Component({
  selector: 'pdfviewer',
  templateUrl: './pdfviewer.component.html'
})
export class PdfviewerComponent {
  public progress: number;
  public message: string;
  page: number = 1;
  totalPages: number;
  isLoaded: boolean = false;
  isAccept: number = 0
  src: string = "../../assets/UserManual.pdf";
  files: any;
  afterLoadComplete(pdfData: any) {
    this.totalPages = pdfData.numPages;
    this.isLoaded = true;
  }
  nextPage() {
    this.page++;
   
  }
  prevPage() {
    this.page--;
  }
  constructor(public router: Router, public headernavService: headernavService) {
    this.headernavService.toggle(false);
  }

  back() {
    this.headernavService.toggle(true);
    this.router.navigate(['dashboard']);
   
     
    }
  }

