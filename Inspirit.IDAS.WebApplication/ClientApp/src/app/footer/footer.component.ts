import { Component, Inject, Input , OnInit} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { SecurityService } from '../services/services';
import { FooterService } from './footer.service';
import { debounce } from 'rxjs/operators';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html'
})

export class FooterComponent implements OnInit{
  loading: boolean;

  _visible: Observable<boolean> = Observable.of(true);

  constructor(public securityService: SecurityService, public router: Router, public footerService: FooterService) {
    
  }

  ngOnInit() {
    this.footerService.changefooter.subscribe((isVisible = this.loading) => {
      this._visible = Observable.of(isVisible);
    });
  }
}

