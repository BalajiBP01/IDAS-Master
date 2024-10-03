import { Component, Inject, Input, OnChanges, Renderer } from '@angular/core';
import { Router } from '@angular/router';
import { DataTableRequest, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as _moment from 'moment';
import { NgbModal, NgbActiveModal, NgbModalOptions} from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
  selector: 'app-dsalist',
  templateUrl: './dsalist.component.html',
})
export class dsalistComponent {
  dtOptions: DataTables.Settings = {};
  _DataTableRequest: DataTableRequest = new DataTableRequest();
  userid: any;
  userper: UserPermission = new UserPermission();

  constructor(public router: Router, private renderer: Renderer, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {

      this.asideNavService.toggle(true);

      this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Data Service Agreement").subscribe(result => {
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
        url: "/api/Dsa/GetDsaTableList",
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
          data: 'effectiveDate', title: 'Effective Date', name: 'effectiveDate',
          "render": function (data, type, row) {
            return _moment(new Date(data).toString()).format('YYYY-MM-DD');
          }
        },        
        { data: 'version', title: 'Version', name: 'version' },
        { data: 'isPublished', title: 'Is Published', name: 'isPublished' },
        {
          data: 'id', title: 'Action', name: 'id',
          "render": function (data: any, type: any, full: any) {
            return '<button class="btn btn-link" style="padding:0px" dsaview-id=' + data + '> View </button>'
          }
        },
      ],
      processing: true,
      serverSide: false,
      pagingType: 'full_numbers',
      pageLength: 10,
      scrollX: true,
    };
  }
  ngAfterViewInit(): void {
    this.renderer.listenGlobal('document', 'click', (event) => {
        if (event.target.hasAttribute("dsaview-id")) {
            this.router.navigate(['dsalist/dsadetail', event.target.getAttribute("dsaview-id")]);
      }
    });
  }
  dsadetailform() {
    this.router.navigate(['dsalist/dsadetail']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
