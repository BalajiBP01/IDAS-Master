import { Component, Inject, Input, OnChanges, Renderer } from '@angular/core';
import { Router } from '@angular/router';
import { DataTableRequest, NewsData, NewsDataTableResponse, NewsService, UserPermission, UserService } from '../services/services';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import * as _moment from 'moment';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';



@Component({
  selector: 'newslist',
  templateUrl: './newslist.component.html',
})
export class NewsListComponent {
  dtOptions: DataTables.Settings = {};
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer, public asideNavService: AsideNavService, public newsService: NewsService, public userService: UserService) {

    this.asideNavService.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "News Blog").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });
  }
  ngOnInit(): void {

    var req = JSON.stringify(this._DataTableRequest);
    this.dtOptions = {
      ajax: {
        url: "/api/News/GetNewsList",
        type: 'POST',
        contentType: 'application/json; charset=UTF-8',
        error: function (xhr, error, code) { console.log(error); },

        data: function (data) {
          var req1 = JSON.parse(req);
          console.log(data);
          req1 = data;
          var req2 = JSON.stringify(req1);
          return req2;
        }
      },
      columns: [
        {
          data: 'createdDate', title: 'Date', name: 'createdDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD H:mm:ss');
          }
        },
        {
          data:'name', title:'Name',name:'name'
        },
        {
          data: 'id', title: 'View', name: 'id',
          'render': function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" view-news-id=' + data + '> View </button>'
          }
        }
        
      ],
      processing: true,
      serverSide: false,
      scrollX: true,
      pagingType: 'full_numbers',
      pageLength: 10,
    };
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("view-news-id")) {
        this.router.navigate(['news/newsdetail', event.target.getAttribute("view-news-id")]);
      }
    });
  }
  newsdetail() {
    this.router.navigate(['news/newsdetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
