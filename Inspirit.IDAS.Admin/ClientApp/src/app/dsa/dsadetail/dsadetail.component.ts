import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType } from '@angular/common/http';
import {
  DsaService, DataServicesAgreement,
  CrudResponse, DataTableRequest, DsaList,
  UserService, UserPermission
} from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
import { Router, ActivatedRoute } from '@angular/router';
import { DatePipe } from '@angular/common';
import { error, isNullOrUndefined } from 'util';
import { EventEmitter } from 'events';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { PopupComponent } from '../../popup/popup.component';
import { PopupService } from '../../popup/popupService';

@Component({
  selector: 'app-dsadetail',
  templateUrl: './dsadetail.component.html'
})
export class dsadetailComponent implements OnInit {
  @ViewChild('file')
  myInputVariable: ElementRef;
  message: string;
  progress: number;
  datestring: string;
  currentObj: DataServicesAgreement = new DataServicesAgreement();
  _dataTableRequest: DataTableRequest = new DataTableRequest();
  id: any;
  private sub: any;
  mode: string = "view";
  reponse: CrudResponse;
  isPublishedList: DsaList[];
  dsaList: DsaList[];
  readonly: boolean = false;
  isSub: boolean = false;
  userid: any;
  userper: UserPermission = new UserPermission();
  isprivileged: boolean = false;
  addNew: number = 0;
  success: any;
  loading: boolean = false;
  public editorOptions = {
    placeholder: 'Compose an epic...',
    modules: {
      toolbar: [[{ 'font': [] }, 'bold', 'italic', 'underline', 'strike',
        'blockquote', 'code-block', { 'header': 1 }, { 'header': 2 },
      { 'list': 'ordered' }, { 'list': 'bullet' },
      { 'script': 'sub' }, { 'script': 'super' },
      { 'indent': '-1' }, { 'indent': '+1' },
      { 'direction': 'rtl' }, { 'size': ['small', false, 'large', 'huge'] },
      { 'header': [1, 2, 3, 4, 5, 6, false] }, { 'color': [] }, { 'background': [] },
      { 'align': [] }, 'clean', 'link', 'image']]
    }
  };
  constructor(public router: Router, private http: HttpClient,
    private route: ActivatedRoute, private dsaService: DsaService,
    private datePipe: DatePipe, public userService: UserService,
    public asideNavService: AsideNavService,
    private modalService: NgbModal,
    public popupservice: PopupService) {

    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    if (!isNullOrUndefined(this.userid)) {
      this.userService.getPermission(this.userid, "Data Service Agreement").subscribe(result => {
        this.userper = result;
        if (this.userper == null || this.userper.viewAction == false) {
          document.getElementById('nopermission').click();
        }
      });

    }
  }
  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });

    this.dsaService.getDsaTableList(this._dataTableRequest).subscribe((res) => {
      this.isPublishedList = res.data.filter(l => l.isPublished == true);
    });
    
    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.currentObj = new DataServicesAgreement();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.dsaService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.readonly = true;
        this.loading = false;
      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  save() {
    if (this.currentObj.version <= 0 || this.currentObj.version == null) {
      this.alertMessage("Enter correct version");
      return;
    }
    if ((this.currentObj.description == null || this.currentObj.description == "undefined")) {
      this.alertMessage("Description is required");
      return;
    }
    this.currentObj.effectiveDate = new Date();
    if (this.mode == "add") {
      this.loading = true;
      this.dsaService.insert(this.currentObj).subscribe((result) => {
        this.loading = false;
        if (result.message != "") {
          this.alertMessage(result.message);
          return;
        }
        else
          this.router.navigate(['dsalist']);
      });
    }
    else if (this.mode == "edit") {
      this.loading = true;
      this.dsaService.update(this.currentObj).subscribe((result) => {
        this.loading = false;
        if (result.message != "") {
          this.alertMessage(result.message);
          return;
        }
        else {
          this.router.navigate(['dsalist']);
        }
      });
    }
  }
  edit() {
    this.mode = "edit";
    this.readonly = false;
    this.loading = true;
    this.dsaService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.loading = false;
      this.datestring = this.datePipe.transform(this.currentObj.effectiveDate, 'yyyy-MM-dd');
    });
  }
  delete() {
    this.dsaService.delete(this.id).subscribe((result) => {
      if (result.message != "") {
        this.alertMessage(result.message);
      }
      else
        this.router.navigate(['dsalist']);
    });
  }   
  list() {
    this.router.navigate(['dsalist']);
  }
  Exportfile() {
    if (isNullOrUndefined(this.currentObj.filePath)) {
      this.alertMessage("Please upload your pdf document");
      return;
    }
    this.dsaService.downloadPdf(this.currentObj.filePath).subscribe((resp) => {
      this.downloadFile(resp.data, resp.fileName);
    });
  }
  downloadFile(blob, filename: string) {
    var url = window.URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.setAttribute("href", url);
    link.setAttribute("download", filename);
    link.click();
  }
  change(files) {
    var ext = files[0].name.substr(files[0].name.lastIndexOf('.') + 1);
    if (files.length === 0 || ext != "pdf" || isNullOrUndefined(this.currentObj.version)) {
      if (isNullOrUndefined(this.currentObj.version)) {
        this.myInputVariable.nativeElement.value = "";
        this.alertMessage("Please enter dsa version");
      }
      return;
    }
    const formData = new FormData();
    for (let file of files) {
      var fileName = "V" + this.currentObj.version + file.name;
      fileName = fileName.trim();
      formData.append(fileName, file);
      this.currentObj.filePath = fileName;
    }
    const uploadReq = new HttpRequest('POST', 'api/Dsa', formData, {
      reportProgress: true,
    });
    this.http.request(uploadReq).subscribe(event => {
      if (event.type === HttpEventType.UploadProgress)
        this.progress = Math.round(100 * event.loaded / event.total);
      else if (event.type === HttpEventType.Response)
        this.message = event.body.toString();
      if (this.message != null && this.message != "undefined") {
        this.isSub = true;
        if (this.message == "Imported Successfully")
          this.isSub = true;
        else
          this.isSub = false;

        this.alertMessage(this.message);
        return;
      }
    });
  } 
  alertMessage(message: string) {

    this.message = message;
    this.loading=false;
    document.getElementById('error').click();
    return;
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
