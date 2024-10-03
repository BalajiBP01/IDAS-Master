import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { DataTablesModule } from 'angular-datatables';
import { LoadingModule } from 'ngx-loading';

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { CKEditorModule } from 'ng2-ckeditor';

import { QuillEditorModule } from 'ngx-quill-editor';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Routes, RouterModule } from '@angular/router';
import { NgbModule, NgbModalModule } from "@ng-bootstrap/ng-bootstrap";
import * as _moment from 'moment';
import { DataTableModule } from 'angular2-datatable';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';

import { AdminLoginComponent } from './adminlogin/adminlogin.component';
import { HeaderNavComponent } from './header-nav/header-nav.component';
import { AsideNavComponent } from './aside-nav/aside-nav.component';

import { dsalistComponent } from './dsa/dsalist/dsalist.component';
import { dsadetailComponent } from './dsa/dsadetail/dsadetail.component';

import { lookuplistComponent } from './lookupdata/lookuplist/lookuplist.component';
import { lookupdetailComponent } from './lookupdata/lookupdetail/lookupdetail.component';

import { EmailTemplateListComponent } from './emailtemplate/emailtemplatelist/emailtemplatelist.component';
import { EmailTemplateDetailComponent } from './emailtemplate/emailtemplatedetail/emailtemplatedetail.component';

import { PaymentlistComponent } from './Payment/PaymentList/Paymentlist.Component';
import { PaymentdetailComponent } from './Payment/PaymentDetail/Paymentdetail.Component';

import { CreditnotelistComponent } from './creditnote/creditnotelist/creditnotelist.component';
import { CreditnotedetailComponent } from './creditnote/creditnotedetail/creditnotedetail.component';

import { InvoiceListComponent } from './invoice/invoicelist/invoicelist.component';
import { InvoiceDetailComponent } from './invoice/invoicedetail/invoicedetail.component';
import { InvoiceBulkEmailComponent } from './invoice/invoicebulkemail/invoicebulkemail.component';

import { ProFormaInvoiceListComponent } from './proformainvoice/proformainvoicelist/proformainvoicelist.component';
import { ProFormaInvoiceDetailComponent } from './proformainvoice/proformainvoicedetail/proformainvoicedetail.component';
import { ProformaInvoiceBulkEmailComponent } from './proformainvoice/proformainvoicebulkemail/proformainvoicebulkemail.component';


import { ProductServiceListComponent } from './productservices/productservicelist/productservicelist.component';
import { ProductServiceDetailComponent } from './productservices/productservicedetail/productservicedetail.component';

import { customerlistComponent } from './customer/customerlist/customerlist.component';
import { customerdetailComponent } from './customer/customerdetails/customerdetails.component';
import { customerProductComponent } from './customer/customerproduct/customerproduct.component';

import { customeruserdetailsComponent } from './customerusers/customeruserdetails/customeruserdetails.component';
import { customeruserlistComponent } from './customerusers/customeruserlist/customeruserlist.component';
import {customerTabsComponent} from './customer/customertabs/customertab.component'
import { SubscriptionComponent } from './subscription/subscription.component';
import { SubscriptionDetailComponent } from './subscriptiondetail/subscriptiondetail.component';
import { SearchCustomer } from './searchcustomer/searchcustomer.component';

import { donotcallregistrylistComponent } from './donotcallregistry/donotcallregistrylist/dncrlist.component';
import { donotcallregistrydetailsComponent } from './donotcallregistry/donotcallregistrydetails/dncrdetails.component';
import { DonotcallregistryFileuploadcomponent } from './donotcallregistry/donotcallregistryFileupload/donotcallregistryFileupload.component';

import { ApplicationMessageDetailComponent } from './applicationmessage/applicationmessagedetail/applicationmessagedetail.component';
import { ApplicationMessagelistComponent } from './applicationmessage/applicationmessagelist/applicationmessagelist.component';

import { ContactusComponent } from './contactus/contactus.component';
import { ContactusDetailComponent } from './contactusdetail/contactusdetail.component';

import { appsettingdetailComponent } from './appsetting/appsettingdetail/appsettingdetail.component';
import { appsettinglistComponent } from './appsetting/appsettinglist/appsettinglist.component';

import { UserlistComponent } from './user/userlist/userlist.component';
import { UserdetailComponent } from './user/userdetail/userdetail.component';

import { ProductListComponent } from './product/productlist/productlist.component';
import { ProductdetailComponent } from './product/productdetails/productdetails.component';

import { BatchtracinglistComponent } from './batchtracing/batchtracinglist/batchtracinglist.component';
import {LeadlistComponent } from './leads/leadslist/leadlist.component';

