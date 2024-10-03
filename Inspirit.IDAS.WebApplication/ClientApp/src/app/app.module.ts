import { BrowserModule } from '@angular/platform-browser'
import { NgModule, ChangeDetectorRef } from '@angular/core'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http'
import { Routes, RouterModule } from '@angular/router'
import * as $ from 'jquery'
import * as moment from 'moment'
import 'datatables.net'
import { LoadingModule } from 'ngx-loading'
import { Ng5SliderModule } from 'ng5-slider'

import { Observable } from 'rxjs/Observable'
import { DataTablesModule } from 'angular-datatables'
import {
  NgbModule,
  NgbModalModule,
  NgbModalRef,
} from '@ng-bootstrap/ng-bootstrap'
import { DataTableModule } from 'angular2-datatable'
import { NouisliderModule } from 'ng2-nouislider'
import { PdfViewerModule } from 'ng2-pdf-viewer'
import { DatePipe } from '@angular/common'
import { Ng4LoadingSpinnerModule } from 'ng4-loading-spinner'
import { ChartsModule } from 'ng2-charts'
import { DxPieChartModule, DxChartModule } from 'devextreme-angular'
import { MorrisJsModule } from 'angular-morris-js'
import { UserIdleModule } from 'angular-user-idle'

//components
import { AppComponent } from './app.component'
import { HeaderNavComponent } from './header-nav/header-nav.component'
import { HomeComponent } from './home/home.component'

import { LoginComponent } from './login/login.component'

import { UsersignupComponent } from './usersignup/usersignup.component'
import { TracingSearchComponent } from './tracingSearch/tracingSearch.component'

import { InvoiceListComponent } from './invoice/invoicelist/invoicelist.component'
import { InvoiceDetailComponent } from './invoice/invoicedetail/invoicedetail.component'

import { FullAuditReportComponent } from './fullauditreport/fullauditreport.component'
import { SummaryFullAuditComponent } from './summaryfullaudit/summaryfullaudit.component'

import { ConsumerSearchResultComponent } from './consumerSearchResult/consumerSearchResult.component'
import { CommercialSearchResultComponent } from './commercialSearchResult/commercialSearchResult.component'

import { AddressSearchResultComponent } from './addressSearchResult/addressSearchResult.component'

import { BatchProcessComponent } from './batchprocess/batchtracingsearch/batchprocess.component'
import { BatchProcessViewComponent } from './batchprocess/batchprocessview/batchprocessview.component'
import { BatchProcessListComponent } from './batchprocess/batchtracinglist/batchtracinglist.component'

import { LeadResponseComponent } from './leadsgeneration/leadresponse/leadresponse'
import { LeadListComponent } from './leadsgeneration/leadlist/leadlist'
import { LeadprocessComponent } from './leadsgeneration/leadprocess/leadprocess'
import { LeadInformationComponent } from './leadsgeneration/leadinformation/leadinformation'

import { DsaComponent } from './dsa/dsa.component'
import { PersonProfileComponent } from './personProfile/personProfile.component'
import { ComapanydtetailResultComponent } from './companydetailresult/companydetailresult.component'

import { RelationshipLinkComponent } from './relationshipLink/relationshipLink.component'

import { DirectorDetialSearchresultComponent } from './directordetialsearchresult/directordetialsearchresult.component'
import { DirectorShipListComponent } from './directorshiplist/directorshiplist.component'
import { EmploymentListComponent } from './employmentlist/employmentlist.component'
import { ContactDetailsComponent } from './contactdetails/contactdetails'
import { AddressDetailComponent } from './addressdetail/addressdetail'
import { AddressListComponent } from './addresslist/addresslist.component'
import { DeedsInformationComponent } from './deedsinformation/deedsinformation'
import { ForgetPasswordComponent } from './forgetpassword/forgetpassword.component'
import { ResetPasswordComponent } from './resetpassword/resetpassword.component'
import { PropertyOwnerShipDetialsresultComponent } from './propertyownershipdetialsresults/propertyownershipdetialsresults.component'
import { ConsumerDebtReviewComponent } from './consumerdebtreview/consumerdebtreview.component'
import { ConsumerjudgmentComponent } from './consumerjudgment/consumerjudgment.component'
import { ConsumerjudgementDetailComponent } from './consumerjudgementdetail/consumerjudgementdetail.component'
import { ConsumerDebtDetailComponent } from './consumerdebtdetail/consumerdebtdetail.component'

import { SubscriptionComponent } from './subscription/subscriptionlist/subscriptionlist.component'
import { SubscriptionDetailComponent } from './subscription/subscriptiondetail/subscriptiondetail.component'
import { SubscriptionLicenceUserListComponent } from './subscription/assignlicenseuserlist/assignlicenseuserlist'

