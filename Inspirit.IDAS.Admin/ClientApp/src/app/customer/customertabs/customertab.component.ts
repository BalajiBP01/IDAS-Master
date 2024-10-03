import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient, HttpRequest, HttpEventType, HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { debug, isNullOrUndefined } from 'util';
import {
  CustomerProdcutVm, Product,
  CustomerProductProperty, CustomerService, CustomerCrudResponse, UserService, UserPermission
} from '../../services/services';
import { AsideNavService } from '../../aside-nav/aside-nav.service';
@Component({
  selector: 'app-customerTab.component',
  templateUrl: './customerTab.component.html'
})

export class customerTabsComponent implements OnInit {
  id: any;
  errorMessageValue: any = "";
  sub: any;
  mode: string = "View";
  companyname: string = "";
  readonly: boolean = true;
  statusOption: string;
  userid: any;
  userper: UserPermission = new UserPermission();
  tabsSelected: Array<string> = new Array<string>();
  tabAdd: string="";
  tabres: any;


  constructor(public router: Router, private route: ActivatedRoute,
    private customerService: CustomerService, private http: HttpClient, public userService: UserService, public asideNavService: AsideNavService) {
    this.asideNavService.toggle(true);

    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "Customers").subscribe(result => {
      this.userper = result;
      if (this.userper == null || this.userper.viewAction == false) {
        document.getElementById('nopermission').click();
      }
    });

  }
  ngOnInit(): void {
    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
    });
    this.customerService.getName(this.id).subscribe(result => {
      this.companyname = result;
    });
    this.customerService.getTabs(this.id).subscribe((result) => {
      this.tabres = result;
      this.validation(this.tabres);
    });
  }
  edit() {
    this.mode = 'Edit';
    this.readonly = false;
  }

  Submit() {

    this.tabsSelected.forEach(res => {
      this.tabAdd += res + ',';
    });
    console.log(this.tabAdd);
    if (this.tabAdd != null && this.tabAdd != "undefined") {
      this.customerService.addTabs(this.tabAdd, this.id).subscribe((res) => {
        this.errorMessageValue = res;
        document.getElementById('error').click();
        this.mode = 'View';
        this.readonly = true;
      });
    } else {
      document.getElementById('error1').click();
    }
  }

  validation(data: string) {
    debugger;
    let ind = this.tabsSelected.indexOf("undefined", 1);
    this.tabsSelected.splice(ind, 1);

    var consumertimeline = <HTMLInputElement>document.getElementById("ConsumerTimeline");
    if (data.includes('ConsumerTimeline')) {
      consumertimeline.checked = true;
      this.tabsSelected.push('ConsumerTimeline');
    } else {
      consumertimeline.checked = false;
    }

    var ConsumerProfile = <HTMLInputElement>document.getElementById("ConsumerProfile");
    if (data.includes('ConsumerProfile')) {
      ConsumerProfile.checked = true;
      this.tabsSelected.push('ConsumerProfile');
    } else {
      ConsumerProfile.checked = false;
    }

    var ConsumerTelephone = <HTMLInputElement>document.getElementById("ConsumerTelephone");
    if (data.includes('ConsumerTelephone')) {
      ConsumerTelephone.checked = true;
      this.tabsSelected.push('ConsumerTelephone');
    } else {
      ConsumerTelephone.checked = false;
    }

    var ConsumerAddress = <HTMLInputElement>document.getElementById("ConsumerAddress");
    if (data.includes('ConsumerAddress')) {
      ConsumerAddress.checked = true;
      this.tabsSelected.push('ConsumerAddress');
    } else {
      ConsumerAddress.checked = false;
    }

    var ConsumerEmployment = <HTMLInputElement>document.getElementById("ConsumerEmployment");
    if (data.includes('ConsumerEmployment')) {
      ConsumerEmployment.checked = true;
      this.tabsSelected.push('ConsumerEmployment');
    } else {
      ConsumerEmployment.checked = false;
    }

    var ConsumerDirector = <HTMLInputElement>document.getElementById("ConsumerDirector");
    if (data.includes('ConsumerDirector')) {
      ConsumerDirector.checked = true;
      this.tabsSelected.push('ConsumerDirector');
    } else {
      ConsumerDirector.checked = false;
    }

    var ConsumerProperty = <HTMLInputElement>document.getElementById("ConsumerProperty");
    if (data.includes('ConsumerProperty')) {
      ConsumerProperty.checked = true;
      this.tabsSelected.push('ConsumerProperty');
    } else {
      ConsumerProperty.checked = false;
    }

    var ConsumerJudgement = <HTMLInputElement>document.getElementById("ConsumerJudgement");
    if (data.includes('ConsumerJudgement')) {
      ConsumerJudgement.checked = true;
      this.tabsSelected.push('ConsumerJudgement');
    } else {
      ConsumerJudgement.checked = false;
    }

    var ConsumerDebtReview = <HTMLInputElement>document.getElementById("ConsumerDebtReview");
    if (data.includes('ConsumerDebtReview')) {
      ConsumerDebtReview.checked = true;
      this.tabsSelected.push('ConsumerDebtReview');
    } else {
      ConsumerDebtReview.checked = false;
    }

    var ConsumerRelationship = <HTMLInputElement>document.getElementById("ConsumerRelationship");
    if (data.includes('ConsumerRelationship')) {
      ConsumerRelationship.checked = true;
      this.tabsSelected.push('ConsumerRelationship');
    } else {
      ConsumerRelationship.checked = false;
    }

    var CommercialTimeline = <HTMLInputElement>document.getElementById("CommercialTimeline");
    if (data.includes('CommercialTimeline')) {
      CommercialTimeline.checked = true;
      this.tabsSelected.push('CommercialTimeline');
    } else {
      CommercialTimeline.checked = false;
    }

    var CommercialProfile = <HTMLInputElement>document.getElementById("CommercialProfile");
    if (data.includes('CommercialProfile')) {
      CommercialProfile.checked = true;
      this.tabsSelected.push('CommercialProfile');
    } else {
      CommercialProfile.checked = false;
    }

    var CommercialTelephone = <HTMLInputElement>document.getElementById("CommercialTelephone");
    if (data.includes('CommercialTelephone')) {
      CommercialTelephone.checked = true;
      this.tabsSelected.push('CommercialTelephone');
    } else {
      CommercialTelephone.checked = false;
    }

    var CommercialAddress = <HTMLInputElement>document.getElementById("CommercialAddress");
    if (data.includes('CommercialAddress')) {
      CommercialAddress.checked = true;
      this.tabsSelected.push('CommercialAddress');
    } else {
      CommercialAddress.checked = false;
    }

    var CommercialDirector = <HTMLInputElement>document.getElementById("CommercialDirector");
    if (data.includes('CommercialDirector')) {
      CommercialDirector.checked = true;
      this.tabsSelected.push('CommercialDirector');
    } else {
      CommercialDirector.checked = false;
    }

    var CommercialJudgement = <HTMLInputElement>document.getElementById("CommercialJudgement");
    if (data.includes('CommercialJudgement')) {
      CommercialJudgement.checked = true;
      this.tabsSelected.push('CommercialJudgement');
    } else {
      CommercialJudgement.checked = false;
    }

    var CommercialProperty = <HTMLInputElement>document.getElementById("CommercialProperty");
    if (data.includes('CommercialProperty')) {
      CommercialProperty.checked = true;
      this.tabsSelected.push('CommercialProperty');
    } else {
      CommercialProperty.checked = false;
    }

    var CommercialAuditor = <HTMLInputElement>document.getElementById("CommercialAuditor");
    if (data.includes('CommercialAuditor')) {
      CommercialAuditor.checked = true;
      this.tabsSelected.push('CommercialAuditor');
    } else {
      CommercialAuditor.checked = false;
    }
  }

  addtabs(tabname: any) {
    switch (tabname) {
     
      case 'ConsumerTimeline':
        var consumertimeline = <HTMLInputElement>document.getElementById("ConsumerTimeline");
        console.log(consumertimeline.checked);
        if (consumertimeline.checked == true) {
          this.tabsSelected.push('ConsumerTimeline');
        } else {
          debugger;
          const index = this.tabsSelected.indexOf('ConsumerTimeline');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerProfile':
        var consumerprofile = <HTMLInputElement>document.getElementById("ConsumerProfile");
        if (consumerprofile.checked == true) {
          this.tabsSelected.push('ConsumerProfile');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerProfile');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerTelephone':
        var ConsumerTelephone = <HTMLInputElement>document.getElementById("ConsumerTelephone");
        if (ConsumerTelephone.checked == true) {
          this.tabsSelected.push('ConsumerTelephone');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerTelephone');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerAddress':
        var ConsumerAddress = <HTMLInputElement>document.getElementById("ConsumerAddress");
        if (ConsumerAddress.checked == true) {
          this.tabsSelected.push('ConsumerAddress');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerAddress');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerEmployment':
        var ConsumerEmployment = <HTMLInputElement>document.getElementById("ConsumerEmployment");
        if (ConsumerEmployment.checked == true) {
          this.tabsSelected.push('ConsumerEmployment');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerEmployment');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerDirector':
        var ConsumerDirector = <HTMLInputElement>document.getElementById("ConsumerDirector");
        if (ConsumerDirector.checked == true) {
          this.tabsSelected.push('ConsumerDirector');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerDirector');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerProperty':
        var ConsumerProperty = <HTMLInputElement>document.getElementById("ConsumerProperty");
        if (ConsumerProperty.checked == true) {
          this.tabsSelected.push('ConsumerProperty');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerProperty');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerJudgement':
        var ConsumerJudgement = <HTMLInputElement>document.getElementById("ConsumerJudgement");
        if (ConsumerJudgement.checked == true) {
          this.tabsSelected.push('ConsumerJudgement');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerJudgement');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerDebtReview':
        var ConsumerDebtReview = <HTMLInputElement>document.getElementById("ConsumerDebtReview");
        if (ConsumerDebtReview.checked == true) {
          this.tabsSelected.push('ConsumerDebtReview');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerDebtReview');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'ConsumerRelationship':
        var ConsumerRelationship = <HTMLInputElement>document.getElementById("ConsumerRelationship");
        if (ConsumerRelationship.checked == true) {
          this.tabsSelected.push('ConsumerRelationship');
        } else {
          const index = this.tabsSelected.indexOf('ConsumerRelationship');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialTimeline':
        var CommercialTimeline = <HTMLInputElement>document.getElementById("CommercialTimeline");
        if (CommercialTimeline.checked == true) {
          this.tabsSelected.push('CommercialTimeline');
        } else {
          const index = this.tabsSelected.indexOf('CommercialTimeline');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialProfile':
        var CommercialProfile = <HTMLInputElement>document.getElementById("CommercialProfile");
        if (CommercialProfile.checked == true) {
          this.tabsSelected.push('CommercialProfile');
        } else {
          const index = this.tabsSelected.indexOf('CommercialProfile');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialTelephone':
        var CommercialTelephone = <HTMLInputElement>document.getElementById("CommercialTelephone");
        if (CommercialTelephone.checked == true) {
          this.tabsSelected.push('CommercialTelephone');
        } else {
          const index = this.tabsSelected.indexOf('CommercialTelephone');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;
   
      case 'CommercialAddress':
        var CommercialAddress = <HTMLInputElement>document.getElementById("CommercialAddress");
        if (CommercialAddress.checked == true) {
          this.tabsSelected.push('CommercialAddress');
        } else {
          const index = this.tabsSelected.indexOf('CommercialAddress');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialDirector':
        var CommercialDirector = <HTMLInputElement>document.getElementById("CommercialDirector");
        if (CommercialDirector.checked == true) {
          this.tabsSelected.push('CommercialDirector');
        } else {
          const index = this.tabsSelected.indexOf('CommercialDirector');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialJudgement':
        var CommercialJudgement = <HTMLInputElement>document.getElementById("CommercialJudgement");
        if (CommercialJudgement.checked == true) {
          this.tabsSelected.push('CommercialJudgement');
        } else {
          const index = this.tabsSelected.indexOf('CommercialJudgement');
          if (index > -1)
            this.tabsSelected.splice(index, 19);
        }
        break;

      case 'CommercialProperty':
        var CommercialProperty = <HTMLInputElement>document.getElementById("CommercialProperty");
        if (CommercialProperty.checked == true) {
          this.tabsSelected.push('CommercialProperty');
        } else {
          const index = this.tabsSelected.indexOf('CommercialProperty');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      case 'CommercialAuditor':
        var CommercialAuditor = <HTMLInputElement>document.getElementById("CommercialAuditor");
        if (CommercialAuditor.checked == true) {
          this.tabsSelected.push('CommercialAuditor');
        } else {
          const index = this.tabsSelected.indexOf('CommercialAuditor');
          if (index > -1)
            this.tabsSelected.splice(index, 1);
        }
        break;

      default:
        break;
    }

  }
  list() {
    this.router.navigate(['customerlist']);
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
