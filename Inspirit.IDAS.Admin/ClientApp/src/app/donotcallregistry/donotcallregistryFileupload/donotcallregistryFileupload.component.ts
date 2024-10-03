import { Component, OnInit, Renderer, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { DataTablesModule } from 'angular-datatables';
import { DataTableDirective } from 'angular-datatables';
import { DatePipe } from '@angular/common';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse, HttpHeaders } from '@angular/common/http';
import { DoNotCallRegistryService, DncrResponse, DonotCallRegistrySearchRequest, DoNotCallRegistryVM, UserService, UserPermission } from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import * as $ from 'jquery';
import 'datatables.net';
import * as _moment from 'moment';
import { Subject } from 'rxjs';
import * as XLSX from 'xlsx';
import { NgbModal, NgbActiveModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';



@Component({
    selector: 'app-donotcallregistryFileupload',
    templateUrl: './donotcallregistryFileupload.component.html',
})

export class DonotcallregistryFileuploadcomponent  {
    columnNames: any;
    isProcess: boolean;
    array_values: any;
    arrayBuffer: any;
    message: any;
    validationmsg: any;
    columns: string[];
    filename: string;
    insertResponse: DoNotCallRegistryVM[];
    isfailed: boolean = false;
    userid: any;
    userper: UserPermission = new UserPermission();
  processbtn: boolean = false;
  fileexists: boolean = false;

  errormsg: any;

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
  changefile(files) {
        this.isfailed = false;
        if (files.length === 0) {
            //alert('Please upload your excel document');
          this.errormsg = 'Browse your excel document';
          document.getElementById('nopermission').click();
          return;
        }
        const formData = new FormData();
    for (let file of files)
      formData.append(file.name, file);
    this.filename = files[0].name;
    this.fileexists = true;
        const uploadReq = new HttpRequest('POST', 'api/DoNotCallRegistry', formData, {
            reportProgress: true,
        });
        this.httpclient.request(uploadReq).subscribe(event => {
            if (event.type === HttpEventType.Response) {
                this.message = event.body.toString();
                this.Importfile(files);
            }
        });
    }

    Importfile(files) {
        if (files.length === 0) {
          this.errormsg = 'Browse your excel document';
          document.getElementById('nopermission').click();
          return;

           
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
                this.isProcess = true;
                this._doNotCallRegistryService.getColumns(files[0].name).subscribe((result) => {
                    this.columnNames = result;
                    this.filename = files[0].name;
                    this.processfile(files[0].name);
                    
                });
        }
        fileReader.readAsArrayBuffer(files[0]);
    }
    processfile(name: string) {
        this._doNotCallRegistryService.excelValidation(name).subscribe((result) => {
            this.validationmsg = result;
          if (this.validationmsg != "") {
            this.errormsg = this.validationmsg;
            document.getElementById('errormsg').click();
            return;
          }
               
            else {
                this.processbtn = true;
                this.isfailed = false;
            }
        });
    }
    process(file) {
        this._doNotCallRegistryService.excelInsert(this.filename).subscribe((result) => {
            this.insertResponse = result;
            if (this.insertResponse != null) {
                this.isProcess = false;
                this.isfailed = true;
            } else {
                this.router.navigate(['dncrlist']);
            }
        });
    }
    list() {
        this.router.navigate(['dncrlist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
