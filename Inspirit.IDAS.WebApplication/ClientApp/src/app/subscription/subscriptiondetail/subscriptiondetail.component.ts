import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { SubscriptionService, ProductPackageRate, ProductsVm ,SubscriptionVm } from '../../services/services';
import { DatePipe } from '@angular/common';
import { headernavService } from '../../header-nav/header-nav.service';


@Component({
    selector: 'app-subscriptiondetail',
    templateUrl: './subscriptiondetail.component.html',

})

export class SubscriptionDetailComponent {
    public  mode: string = "view";
    currentObject: SubscriptionVm = new SubscriptionVm();
    type: any;
    customerid: any;
    readonly: boolean = false;
    sub: any;
    name: any;
    id: any;
    prodname: string;
    rates: ProductPackageRate[];
    public products: ProductsVm[];
    public prod: ProductsVm = new ProductsVm();
  public errormsg: any = '';
  isuserExists: any;
   loading: boolean = false;
    constructor(public subscriptionService: SubscriptionService, public router: Router, public route: ActivatedRoute, public datepipe: DatePipe, public headernavService:headernavService) {
        this.subscriptionService.getData().subscribe((result) => {
            this.products = result;
        });
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

      this.sub = this.route.queryParams.subscribe(params => {
        this.id = params['id'];
      });
      if (typeof this.id != 'undefined' && typeof this.id != null) {
        this.mode = "view";
        this.subscriptionService.getSubscription(this.id).subscribe((result) => {
          this.currentObject = result;
          this.prod.billingType = this.currentObject.billingType;
          this.prod.duration = this.currentObject.duration;
          this.prod.name = this.currentObject.productName;
          this.prod.quantity = this.currentObject.quantity;
          this.prod.startDate = this.datepipe.transform(this.currentObject.startDate, 'dd-MM-yyyy');
          this.prod.id = this.currentObject.productId;
          this.readonly = true;
          this.loading = false;
        });
      }
      else {
        this.mode = "add";
        this.readonly = false;
      }
    } else {
      this.router.navigate(['/login']);
    }
    
    }

    submit() {

      
        if (this.prod.id != null && this.prod.billingType != null && this.prod.quantity != null && this.prod.startDate)
        {
            this.mode = "add";
            if ((typeof window !== "undefined") ? window.localStorage : null) {
                this.customerid = localStorage.getItem('customerid');
            }
            this.prod.rates = this.rates;
            this.subscriptionService.addSubscription(this.prod, this.customerid).subscribe((result) => {
            });
            this.router.navigate(['Subscriptions']);
        }
        else
        {
            this.errormsg = "All mandatory fields required";
        }
    }

  


    list()
    {
        this.router.navigate(['Subscriptions']);
    }
}