import { userlistComponent } from './users/userlist/userlist.component'
import { userdetailsComponent } from './users/userdetails/userdetails.component'

import { DonotCallRegistryComponent } from './donotcallregistry/donotcallregistry.component'

import { PdfviewerComponent } from './pdfviewer/pdfviewer.component'

import { CommercialAuditorComponent } from './commercialAuditor/commercialAuditor.component'
import { CommercialAuditorDetailComponent } from './commercialAuditorDetail/commercialAuditorDetail.component'

import { TimeLineComponent } from './timeline/timeline.component'
import { FooterComponent } from './footer/footer.component'

import { PopupComponent } from './popup/popup.component'
import { MessagePopupComponent } from './messagepopup/messagepopup.component'

//services
import {
  SecurityService,
  TracingService,
  CompanyService,
  PersonProfileService,
  SubscriptionService,
  UserService,
  InvoiceService,
  FullAuditReportService,
  BatchTracingService,
  SummaryFullAuditService,
  DashboardService,
  LeadGenerationService,
} from './services/services'
import { RecaptchaService } from './services/recaptcha.service'
import { headernavService } from './header-nav/header-nav.service'
import { FooterService } from './footer/footer.service'
import { PopupService } from './popup/popupService'
import { RelationshipService } from './relationshipLink/RelationshipService'
import { XdsLoginComponent } from './XdsLogin/XdsLogin.component'
import { XdsLoginErrorComponent } from './XdsLoginError/XdsLoginError.component'

