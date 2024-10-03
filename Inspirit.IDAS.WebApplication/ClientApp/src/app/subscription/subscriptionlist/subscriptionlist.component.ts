import { Component, OnInit, Renderer } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { debug, isNullOrUndefined } from 'util';
import * as _moment from 'moment';
import { EventEmitter } from 'events';
import { headernavService } from '../../header-nav/header-nav.service';

import {
  SubscriptionService, CustomerUser,
  SubDataTableRequest  , SubscriptionLicenceVm, SubscriptionLicenceRequest
} from '../../services/services';
import { SubscriptionLicenceUserListComponent } from '../assignlicenseuserlist/assignlicenseuserlist';

@Component({
  selector: 'app-subscriptionlist',
  templateUrl: './subscriptionlist.component.html'
})

export class SubscriptionComponent implements OnInit {
 
  dtOptions: DataTables.Settings = {};
  public sub: any;
  public id: any;
  public customerid: any;
  public subid: any;
  public users: SubscriptionLicenceVm[];
  public asignedUsers: SubscriptionLicenceRequest = new SubscriptionLicenceRequest();
  dataTablereq: SubDataTableRequest = new SubDataTableRequest();
  public customerList: CustomerUser[];
  mode: string = "View";
  custmerId: any;
  subId: any;
  public res: any;
  name: any;
  isuserExists: any;
  loading: boolean = false;

  constructor(public router: Router, public route: ActivatedRoute,
    public renderer: Renderer,
    public subscriptionservice: SubscriptionService, private modalService: NgbModal,
    public headernavService: headernavService) {
    if ((typeof window !== "undefined") ? window.localStorage : null) {
      this.customerid = localStorage.getItem('customerid');
    }
  }
  ngOnInit(): void {
    this.loading = true;
    this.isuserExists = localStorage.getItem('userid');
    if (this.isuserExists != null && this.isuserExists != "undefined") {

      this.headernavService.toggle(true);
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }

      this.custmerId = localStorage.getItem('customerid');
     

      this.dataTablereq.customerId = this.custmerId;
      

      var req = JSON.stringify(this.dataTablereq);
      this.dtOptions = {
        ajax: {
          url: '/api/Subscription/GetSubscriptionList',
          type: 'POST',
          contentType: 'application/json; charset=UTF-8',
          error: function (xhr, error, code) { console.log(error); },
          data: function (data) {
            var req1 = JSON.parse(req);
            req1.dtRequest = data;
            var req2 = JSON.stringify(req1);
            return req2;
          }
        },

        columns: [
          {
            data: 'subscriptionDate', title: 'Subscription Date', name: 'subscriptionDate',
            "render": function (data, type, row) {
              return _moment(new Date(data).toString()).format('YYYY-MM-DD');
            }
          },
          { data: 'productName', title: 'Product Name', name: 'productName' },
          { data: 'numberofAssign', title: 'Licences Purchased/Credits Available', name: 'numberofAssign' },
          { data: 'numberOfUsers', title: 'Number of Users Assigned', name: 'numberOfUsers' },
          {
            data: 'id', title: 'Subscription Number', name: 'subscriptionid',
            "render": function (data: any, type: any, row: any) {
              return '<button class="btn btn-link" style="padding:0px"  subscription-view-id=' + data + '> ' + row.number + ' </button>'
            }
          },
          {
            data: 'isPaid', title: 'Action', name: 'isPaid',
            "render": function (data: any, type: any, row: any) {
              var value = "Payment Not Recieved";
              if (row.isAutoBilled == false) {
                value = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModel" license-user-id=' + row.id + '> Assign License to User </button>'
              }
              else if (data != "") {
                if (row.isLicenced == false) {
                  value = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModel"  license-user-id=' + row.id + '> Assign Credits to User </button>'
                }
                else {
                  value = '<button class="btn btn-link" style="padding:0px" data-toggle="modal" data-target="#myModel" license-user-id=' + row.id + '> Assign License to User </button>'
                }
              }
              return value
            }
          }
        ],
        initComplete: function () {
          document.getElementById('load').click();
        },
        language: {
          search: "Filter:"
        },
        processing: true,
        serverSide: false,
        pagingType: 'full_numbers',
        pageLength: 10,
        scrollX: true,
      };
    } else {
      this.router.navigate(['/login']);
    }
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
      if (event.target.hasAttribute("subscription-view-id")) {
        this.router.navigate(['Subscriptions/subscriptiondetail'], { queryParams: { id: event.target.getAttribute("subscription-view-id") }, skipLocationChange: true });
      }
      else if (event.target.hasAttribute("license-user-id")) {       
        this.subscriptionservice.getSubscribtionUsers(event.target.getAttribute("license-user-id")).subscribe((result) => {
          this.users = result;
        });
      }
    });
  }
  subscriptionform() {
    this.router.navigate(['Subscriptions/subscriptiondetail']);
  }
  save() {
    this.asignedUsers.subscriptionLicenceVms = this.users;
    this.subscriptionservice.assignLicensetoUsers(this.asignedUsers).subscribe((result) => {
      if (result != "") { alert(result); }
      else {
        let origin = window.location.href;
        if (origin.indexOf('assignusers') >= 0)
          this.router.navigate(['Subscriptions']);
        else
          this.router.navigate(['Subscriptions/assignusers']);
      }
    });
  }
  load() {
    this.loading = false;
  }
}
