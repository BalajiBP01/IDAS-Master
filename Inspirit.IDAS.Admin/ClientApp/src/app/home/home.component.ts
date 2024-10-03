import { Component, OnInit, transition } from '@angular/core';
import { AsideNavService } from '../aside-nav/aside-nav.service';
import { DashboardService, CustomerCount, CustomerUserCount, DonotCallRegCount, CustomerLog, ContactUsCount, InvoiceCount, PaymentCount, SubscriptionCount } from '../services/services';
import { Tree } from '@angular/router/src/utils/tree';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {
  constructor(public asideNavService: AsideNavService, public dashboardService: DashboardService) {
    this.asideNavService.toggle(true);
  }

  //customer
  customerCount: CustomerCount = new CustomerCount();
  customeraddredper: any;
  customeractiveper: any;
  customerdeactiveper: any;
  customerpendingper: any;

  //customerUser
  customerUserCount: CustomerUserCount = new CustomerUserCount();
  customeruseraddredper: any;
  customeruseractiveper: any;
  customeruserdeactiveper: any;

  //Donot call reg
  donotcallregCount: DonotCallRegCount = new DonotCallRegCount();
  donotcallper: any;

  //Customer Logs
  customerlogs: CustomerLog[];

  //Contact Us
  contactuscount: ContactUsCount = new ContactUsCount();
  contactusreceived: any;
  contactusnotres: any;

  //Invoice
  invoicedetail: InvoiceCount = new InvoiceCount();
  invoiceSent: any;
  invoiceNotSent: any;
  invoiceTotal: any;
  invoiceCancelled: any;
  invoicecancelprev: any;
  taxinvoice: any;
  //payment
  paymentdetail: PaymentCount = new PaymentCount();
  paid: any;
  notpaid: any;
  shortpaid: any;

  prevpaid: any;
  prevnotpaid: any;
  prevshortpaid: any;

  paymenydone = "currentmonth"
  shortpayment = "currentmonth";
  paymentnotdone = "currentmonth"
  cancelinv = "currentmonth";

  //Subscription
  subdetail: SubscriptionCount = new SubscriptionCount();
  subactive: any;

  ngOnInit(): void {


    $('#curr').css("color", "#716aca");
    $('#paycurr').css("color", "#716aca");
    $('#notpaycurr').css("color", "#716aca");
    $('#cancurre').css("color", "#716aca");


    this.dashboardService.getCustCount().subscribe((response) => {
      this.customerCount = response;
      //Customer addred percentage
      this.customeraddredper = ((this.customerCount.monthTotal / this.customerCount.total) * 100).toFixed(0);
      if (this.customeraddredper != "NaN")
        $('#customertotalProgressbar').css("width", this.customeraddredper + "%");
      else {
        this.customeraddredper = "0";
        $('#customertotalProgressbar').css("width", "0" + "%");
      }
      //customer active percentage
      this.customeractiveper = ((this.customerCount.activeMonthtTotal / this.customerCount.activeTotalCount) * 100).toFixed(0);
      if (this.customeractiveper != "NaN")
        $('#customeractiveper').css("width", this.customeractiveper + "%");
      else {
        this.customeractiveper = "0";
        $('#customeractiveper').css("width", "0" + "%");
      }
      //customer deactive percentage
      this.customerdeactiveper = ((this.customerCount.inactiveMonthTotal / this.customerCount.inActiveTotalCount) * 100).toFixed(0);
      if (this.customerdeactiveper != "NaN")
        $('#customerdeactiveper').css("width", this.customerdeactiveper + "%");
      else {
        this.customerdeactiveper = "0";
        $('#customerdeactiveper').css("width", "0" + "%");
      }
      this.customerpendingper = ((this.customerCount.pendingMonthTotal / (this.customerCount.activeMonthtTotal + this.customerCount.pendingMonthTotal)) * 100).toFixed(0);
      if (this.customerpendingper != "NaN")
        $('#customerpendingper').css("width", this.customerpendingper + "%");
      else {
        this.customerpendingper = "0";
        $('#customerpendingper').css("width", "0" + "%");
      }
      this.customeruser();
    });

  }

  customeruser() {
    this.dashboardService.getCustuserCount().subscribe((response) => {
      this.customerUserCount = response;
      //Customer addred percentage
      this.customeruseraddredper = ((this.customerUserCount.monthTotal / this.customerUserCount.total) * 100).toFixed(0);
      if (this.customeruseraddredper != "NaN")
        $('#customerusertotalProgressbar').css("width", this.customeruseraddredper + "%");
      else {
        this.customeruseraddredper = "0";
        $('#customerusertotalProgressbar').css("width", "0" + "%");
      }
      //customer active percentage
      this.customeruseractiveper = ((this.customerUserCount.activeMonthtTotal / this.customerUserCount.activeTotalCount) * 100).toFixed(0);
      if (this.customeruseractiveper != "NaN")
        $('#customeruseractiveper').css("width", this.customeruseractiveper + "%");
      else {
        this.customeruseractiveper = "0";
        $('#customeruseractiveper').css("width", "0" + "%");
      }
      //customer deactive percentage
      this.customeruserdeactiveper = ((this.customerUserCount.inactiveMonthTotal / this.customerUserCount.inActiveTotalCount) * 100).toFixed(0);
      if (this.customeruserdeactiveper != "NaN")
        $('#customeruserdeactiveper').css("width", this.customeruserdeactiveper + "%");
      else {
        this.customeruserdeactiveper = "0";
        $('#customeruserdeactiveper').css("width", "0" + "%");
      }
      this.donotcall();
    });
  }

  donotcall() {
    this.dashboardService.getDonotCallCount().subscribe((response) => {
      this.donotcallregCount = response;
      this.donotcallper = ((this.donotcallregCount.monthTotal / this.donotcallregCount.total) * 100).toFixed(0);
      if (this.donotcallper != "NaN")
        $('#donotcallregbar').css("width", this.donotcallper + "%");
      else {
        this.donotcallper = "0";
        $('#donotcallregbar').css("width", "0" + "%");
      }
      this.dashboardService.customerLog().subscribe((response) => {
        this.customerlogs = response;

        this.custlog();
      });
    });
  }

  custlog() {

    this.dashboardService.getContactus().subscribe((response) => {
      this.contactuscount = response;
      this.contactusreceived = ((this.contactuscount.monthTotal / this.contactuscount.total) * 100).toFixed(0);
      if (this.contactusreceived != "NaN")
        $('#contactustotal').css("width", this.contactusreceived + "%");
      else {
        this.contactusreceived = "0";
        $('#contactustotal').css("width", "0" + "%");
      }
      this.contactusnotres = ((this.contactuscount.notActionedMonth / this.contactuscount.notActionedTotal) * 100).toFixed(0);
      if (this.contactusnotres != "NaN")
        $('#contactuspending').css("width", this.contactusnotres + "%");
      else {
        this.contactusnotres = "0";
        $('#contactuspending').css("width", "0" + "%");
      }
      this.invoice();
    });
  }
  invoice() {
    this.dashboardService.getInvoice().subscribe((response) => {
      this.invoicedetail = response;

      this.invoiceSent = ((this.invoicedetail.sentCustMonth / this.invoicedetail.sentCustTotal) * 100).toFixed(0);
      if (this.invoiceSent != "NaN")
        $('#invoicesentid').css("width", this.invoiceSent + "%");
      else {
        this.invoiceSent = "0";
        $('#invoicesentid').css("width", "0" + "%");
      }
      this.invoiceNotSent = ((this.invoicedetail.notSentMonth / this.invoicedetail.notSentTotal) * 100).toFixed(0);
      if (this.invoiceNotSent != "NaN")
        $('#invoicenotsentid').css("width", this.invoiceNotSent + "%");
      else {
        this.invoiceNotSent = "0"
        $('#invoicenotsentid').css("width", "0" + "%");
      }
      this.invoiceTotal = ((this.invoicedetail.monthTotal / this.invoicedetail.total) * 100).toFixed(0);
      if (this.invoiceTotal != "NaN")
        $('#invoicetotal').css("width", this.invoiceTotal + "%");
      else {
        this.invoiceTotal = "0";
        $('#invoicetotal').css("width", "0" + "%");
      }
      this.invoiceCancelled = ((this.invoicedetail.cancelMonth / this.invoicedetail.cancelTotal) * 100).toFixed(0);
      if (this.invoiceCancelled != "NaN")
        $('#invoicecancel').css("width", this.invoiceCancelled + "%");
      else {
        this.invoiceCancelled = "0";
        $('#invoicecancel').css("width", "0" + "%");
      }

      this.invoicecancelprev = ((this.invoicedetail.cancelPrevMonth / this.invoicedetail.cancelTotal) * 100).toFixed(0);
      if (this.invoicecancelprev != "NaN")
        $('#invoicecancelprev').css("width", this.invoicecancelprev + "%");
      else {
        this.invoicecancelprev = "0";
        $('#invoicecancelprev').css("width", "0" + "%");
      }

      this.taxinvoice = ((this.invoicedetail.taxinvMonth / this.invoicedetail.taxinvTotal) * 100).toFixed(0);
      if (this.taxinvoice != "NaN")
        $('#tsxinv').css("width", this.taxinvoice + "%");
      else {
        this.taxinvoice = "0";
        $('#tsxinv').css("width", "0" + "%");
      }
      this.payment();
    });
  }
  payment() {
    this.dashboardService.getPaymentdetail().subscribe((response) => {
      this.paymentdetail = response;

      this.paid = ((this.paymentdetail.paidMonth / this.paymentdetail.paidTotal) * 100).toFixed(0);
      if (this.paid != NaN)
        $('#paid').css("width", this.paid + "%");
      else {
        this.paid = "0";
        $('#paid').css("width", "0" + "%");
      }
      this.notpaid = ((this.paymentdetail.notPaidMonth / this.paymentdetail.notPaidTotal) * 100).toFixed(0);
      if (this.notpaid != "NaN")
        $('#notpaid').css("width", this.notpaid + "%");
      else {
        this.notpaid = "0";
        $('#notpaid').css("width", "0" + "%");
      }
      this.shortpaid = ((this.paymentdetail.shortPaidMonth / this.paymentdetail.shortPaidTotal) * 100).toFixed(0);
      if (this.shortpaid != "NaN")
        $('#shortpaid').css("width", this.shortpaid + "%");
      else {
        this.shortpaid = "0";
        $('#shortpaid').css("width", "0" + "%");
      }

      this.prevpaid = ((this.paymentdetail.paidprevmonth / this.paymentdetail.paidTotal) * 100).toFixed(0);
      if (this.prevpaid != "NaN")
        $('#prevpaid').css("width", this.prevpaid + "%");
      else {
        this.prevpaid = "0";
        $('#prevpaid').css("width", "0" + "%");
      }

      this.prevnotpaid = ((this.paymentdetail.notPaidPrevmonth / this.paymentdetail.notPaidTotal) * 100).toFixed(0);
      if (this.prevnotpaid != "NaN")
        $('#prevnotpaid').css("width", this.prevnotpaid + "%");
      else {
        this.prevnotpaid = "0";
        $('#prevnotpaid').css("width", "0" + "%");
      }
      this.prevshortpaid = ((this.paymentdetail.shortPaidprevmonth / this.paymentdetail.shortPaidTotal) * 100).toFixed(0);
      if (this.prevshortpaid != "NaN")
        $('#prevshortpaid').css("width", this.prevshortpaid + "%");
      else {
        this.prevshortpaid = "0";
        $('#prevshortpaid').css("width", "0" + "%");
      }
      this.subscription();
    });
  }
  subscription() {
    this.dashboardService.getSubscriptiondet().subscribe((result) => {
      this.subdetail = result;
      this.subactive = ((this.subdetail.monthTotal / this.subdetail.total) * 100).toFixed(0);
      if (this.subactive != "NaN")
        $('#subactive').css('width', this.subactive + "%");
      else {
        this.subactive = "0";
        $('#subactive').css('width', "0" + "%");
      }
    });
  }

  prev() {
    this.shortpayment = "lastmonth";
    this.payment();
    $('#curr').css("color", "#6f727d");
    $('#prev').css("color", "#716aca");
  }
  curr() {
    this.shortpayment = "currentmonth";
    this.payment();
    $('#prev').css("color", "#6f727d");
    $('#curr').css("color", "#716aca");
  }
  paylast() {
    this.paymenydone = "lastmonth";
    this.payment();
    $('#paycurr').css("color", "#6f727d");
    $('#paylast').css("color", "#716aca");
  }
  paycurr() {
    this.paymenydone = "currentmonth";
    this.payment();
    $('#paylast').css("color", "#6f727d");
    $('#paycurr').css("color", "#716aca");
  }
  notpaylast() {
    this.paymentnotdone = "lastmonth";
    this.payment();
    $('#notpaycurr').css("color", "#6f727d");
    $('#notpaylast').css("color", "#716aca");
  }
  notpaycurr() {
    this.paymentnotdone = "currentmonth";
    this.payment();
    $('#notpaylast').css("color", "#6f727d");
    $('#notpaycurr').css("color", "#716aca");
  }
  cancurre() {
    this.cancelinv = "currentmonth";
    this.invoice();
    $('#canprev').css("color", "#6f727d");
    $('#cancurre').css("color", "#716aca");
  }
  canprev() {
    this.cancelinv = "lastmonth";
    this.invoice();
    $('#cancurre').css("color", "#6f727d");
    $('#canprev').css("color", "#716aca");
  }
}