import { PopupComponent } from './popup/popup.component';
import { PopupService } from './popup/popupService';
import 'datatables.net';
import { DatePipe } from '@angular/common';

import { NewsComponent } from './news/news.component';
import { NewsListComponent } from './newslist/newslist.component';

//services
import { HeaderService } from './header-nav/headerService';
import { AsideNavService } from './aside-nav/aside-nav.service';
import {
  SecurityService, CustomerService, CustomerUserService, DsaService, ProFormaInvoiceService, SubscriptionService, InvoiceService, AppSettingService, ProductService, ApplicationMessageService, EmailTemplateService,
  LookupDataService, DataTableRequest, UserService, ContactusService, PaymentService, DoNotCallRegistryService, ProdservService, BatchTracingService, DashboardService, NewsService, InvoiceBulkEmail, LeadGenerationService
} from './services/services';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AdminLoginComponent,
    HeaderNavComponent,
    AsideNavComponent,
    dsalistComponent,
    dsadetailComponent,
    lookuplistComponent,
    lookupdetailComponent,
    EmailTemplateListComponent,
    EmailTemplateDetailComponent,
    InvoiceListComponent,
    InvoiceDetailComponent,
    InvoiceBulkEmailComponent,
    ApplicationMessagelistComponent,
    ContactusComponent,
    ContactusDetailComponent,
    ProductListComponent,
    ProductdetailComponent,
    ApplicationMessageDetailComponent,
    appsettinglistComponent,
    appsettingdetailComponent,
    ProductServiceListComponent,
    ProductServiceDetailComponent,
    customerdetailComponent,
    customerProductComponent,
    customerlistComponent,
    customeruserlistComponent,
    customeruserdetailsComponent,
    EmailTemplateListComponent,
    donotcallregistrylistComponent,
    donotcallregistrydetailsComponent,
    UserlistComponent,
    UserdetailComponent,
    PaymentlistComponent,
    PaymentdetailComponent,
    SubscriptionComponent,
    SubscriptionDetailComponent,
    SearchCustomer,
    ProFormaInvoiceListComponent,
    ProFormaInvoiceDetailComponent,     
    ProformaInvoiceBulkEmailComponent,
    CreditnotelistComponent,
    CreditnotedetailComponent,
    DonotcallregistryFileuploadcomponent,
    PopupComponent,
    BatchtracinglistComponent,
    NewsComponent,
    NewsListComponent,
    LeadlistComponent,
    customerTabsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,

    CKEditorModule,


    DataTablesModule,
    NgbModule.forRoot(),
    NgbModalModule,
    QuillEditorModule,   
    LoadingModule,
    DataTableModule,   
    RouterModule.forRoot(
      [
        { path: '', component: AdminLoginComponent, pathMatch: 'full' },
        { path: 'dashboard', component: HomeComponent },

        { path: 'adminlogin', component: AdminLoginComponent },

        { path: 'dsalist', component: dsalistComponent },
        { path: 'dsalist/dsadetail', component: dsadetailComponent },
        { path: 'dsalist/dsadetail/:id', component: dsadetailComponent },

        { path: 'lookuplist', component: lookuplistComponent },
        { path: 'lookuplist/lookupdetail', component: lookupdetailComponent },
        { path: 'lookuplist/lookupdetail/:id', component: lookupdetailComponent },

        { path: 'emailtemplatelist', component: EmailTemplateListComponent },
        { path: 'emailtemplatelist/emailtemplatedetail', component: EmailTemplateDetailComponent },
        { path: 'emailtemplatelist/emailtemplatedetail/:id', component: EmailTemplateDetailComponent },

        { path: 'paymentlist', component: PaymentlistComponent },
        { path: 'paymentlist/paymentlistcustomer', component: PaymentlistComponent },
        { path: 'paymentlist/paymentdetail', component: PaymentdetailComponent },
        { path: 'paymentlist/paymentdetail/:id', component: PaymentdetailComponent, data: { title: 'Request' } },

        { path: 'subscribtions', component: SubscriptionComponent },
        { path: 'subscribtions/subscriptionlistcustomer', component: SubscriptionComponent },
        { path: 'subscribtions/subscriptioncustomer', component: SubscriptionDetailComponent },

        { path: 'subscribtions/subscriptiondetail', component: SubscriptionDetailComponent },
        { path: 'subscribtions/subscriptiondetail/:id', component: SubscriptionDetailComponent },

        { path: 'productlist', component: ProductListComponent },
        { path: 'productdetails', component: ProductdetailComponent },
        { path: 'productlist/productdetails/:id', component: ProductdetailComponent },

        { path: 'customerlist/customerdetails', component: customerdetailComponent },
        { path: 'customerlist/customerproduct/:id', component: customerProductComponent },
        { path: 'customerlist', component: customerlistComponent },       
        { path: 'customerlist/customerdetails/:id', component: customerdetailComponent },
{ path: 'customerlist/customertabs/:id', component: customerTabsComponent},
        { path: 'customerlist/customeruserlist', component: customeruserlistComponent },
        { path: 'customerlist/customeruserlist/:id', component: customeruserlistComponent },

        { path: 'customerlist/customeruserdetails', component: customeruserdetailsComponent, data: { title: 'Request' } },
        { path: 'customerlist/customeruserdetails/:id', component: customeruserdetailsComponent, data: { title: 'Request' } },

        { path: 'dncrlist', component: donotcallregistrylistComponent },
        { path: 'dncrlist/dncrdetails/:id', component: donotcallregistrydetailsComponent },
        { path: 'dncrlist/dncfileupload', component: DonotcallregistryFileuploadcomponent },

        { path: 'applicationmessagelist', component: ApplicationMessagelistComponent },
        { path: 'applicationmessagelist/applicationmessagedetail', component: ApplicationMessageDetailComponent },
        { path: 'applicationmessagelist/applicationmessagedetail/:id', component: ApplicationMessageDetailComponent },

        { path: 'productservicelist', component: ProductServiceListComponent },
        { path: 'productservicelist/productservicedetail', component: ProductServiceDetailComponent },
        { path: 'productservicelist/productservicedetail/:id', component: ProductServiceDetailComponent },

        { path: 'invoicelist', component: InvoiceListComponent },
        { path: 'invoicelist/invoicecustomer', component: InvoiceListComponent },
        { path: 'invoicelist/invoicedetail', component: InvoiceDetailComponent },
        { path: 'invoicelist/invoicedetail/:id', component: InvoiceDetailComponent },
        { path: 'invoicelist/invoicebulkemail', component: InvoiceBulkEmailComponent },

        { path: 'proformainvoicelist', component: ProFormaInvoiceListComponent },
        { path: 'proformainvoicelist/proformainvoicelistcustomer', component: ProFormaInvoiceListComponent },
        { path: 'proformainvoicelist/proformainvoicedetail', component: ProFormaInvoiceDetailComponent },
        { path: 'proformainvoicelist/proformainvoicedetailcustomer', component: ProFormaInvoiceDetailComponent },
        { path: 'proformainvoicelist/proformainvoicedetail/:id', component: ProFormaInvoiceDetailComponent },
        { path: 'proformainvoicelist/proformainvoicebulkemail', component: ProformaInvoiceBulkEmailComponent },

        { path: 'appsettinglist', component: appsettinglistComponent },
        { path: 'appsettinglist/appsettingdetail', component: appsettingdetailComponent },
        { path: 'appsettinglist/appsettingdetail/:id', component: appsettingdetailComponent },

        { path: 'contactus', component: ContactusComponent },
        { path: 'contactus/contactusdetail', component: ContactusDetailComponent },
        { path: 'contactus/contactusdetail/:id', component: ContactusDetailComponent },

        { path: 'userlist', component: UserlistComponent },
        { path: 'userlist/userdetail', component: UserdetailComponent },
        { path: 'userlist/userdetail/:id', component: UserdetailComponent },

        { path: 'creditnotelist', component: CreditnotelistComponent },
        { path: 'creditnotelist/creditnotedetail', component: CreditnotedetailComponent },
        { path: 'creditnotelist/creditnotedetail/:id', component: CreditnotedetailComponent },

        { path: 'batchtrace', component: BatchtracinglistComponent },
        { path: 'batchtrace/batchtracelistcustomer', component: BatchtracinglistComponent },

        { path: 'leadlist', component: LeadlistComponent },
         { path: 'leadlist/leadlistcustomer', component: LeadlistComponent },

        {path:'news',component:NewsListComponent},
        { path: 'news/newsdetail', component: NewsComponent },
        { path: 'news/newsdetail/:id', component: NewsComponent }
      ])
  ],
  entryComponents: [
    PopupComponent, SearchCustomer
  ],
  providers: [AsideNavService, SecurityService, CustomerService, CustomerUserService,
    UserService, SubscriptionService, InvoiceService,
    AppSettingService, ProductService, DsaService, DatePipe,
    LookupDataService, ApplicationMessageService, EmailTemplateService,
    AsideNavService, ContactusService, PaymentService, LeadGenerationService,
    DoNotCallRegistryService, ProdservService, ProFormaInvoiceService, HeaderService, PopupService, BatchTracingService, DashboardService, NewsService],
  bootstrap: [AppComponent],
})

export class AppModule { }