@NgModule({
  declarations: [
    AppComponent,
    CommercialAuditorDetailComponent,
    HomeComponent,
    LoginComponent,
    UsersignupComponent,
    ForgetPasswordComponent,
    ResetPasswordComponent,
    TracingSearchComponent,
    BatchProcessComponent,
    BatchProcessListComponent,
    BatchProcessViewComponent,
    ConsumerSearchResultComponent,
    LeadResponseComponent,
    DsaComponent,
    RelationshipLinkComponent,
    PersonProfileComponent,
    DeedsInformationComponent,
    CommercialSearchResultComponent,
    HeaderNavComponent,
    ComapanydtetailResultComponent,
    InvoiceListComponent,
    InvoiceDetailComponent,
    PdfviewerComponent,
    FullAuditReportComponent,
    SummaryFullAuditComponent,
    DirectorDetialSearchresultComponent,
    PropertyOwnerShipDetialsresultComponent,
    DirectorShipListComponent,
    EmploymentListComponent,
    ContactDetailsComponent,
    AddressListComponent,
    AddressDetailComponent,
    DonotCallRegistryComponent,
    ConsumerjudgmentComponent,
    ConsumerjudgementDetailComponent,
    ConsumerDebtReviewComponent,
    ConsumerDebtDetailComponent,
    userlistComponent,
    userdetailsComponent,
    SubscriptionComponent,
    SubscriptionDetailComponent,
    SubscriptionLicenceUserListComponent,
    userlistComponent,
    userdetailsComponent,
    CommercialAuditorComponent,
    TimeLineComponent,
    FooterComponent,
    PopupComponent,
    MessagePopupComponent,
    AddressSearchResultComponent,
    LeadListComponent,
    LeadprocessComponent,
    LeadInformationComponent,
    XdsLoginComponent,
    XdsLoginErrorComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    ChartsModule,
    DxPieChartModule,
    DxChartModule,
    MorrisJsModule,
    FormsModule,
    ReactiveFormsModule,
    DataTablesModule,
    UserIdleModule.forRoot({ idle: 480, timeout: 60, ping: 60 }),
    NgbModule.forRoot(),
    Ng4LoadingSpinnerModule.forRoot(),
    NgbModalModule,
    DataTableModule,
    LoadingModule,
    Ng5SliderModule,
    PdfViewerModule,
    RouterModule.forRoot([
      { path: '', component: LoginComponent, pathMatch: 'full' },
      { path: 'dashboard', component: HomeComponent },
      { path: 'login', component: LoginComponent },
      { path: 'usersignup', component: UsersignupComponent },
      { path: 'forgetpassword', component: ForgetPasswordComponent },
      { path: 'resetpassword', component: ResetPasswordComponent },
      {
        path: 'tracingSearch',
        component: TracingSearchComponent,
        data: { title: 'Request' },
      },

      {
        path: 'tracingSearch/consumerSearchResult',
        component: ConsumerSearchResultComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/addressSearchResult',
        component: AddressSearchResultComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/consumergloSearchResult',
        component: ConsumerSearchResultComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/consumerSearchResult/:id',
        component: ConsumerSearchResultComponent,
      },

      {
        path: 'tracingSearch/commercialSearchResult',
        component: CommercialSearchResultComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/commercialgloSearchResult',
        component: CommercialSearchResultComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/commercialSearchResult/:id',
        component: CommercialSearchResultComponent,
      },

      {
        path: 'tracingSearch/personProfile',
        component: PersonProfileComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/personProfilelink',
        component: PersonProfileComponent,
        data: { title: 'Request' },
      },
      {
        path: 'tracingSearch/companydetailresult',
        component: ComapanydtetailResultComponent,
        data: { title: 'Request' },
      },

      {
        path: 'batchprocess/batchprocessdetail',
        component: BatchProcessComponent,
      },
      {
        path: 'batchprocess/batchprocessview',
        component: BatchProcessViewComponent,
      },
      {
        path: 'batchprocess/batchprocessview/:id',
        component: BatchProcessViewComponent,
      },
      { path: 'batchprocess', component: BatchProcessListComponent },

      { path: 'leadGeneration/leadresponse', component: LeadResponseComponent },
      {
        path: 'leadGeneration/leadresponse/:id',
        component: LeadResponseComponent,
        data: { title: 'Request' },
      },
      { path: 'leadGeneration', component: LeadListComponent },
      { path: 'leadGeneration/process', component: LeadprocessComponent },
      { path: 'leadGeneration/process/:id', component: LeadprocessComponent },
      {
        path: 'leadGeneration/information/:id',
        component: LeadInformationComponent,
      },

      { path: 'invoicelist', component: InvoiceListComponent },
      {
        path: 'invoicelist/invoicedetail',
        component: InvoiceDetailComponent,
        data: { title: 'Request' },
      },

      { path: 'fullauditreport', component: FullAuditReportComponent },
      { path: 'fullauditreport/:id', component: FullAuditReportComponent },

      { path: 'summaryfullaudit', component: SummaryFullAuditComponent },
      { path: 'summaryfullaudit/:id', component: SummaryFullAuditComponent },

      { path: 'dsa', component: DsaComponent, data: { title: 'Request' } },

      { path: 'forgetpassword', component: ForgetPasswordComponent },
      { path: 'deedsinformation', component: DeedsInformationComponent },
      {
        path: 'directordetialsearchresult',
        component: DirectorDetialSearchresultComponent,
      },
      {
        path: 'propertyownershipdetialsresults',
        component: PropertyOwnerShipDetialsresultComponent,
      },
      { path: 'directorshiplist', component: DirectorShipListComponent },
      { path: 'employmentlist', component: EmploymentListComponent },
      { path: 'contactdetails', component: ContactDetailsComponent },
      { path: 'addresslist', component: AddressListComponent },
      { path: 'consumerjudgment', component: ConsumerjudgmentComponent },
      {
        path: 'consumerjudgementDetailComponent',
        component: ConsumerjudgementDetailComponent,
      },
      {
        path: 'consumerDebtReviewComponent',
        component: ConsumerDebtReviewComponent,
      },
      {
        path: 'ConsumerDebtDetailComponent',
        component: ConsumerDebtDetailComponent,
      },
      { path: 'donotcallregistry', component: DonotCallRegistryComponent },

      { path: 'Subscriptions', component: SubscriptionComponent },
      { path: 'Subscriptions/assignusers', component: SubscriptionComponent },
      {
        path: 'Subscriptions/subscriptiondetail',
        component: SubscriptionDetailComponent,
        data: { title: 'Request' },
      },

      { path: 'userlist', component: userlistComponent },
      { path: 'pdfviewer', component: PdfviewerComponent },
      {
        path: 'userlist/userdetails',
        component: userdetailsComponent,
        data: { title: 'Request' },
      },
      { path: 'pdfviewer', component: PdfviewerComponent },
      { path: 'xdslogin/:token', component: XdsLoginComponent },
      //{ path: 'xdslogin', component: XdsLoginComponent },
      { path: 'xdsloginerror', component: XdsLoginErrorComponent },
    ]),
  ],
  entryComponents: [
    AddressDetailComponent,
    CommercialAuditorDetailComponent,
    SubscriptionLicenceUserListComponent,
    PopupComponent,
    MessagePopupComponent,
  ],
  providers: [
    SecurityService,
    TracingService,
    RecaptchaService,
    headernavService,
    SubscriptionService,
    CompanyService,
    PersonProfileService,
    BatchTracingService,
    UserService,
    DatePipe,
    InvoiceService,
    FullAuditReportService,
    SummaryFullAuditService,
    FooterService,
    PopupService,
    RelationshipService,
    DashboardService,
    LeadGenerationService,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
