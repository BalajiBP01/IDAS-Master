import { Component, Inject, OnChanges } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { Router } from '@angular/router'
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap'
import { PopupService } from './popupService'

@Component({
  selector: 'app-popupComponent',
  templateUrl: './popup.component.html',
})
export class PopupComponent implements OnChanges {
  title: 'INSPIRIT IDAS Says:'
  message: string
  isconfirm: boolean

  constructor(
    public _myModal: NgbActiveModal,
    public popupService: PopupService,
  ) {}
  ngOnChanges() {}
  close() {
    this.popupService.updatenavigation(false)
    this._myModal.dismiss()
  }
  ok() {
    this.popupService.updatenavigation(true)
    this._myModal.dismiss()
  }
}
