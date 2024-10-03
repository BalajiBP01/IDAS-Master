import { Component, Inject, Input } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import {
  AddressDetail,
  ConsumerSearchRequest,
  CompanySearchRequest,
} from '../services/services'
import { AddressDetailComponent } from '../addressdetail/addressdetail'
import { headernavService } from '../header-nav/header-nav.service'

@Component({
  selector: 'addresslist',
  templateUrl: './addresslist.component.html',
})
export class AddressListComponent {
  dtOptions: DataTables.Settings = {}
  //_tracingRequest: AddressSearchRequest = new AddressSearchRequest();
  //_tracingResponse: AddressSearchResponse = new AddressSearchResponse();
  _consumerRequest: ConsumerSearchRequest = new ConsumerSearchRequest()
  result: any
  @Input() addresslist: any
  @Input() pagename: string
  _tracingRequest: ConsumerSearchRequest = new ConsumerSearchRequest()
  _companyRequest: CompanySearchRequest = new CompanySearchRequest()
  IsRestrictedCustomerUser: any
  IsRestrictedCustomer: any

  constructor(
    private modalService: NgbModal,
    public router: Router,
    public headerservice: headernavService,
  ) {}
  ngOnInit(): void {
    this.result = this.addresslist
    this.headerservice.toggle(true)

    this.IsRestrictedCustomer = localStorage.getItem('IsRestrictedCustomer')
    this.IsRestrictedCustomerUser = localStorage.getItem(
      'IsRestrictedCustomerUser',
    )
    this.dtOptions = {
      scrollX: true,
      order: [3, 'desc'],
      language: {
        search: 'Filter:',
      },
    }
  }
  showModal(address: any) {
    if (this.pagename == 'profile') {
      this._tracingRequest.address = address
      this._tracingRequest.type = 'lookup'
      this.router.navigate(['tracingSearch/consumerSearchResult'], {
        queryParams: this._tracingRequest,
        skipLocationChange: true,
      })
    } else if (this.pagename == 'company') {
      this._companyRequest.commercialAddress = address
      this._companyRequest.type = 'lookup'
      this.router.navigate(['tracingSearch/commercialSearchResult'], {
        queryParams: this._companyRequest,
        skipLocationChange: true,
      })
    }
  }
  //  showModal(address: any) {
  //  if (this.pagename == "profile") {
  //    this._consumerRequest = new ConsumerSearchRequest();
  //    var addr = address;//event.target.getAttribute("view-add-id").split("_").join(" ");
  //    this._consumerRequest.address = addr;
  //    this._consumerRequest.type = "Profile"
  //    this.router.navigate(['tracingSearch/consumerSearchResult'], { queryParams: this._consumerRequest, skipLocationChange: true });
  //  }
  //  else if (this.pagename == "company") {
  //    this._consumerRequest = new ConsumerSearchRequest();
  //    var addr = address;//event.target.getAttribute("view-add-id").split("_").join(" ");
  //    this._consumerRequest.address = addr;
  //    this._consumerRequest.type = "Profile"
  //    this.router.navigate(['tracingSearch/consumerSearchResult'], { queryParams: this._consumerRequest, skipLocationChange: true });
  //  }
  //}
  addressmodel(id: any) {
    const modalRef = this.modalService.open(AddressDetailComponent, {
      size: 'lg',
    })
    modalRef.componentInstance.address = this.result.find((x) => x.id == id)
  }
}
