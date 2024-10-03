import { Component, OnInit, Renderer, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import { DataTableDirective } from 'angular-datatables';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { DoNotCallRegistryService, DncrResponse, DonotCallRegistrySearchRequest , UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as $ from 'jquery';
import 'datatables.net';
import * as _moment from 'moment';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
import { NgbModal, NgbActiveModal, NgbModalOptions} from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';


@Component({
    selector: 'app-dncrlist',
    templateUrl: './dncrlist.component.html',
})

export class donotcallregistrylistComponent implements OnInit {
    columnNames: any;
    isProcess: boolean;
    array_values: any;
    arrayBuffer: any;
    @ViewChild(DataTableDirective)
    dtElement: DataTableDirective;
  
    dtOptions: DataTables.Settings = {};
    dtTrigger: Subject<any> = new Subject();
    fromDatestring: string;
    toDatestring: string;
    _DataTableRequest: DonotCallRegistrySearchRequest = new DonotCallRegistrySearchRequest();
    message: string;

  errormsg: any;

    fromDate: string = Date.toString();
    toDate: string = Date.toString();
    mode: string = "View";
    userid: any;
    userper: UserPermission = new UserPermission();

    constructor(public router: Router, private renderer: Renderer, private datePipe: DatePipe, private httpclient: HttpClient, public _doNotCallRegistryService: DoNotCallRegistryService, public userService: UserService, public asideNavService: AsideNavService, private modalService: NgbModal, public popupservice: PopupService) {


        this.asideNavService.toggle(true);

        this.userid = localStorage.getItem('userid');
      this.userService.getPermission(this.userid, "Donot Call Registry").subscribe(result => {
            this.userper = result;
            if (this.userper == null || this.userper.viewAction == false) {
              document.getElementById('nopermission').click();
            }
        });


    }
    ngOnInit(): void {
        this.change();
        this.fromDatestring = this.datePipe.transform(new Date(new Date().getFullYear(), new Date().getMonth(), 1), 'yyyy-MM-dd');
        this.toDatestring = this.datePipe.transform(new Date(), 'yyyy-MM-dd');
    }


    ngAfterViewInit(): void {
        this.renderer.listenGlobal('document', 'click', (event) => {
            if (event.target.hasAttribute("view-id")) {
                this.router.navigate(['dncrlist/dncrdetails', event.target.getAttribute("view-id")]);
            }
        });
        this.dtTrigger.next();
    }


    dncrdetailform() {
        this.router.navigate(['/dncrdetails']);
    }

    uploadfile() {
        this.router.navigate(['dncrlist/dncfileupload']);
    }


    change() {


        this._DataTableRequest = new DonotCallRegistrySearchRequest();
      
        var req = JSON.stringify(this._DataTableRequest);

        this.dtOptions = {
            ajax: {
                url: "/api/DoNotCallRegistry/GetDoNotCallRegList",
                type: 'POST',
                contentType: 'application/json; charset=UTF-8',
                error: function (xhr, error, code) { console.log(error); },


                data: function (data) {
                    var req1 = JSON.parse(req);
                    req1.fromdate = $('#fromdate').val();
                    req1.todate = $('#todate').val();
                    req1.dtRequest = data;
                    var req2 = JSON.stringify(req1);
                    return req2;
                }
            },

            columns: [

                { data: 'name', title: 'Name', name: 'name' },

                { data: 'surname', title: 'SurName', name: 'surname' },

                { data: 'idNumber', title: 'Id Number', name: 'idNumber' },
                { data: 'emailId', title: 'Email Id', name: 'emailId' },
                { data: 'phoneNumber', title: 'Phone Number', name: 'phoneNumber' },
                {
                    data: 'currentDate', title: 'Date', name: 'currentDate', "render": function (data, type, row) {
                        return _moment(new Date(data).toString()).format('YYYY-MM-DD');
                    }
                },
                { data: 'isApproved', title: 'Approved', name: 'isApproved' },
                {
                    data: 'id', title: 'Action', name: 'id',
                    "render": function (data: any, type: any, full: any) {
                        return '<button class="btn btn-link" style="padding:0px"  view-id=' + data + '> View </button>'
                    }
                },
            ],

            processing: true,
          serverSide: false,
          scrollX: true,
            pagingType: 'full_numbers',
            pageLength: 10,
        };
    }
   

    ngOnDestroy(): void {
        // Do not forget to unsubscribe the event
        this.dtTrigger.unsubscribe();
    }

    changefile(files) {
        if (files.length === 0) {
            let ngbModalOptions: NgbModalOptions = {
                backdrop: 'static',
                keyboard: false
            };


            const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
            modalRef.componentInstance.message = "Please upload your excel document.";
            modalRef.componentInstance.isconfirm = false;

            this.popupservice.buttonchange.subscribe((credits) => {
                return false;
            });
            
        }
        const formData = new FormData();
        for (let file of files)
            formData.append(file.name, file);
        const uploadReq = new HttpRequest('POST', 'api/DoNotCallRegistry', formData, {
            reportProgress: true,
        });
        this.httpclient.request(uploadReq).subscribe(event => {
            if (event.type === HttpEventType.Response) {
                this.message = event.body.toString();
            }
        });
    }

    Importfile(files)
    {
        if (files.length === 0) {
            let ngbModalOptions: NgbModalOptions = {
                backdrop: 'static',
                keyboard: false
            };

            const modalRef = this.modalService.open(PopupComponent, ngbModalOptions);
            modalRef.componentInstance.message = "Please upload your excel document.";
            modalRef.componentInstance.isconfirm = false;

            this.popupservice.buttonchange.subscribe((credits) => {
                return false;
            });
            
        }
        let fileReader = new FileReader();
        fileReader.onload = (e) => {
            this.arrayBuffer = fileReader.result;
            var data = new Uint8Array(this.arrayBuffer);
            var arr = new Array();
            for (var i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
            var bstr = arr.join("");
            var workbook = XLSX.read(bstr, { type: "binary" });
            var first_sheet_name = workbook.SheetNames[0];
            var worksheet = workbook.Sheets[first_sheet_name];
            this.array_values = XLSX.utils.sheet_to_json(worksheet, { raw: true }).slice(0, 100);
            if (this.array_values.length >= 1 && this.array_values.length <= 10) {
                this.isProcess = true;
                this._doNotCallRegistryService.getColumns(files[0].name).subscribe((result) => {
                    this.columnNames = result;
                });
            }
            else {
                this.isProcess = false;
            }
        }
        fileReader.readAsArrayBuffer(files[0]);
    }

    process(file)
    {
        this.isProcess = false;
        this.change();
    }


    rerender(): void {
        if (new Date($('#fromdate').val().toString()) > new Date($('#todate').val().toString())) {
          this.errormsg = 'To date should be greater than from date';
          document.getElementById('errormsg').click();
          return;
        }
        this.dtElement.dtInstance.then((dtInstance1: DataTables.Api) => {
            dtInstance1.ajax.reload();
        });
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
    refresh() {

    }
    
}
