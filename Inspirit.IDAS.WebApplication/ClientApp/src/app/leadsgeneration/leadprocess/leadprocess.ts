import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { LeadGenerationService, LeadsResponse, LeadsRequest, Gender, MaritalStatus, Alloy, LSM, RiskCategory, Province, IncomeCategory } from '../../services/services';
import { headernavService } from '../../header-nav/header-nav.service';
import { Options } from 'ng5-slider';
import { all } from 'q';
import { concat } from 'rxjs/operator/concat';
import { Tree } from '@angular/router/src/utils/tree';

@Component({
  selector: 'leadprocess',
  templateUrl: './leadprocess.html'
})
export class LeadprocessComponent {

  leadres: LeadsResponse[] = [];
  leadreq: LeadsRequest = new LeadsRequest();

  gender: Gender = new Gender();
  genderlst: Gender[] = [];
  mstatus: MaritalStatus = new MaritalStatus();
  mstatuslst: MaritalStatus[] = [];
  cscore: Alloy = new Alloy();
  cscorelst: Alloy[] = [];
  lsm: LSM = new LSM();
  lsmlist: LSM[] = [];
  riskcategory: RiskCategory = new RiskCategory();
  risklist: RiskCategory[] = [];
  province: Province = new Province();
  provincelst: Province[] = [];
  income: IncomeCategory = new IncomeCategory();
  incomelst: IncomeCategory[] = [];
  name: any;
  loading: boolean = false;
  userid: any;
  customerid: any;
  warningMesage: any;
  value: number = 18;
  highValue: number = 100;
  checked: boolean;
  options: Options = {
    floor: 18,
    ceil: 100
  };
  p1: number =0;
  p2: number=0;
  p3: number=0;
  p4: number=0;
  p5: number=0;
  p6: number=0;
  p7: number=0;
  p8: number=0;
  p9: number=0;

  gen1: number=0;
  gen2: number=0;

  mar1: number=0;
  mar2: number=0;

  allowprovince1: boolean = false;
  allowprovince2: boolean = false;
  allowprovince3: boolean = false;
  allowprovince4: boolean = false;
  allowprovince5: boolean = false;
  allowprovince6: boolean = false;
  allowprovince7: boolean = false;
  allowprovince8: boolean = false;
  allowprovince9: boolean = false;

  allowcontact: boolean = true;
  allowrisk: boolean = true;
  allowincome: boolean = true;
  allowlsm: boolean = true;
  desprovince: boolean = true;

  provincetotal: number=0;
  gentotal: number=0;
  maritaltotal: number=0;

  id: any;
  sub: any;

  inpstr: string;

  inputstring: LeadsRequest = new LeadsRequest;

  constructor(public LeadGenerationService: LeadGenerationService, public headernavservice: headernavService, public router: Router, public route: ActivatedRoute) {
    this.headernavservice.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.customerid = localStorage.getItem('customerid');
    this.name = localStorage.getItem('name');
    if (this.name != null && this.name != 'undefined') {
      this.headernavservice.updateUserName(this.name);
    }
  }
  ngOnInit(): void {
    this.loading = true;
    localStorage.removeItem('leadrequest');
    this.LeadGenerationService.checkLeadConfig(this.userid).subscribe((res) => {
      if (res != "") {
        this.warningMesage = res;
        document.getElementById("openPopUpLead").click();
      }
    });

    this.sub = this.route.params.subscribe(params => {
      this.id = params['id'];
      if (this.id != 'undefined' && this.id != null) {
        this.editform();
      } else {
        this.initialvalidation();
      }
    });

  }
  list() {
    this.router.navigate(['leadGeneration']);
  }
  submit() {


    this.leadreq = new LeadsRequest();
    this.cscorelst= [];
     this.genderlst=[];
    this.lsmlist=[];
    this.provincelst=[];
    this.risklist=[];
     this.mstatuslst=[];
    this.incomelst=[];

    this.gen1 = +$('#malevalue').val();
    this.gen2 = +$('#femalevalue').val();
    this.gentotal = this.gen1 + this.gen2;

    this.mar1 = +$('#singlevalue').val();
    this.mar2 = +$('#marriedvalue').val();
    this.maritaltotal = this.mar1 + this.mar2;


    this.p1 = + $('#natalvalue').val();
    this.p2 = + $('#northwestvalue').val();
    this.p3 = +$('#northernvalue').val();
    this.p4 = +$('#westernvalue').val();
    this.p5 = +$('#limpopovalue').val();
    this.p6 = +$('#easternvalue').val();
    this.p7 = +$('#gautengvalue').val();
    this.p8 = +$('#mpumalangavalue').val();
    this.p9 = +$('#freestatevalue').val();

    this.provincetotal = this.p1 + this.p2 + this.p3 + this.p4 + this.p5 + this.p6 + this.p7 + this.p8 + this.p9;
    var province10 = <HTMLInputElement>document.getElementById("selectallprovince");


    let leadcount = + $('#leadcount').val();
    if (leadcount < 100 || leadcount > 100000) {
      this.warningMesage = "Requested Lead count should be with in 100 to 100000 range."
      document.getElementById('openPopUpvalid1').click();
      return;
    }

    if (this.gentotal != 100) {
      this.warningMesage = "Gender selected total percentage should be equal to 100%."
      document.getElementById('openPopUpvalid1').click();
      return;
    }
    else if (this.maritaltotal != 100) {
      this.warningMesage = "Marital status selected percentage should be equal to 100%."
      document.getElementById('openPopUpvalid1').click();
      return;
    }
    else if (this.provincetotal != 100 && province10.checked == false) {

      this.warningMesage = "Province selected percentage should be equal to 100%."
      document.getElementById('openPopUpvalid1').click();
      return;
    }


    this.leadreq.customerId = this.customerid;
    this.leadreq.customerUserId = this.userid;
    this.leadreq.dateRange1 = + this.value;
    this.leadreq.dateRange2 = + this.highValue;

    this.leadreq.requiredLeads = + $('#leadcount').val();
    //addverse
    var adverse = <HTMLInputElement>document.getElementById("excludeadverse");
    this.leadreq.isAdversed = adverse.checked;
    //cell number
    var cellnumber = <HTMLInputElement>document.getElementById("iscellnumber");
    this.leadreq.isCellNumber = cellnumber.checked;
    //director
    var director = <HTMLInputElement>document.getElementById("isdirector");
    this.leadreq.isDirector = director.checked;
    //email
    var email = <HTMLInputElement>document.getElementById("isemailonly");
    this.leadreq.isEmail = email.checked;
    //house owner
    var houseowner = <HTMLInputElement>document.getElementById("ishomeowner");
    this.leadreq.isHomeOwner = houseowner.checked;
    //employed
    var employed = <HTMLInputElement>document.getElementById("isemployed");
    this.leadreq.isEmployed = employed.checked;
    //isDeceased

    var isDeceased = <HTMLInputElement>document.getElementById("isDeceased");
    this.leadreq.isDeceased = isDeceased.checked;

    //Gender list
    var malegender = <HTMLInputElement>document.getElementById("malebox");
    if (malegender.checked == true) {
      this.gender = new Gender();
      this.gender.genderName = "Male";
      this.gender.percentage = + $('#malevalue').val();
      this.genderlst.push(this.gender);
    }
    var femalegender = <HTMLInputElement>document.getElementById("femalebox");
    if (femalegender.checked == true) {
      this.gender = new Gender();
      this.gender.genderName = "Female";
      this.gender.percentage = + $('#femalevalue').val();
      this.genderlst.push(this.gender);
    }

    

    if (malegender.checked == false && femalegender.checked == false) {
      this.gender = new Gender();
      this.gender.genderName = "Male";
      this.gender.percentage = 50
      this.genderlst.push(this.gender);
      this.gender = new Gender();
      this.gender.genderName = "Female";
      this.gender.percentage = 50;
      this.genderlst.push(this.gender);
    }

    //marital information
    var single = <HTMLInputElement>document.getElementById("singlebox");
    if (single.checked == true) {
      this.mstatus = new MaritalStatus();
      this.mstatus.statusName = "Single";
      this.mstatus.percentage = + $('#singlevalue').val();
      this.mstatuslst.push(this.mstatus);
    }
    var married = <HTMLInputElement>document.getElementById("marriedbox");
    if (married.checked == true) {
      this.mstatus = new MaritalStatus();
      this.mstatus.statusName = "Married";
      this.mstatus.percentage = + $('#marriedvalue').val();
      this.mstatuslst.push(this.mstatus);
    }


    if (single.checked == false && married.checked == false) {
      this.mstatus = new MaritalStatus();
      this.mstatus.statusName = "Single";
      this.mstatus.percentage = 50
      this.mstatuslst.push(this.mstatus);
      this.mstatus = new MaritalStatus();
      this.mstatus.statusName = "Married";
      this.gender.percentage = 50;
      this.mstatuslst.push(this.mstatus);
    }

    //contact score
    var selectallcontact = <HTMLInputElement>document.getElementById("selectallcontact");
    var higlycontact = <HTMLInputElement>document.getElementById("highlycontact");
    if (higlycontact.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Highly contactable";
      this.cscorelst.push(this.cscore);
    }
    var verygood = <HTMLInputElement>document.getElementById("verygoodcontact");
    if (verygood.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Very Good";
      this.cscorelst.push(this.cscore);
    }
    var fair = <HTMLInputElement>document.getElementById("faircontact");
    if (fair.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Fair";
      this.cscorelst.push(this.cscore);
    }
    var averagecontact = <HTMLInputElement>document.getElementById("averagecontact");
    if (averagecontact.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Average";
      this.cscorelst.push(this.cscore);
    }
    var poorcontact = <HTMLInputElement>document.getElementById("poorcontact");
    if (poorcontact.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Poor";
      this.cscorelst.push(this.cscore);
    }

    var nocontact = <HTMLInputElement>document.getElementById("nocontact");
    if (nocontact.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "No Contact";
      this.cscorelst.push(this.cscore);
    }

    var rightcontact = <HTMLInputElement>document.getElementById("rightcontact");
    if (rightcontact.checked == true || selectallcontact.checked == true) {
      this.cscore = new Alloy();
      this.cscore.alloyName = "Right Party Contact";
      this.cscorelst.push(this.cscore);
    }



    //lsm
    var selectallLsm = <HTMLInputElement>document.getElementById('selectallLsm');
    var lsm0 = <HTMLInputElement>document.getElementById("lsm0");
    if (lsm0.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "0";
      this.lsmlist.push(this.lsm);
    }

    var lsm1 = <HTMLInputElement>document.getElementById("lsm1");
    if (lsm1.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "1";
      this.lsmlist.push(this.lsm);
    }
    var lsm2 = <HTMLInputElement>document.getElementById("lsm2");
    if (lsm2.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "2";
      this.lsmlist.push(this.lsm);
    }
    var lsm3 = <HTMLInputElement>document.getElementById("lsm3");
    if (lsm3.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "3";
      this.lsmlist.push(this.lsm);
    }
    var lsm4 = <HTMLInputElement>document.getElementById("lsm4");
    if (lsm4.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "4";
      this.lsmlist.push(this.lsm);
    }
    var lsm5 = <HTMLInputElement>document.getElementById("lsm5");
    if (lsm5.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "5";
      this.lsmlist.push(this.lsm);
    }
    var lsm6 = <HTMLInputElement>document.getElementById("lsm6");
    if (lsm6.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "6";
      this.lsmlist.push(this.lsm);
    }
    var lsm7 = <HTMLInputElement>document.getElementById("lsm7");
    if (lsm7.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "7";
      this.lsmlist.push(this.lsm);
    }
    var lsm8 = <HTMLInputElement>document.getElementById("lsm8");
    if (lsm8.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "8";
      this.lsmlist.push(this.lsm);
    }
    var lsm9 = <HTMLInputElement>document.getElementById("lsm9");
    if (lsm9.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "9";
      this.lsmlist.push(this.lsm);
    }
    var lsm10 = <HTMLInputElement>document.getElementById("lsm10");
    if (lsm10.checked == true || selectallLsm.checked == true) {
      this.lsm = new LSM();
      this.lsm.lsmName = "10";
      this.lsmlist.push(this.lsm);
    }

    //Risk catogory
    var selectallrisk = <HTMLInputElement>document.getElementById("selectallrisk");
    var lowrisk = <HTMLInputElement>document.getElementById("lowrisk");
    if (lowrisk.checked == true || selectallrisk.checked == true) {
      this.riskcategory = new RiskCategory();
      this.riskcategory.riskName = "Low Risk";
      this.risklist.push(this.riskcategory);
    }
    var extremelylow = <HTMLInputElement>document.getElementById("extremelylowrisk");
    if (extremelylow.checked == true || selectallrisk.checked == true) {
      this.riskcategory = new RiskCategory();
      this.riskcategory.riskName = "Extremely Low Risk";
      this.risklist.push(this.riskcategory);
    }
    var highrisk = <HTMLInputElement>document.getElementById("highrisk");
    if (highrisk.checked == true || selectallrisk.checked == true) {
      this.riskcategory = new RiskCategory();
      this.riskcategory.riskName = "High Risk";
      this.risklist.push(this.riskcategory);
    }
    var notcontact = <HTMLInputElement>document.getElementById("notcontactrisk");
    if (notcontact.checked == true || selectallrisk.checked == true) {
      this.riskcategory = new RiskCategory();
      this.riskcategory.riskName = "Should Not be Contacted";
      this.risklist.push(this.riskcategory);
    }
    var mediumrisk = <HTMLInputElement>document.getElementById("mediumrisk");
    if (mediumrisk.checked == true || selectallrisk.checked == true) {
      this.riskcategory = new RiskCategory();
      this.riskcategory.riskName = "Medium Risk";
      this.risklist.push(this.riskcategory);
    }

    //province

    var province1 = <HTMLInputElement>document.getElementById("natalbox");
    if (province1.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Kwazulu Natal";
      this.province.percentage = + $('#natalvalue').val();
      this.provincelst.push(this.province);
    }
    var province2 = <HTMLInputElement>document.getElementById("northwestbox");
    if (province2.checked == true) {
      this.province = new Province();
      this.province.provinceName = "North West";
      this.province.percentage = + $('#northwestvalue').val();
      this.provincelst.push(this.province);
    }
    var province3 = <HTMLInputElement>document.getElementById("northernbox");
    if (province3.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Northern Cape";
      this.province.percentage = + $('#northernvalue').val();
      this.provincelst.push(this.province);
    }
    var province4 = <HTMLInputElement>document.getElementById("westernbox");
    if (province4.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Western Cape";
      this.province.percentage = + $('#westernvalue').val();
      this.provincelst.push(this.province);
    }
    var province5 = <HTMLInputElement>document.getElementById("limpopobox");
    if (province5.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Limpopo";
      this.province.percentage = + $('#limpopovalue').val();
      this.provincelst.push(this.province);
    }
    var province6 = <HTMLInputElement>document.getElementById("easternbox");
    if (province6.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Eastern Cape";
      this.province.percentage = + $('#easternvalue').val();
      this.provincelst.push(this.province);
    }
    var province7 = <HTMLInputElement>document.getElementById("gautengbox");
    if (province7.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Gauteng";
      this.province.percentage = + $('#gautengvalue').val();
      this.provincelst.push(this.province);
    }
    var province8 = <HTMLInputElement>document.getElementById("mpumalangabox");
    if (province8.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Mpumalanga";
      this.province.percentage = + $('#mpumalangavalue').val();
      this.provincelst.push(this.province);
    }
    var province9 = <HTMLInputElement>document.getElementById("freestatebox");
    if (province9.checked == true) {
      this.province = new Province();
      this.province.provinceName = "Free State";
      this.province.percentage = + $('#freestatevalue').val();
      this.provincelst.push(this.province);
    }

    var province10 = <HTMLInputElement>document.getElementById("selectallprovince");
    if (province10.checked == true || (province1.checked == false && province2.checked == false && province3.checked == false
      && province4.checked == false && province5.checked == false && province6.checked == false && province7.checked == false && province8.checked == false && province9.checked == false)) {

      this.provincelst = [];
      debugger;

      this.province = new Province();
      this.province.provinceName = "Kwazulu Natal";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "North West";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Northern Cape";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Western Cape";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Limpopo";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Eastern Cape";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Gauteng";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Mpumalanga";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Free State";
      this.province.percentage = 10;
      this.provincelst.push(this.province);

      this.province = new Province();
      this.province.provinceName = "Unknown";
      this.province.percentage = 10;
      this.provincelst.push(this.province);
    }

    //income category
  
    var selectallinc = <HTMLInputElement>document.getElementById("selectallincome");
  
    var incB = <HTMLInputElement>document.getElementById("incB");
    if (incB.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "B";
      this.incomelst.push(this.income);
    }
    var incC = <HTMLInputElement>document.getElementById("incC");
    if (incC.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "C";
      this.incomelst.push(this.income);
    }
    var incD = <HTMLInputElement>document.getElementById("incD");
    if (incD.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "D";
      this.incomelst.push(this.income);
    }
    var incE = <HTMLInputElement>document.getElementById("incE");
    if (incE.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "E";
      this.incomelst.push(this.income);
    }
    var incF = <HTMLInputElement>document.getElementById("incF");
    if (incF.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "F";
      this.incomelst.push(this.income);
    }
    var incG = <HTMLInputElement>document.getElementById("incG");
    if (incG.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "G";
      this.incomelst.push(this.income);
    }
    var incH = <HTMLInputElement>document.getElementById("incH");
    if (incH.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "H";
      this.incomelst.push(this.income);
    }
    var incI = <HTMLInputElement>document.getElementById("incI");
    if (incI.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "I";
      this.incomelst.push(this.income);
    }
    var incJ = <HTMLInputElement>document.getElementById("incJ");
    if (incJ.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "J";
      this.incomelst.push(this.income);
    }
    var incK = <HTMLInputElement>document.getElementById("incK");
    if (incK.checked == true || selectallinc.checked == true) {
      this.income = new IncomeCategory();
      this.income.incomeCategoryName = "K";
      this.incomelst.push(this.income);
    }
    this.leadreq.alloylst = this.cscorelst;
    this.leadreq.genderlst = this.genderlst;
    this.leadreq.lsmlst = this.lsmlist;
    this.leadreq.provincelst = this.provincelst;
    this.leadreq.risklst = this.risklist;
    this.leadreq.statuslst = this.mstatuslst;
    this.leadreq.inclst = this.incomelst;
    if (this.id != null && this.id != "undefined") {
      this.leadreq.leadId = this.id;
      this.inpstr = JSON.stringify(this.leadreq);
      localStorage.setItem('leadrequestupdates', this.inpstr);
      this.router.navigate(['leadGeneration/leadresponse', {id:this.id,type:"update"}]);
    } else {
      localStorage.setItem('leadrequest', JSON.stringify(this.leadreq));
      this.router.navigate(['leadGeneration/leadresponse']);
    }


  }
  cancel() {
    this.router.navigate(['leadGeneration']);
  }
  provinceall() {
    var provinceall = <HTMLInputElement>document.getElementById("selectallprovince");
    if (provinceall.checked == true) {
      this.allowprovince1 = false;
      this.allowprovince2 = false;
      this.allowprovince3 = false;
      this.allowprovince4 = false;
      this.allowprovince5 = false;
      this.allowprovince6 = false;
      this.allowprovince7 = false;
      this.allowprovince8 = false;
      this.allowprovince9 = false;
      this.desprovince = false;
      var northwestbox = <HTMLInputElement>document.getElementById("northwestbox");
      northwestbox.checked = true;
      var northernbox = <HTMLInputElement>document.getElementById("northernbox");
      northernbox.checked = true;
      var westernbox = <HTMLInputElement>document.getElementById("westernbox");
      westernbox.checked = true;
      var limpopobox = <HTMLInputElement>document.getElementById("limpopobox");
      limpopobox.checked = true;
      var easternbox = <HTMLInputElement>document.getElementById("easternbox");
      easternbox.checked = true;
      var gautengbox = <HTMLInputElement>document.getElementById("gautengbox");
      gautengbox.checked = true;
      var mpumalangabox = <HTMLInputElement>document.getElementById("mpumalangabox");
      mpumalangabox.checked = true;
      var freestatebox = <HTMLInputElement>document.getElementById("freestatebox");
      freestatebox.checked = true;
      var natalbox = <HTMLInputElement>document.getElementById("natalbox");
      natalbox.checked = true;
    } else {
      this.allowprovince1 = false;
      this.allowprovince2 = false;
      this.allowprovince3 = false;
      this.allowprovince4 = false;
      this.allowprovince5 = false;
      this.allowprovince6 = false;
      this.allowprovince7 = false;
      this.allowprovince8 = false;
      this.allowprovince9 = false;
      this.desprovince = true;
      var northwestbox = <HTMLInputElement>document.getElementById("northwestbox");
      northwestbox.checked = false;
      var northernbox = <HTMLInputElement>document.getElementById("northernbox");
      northernbox.checked = false;
      var westernbox = <HTMLInputElement>document.getElementById("westernbox");
      westernbox.checked = false;
      var limpopobox = <HTMLInputElement>document.getElementById("limpopobox");
      limpopobox.checked = false;
      var easternbox = <HTMLInputElement>document.getElementById("easternbox");
      easternbox.checked = false;
      var gautengbox = <HTMLInputElement>document.getElementById("gautengbox");
      gautengbox.checked = false;
      var mpumalangabox = <HTMLInputElement>document.getElementById("mpumalangabox");
      mpumalangabox.checked = false;
      var freestatebox = <HTMLInputElement>document.getElementById("freestatebox");
      freestatebox.checked = false;
      var natalbox = <HTMLInputElement>document.getElementById("natalbox");
      natalbox.checked = false;
    }
  }
  contactall() {
    var contact = <HTMLInputElement>document.getElementById("selectallcontact");
    if (contact.checked == true) {
      this.allowcontact = false;

      var highlycontact = <HTMLInputElement>document.getElementById("highlycontact");
      highlycontact.checked = true;
      var verygoodcontact = <HTMLInputElement>document.getElementById("verygoodcontact");
      verygoodcontact.checked = true;
      var faircontact = <HTMLInputElement>document.getElementById("faircontact");
      faircontact.checked = true;
      var averagecontact = <HTMLInputElement>document.getElementById("averagecontact");
      averagecontact.checked = true;
      var poorcontact = <HTMLInputElement>document.getElementById("poorcontact");
      poorcontact.checked = true;
      var nocontact = <HTMLInputElement>document.getElementById("nocontact");
      nocontact.checked = true;
      var rightcontact = <HTMLInputElement>document.getElementById("rightcontact");
      rightcontact.checked = true;

    } else {
      this.allowcontact = true;
      var highlycontact = <HTMLInputElement>document.getElementById("highlycontact");
      highlycontact.checked = false;
      var verygoodcontact = <HTMLInputElement>document.getElementById("verygoodcontact");
      verygoodcontact.checked = false;
      var faircontact = <HTMLInputElement>document.getElementById("faircontact");
      faircontact.checked = false;
      var averagecontact = <HTMLInputElement>document.getElementById("averagecontact");
      averagecontact.checked = false;
      var poorcontact = <HTMLInputElement>document.getElementById("poorcontact");
      poorcontact.checked = false;
      var nocontact = <HTMLInputElement>document.getElementById("nocontact");
      nocontact.checked = false;
      var rightcontact = <HTMLInputElement>document.getElementById("rightcontact");
      rightcontact.checked = false;
    }
  }
  lsmall() {
    var lsm = <HTMLInputElement>document.getElementById("selectallLsm");
    if (lsm.checked == true) {
      this.allowlsm = false;
      var lsm0 = <HTMLInputElement>document.getElementById("lsm0");
      lsm0.checked = true;
      var lsm1 = <HTMLInputElement>document.getElementById("lsm1");
      lsm1.checked = true;
      var lsm2 = <HTMLInputElement>document.getElementById("lsm2");
      lsm2.checked = true;
      var lsm3 = <HTMLInputElement>document.getElementById("lsm3");
      lsm3.checked = true;
      var lsm4 = <HTMLInputElement>document.getElementById("lsm4");
      lsm4.checked = true;
      var lsm5 = <HTMLInputElement>document.getElementById("lsm5");
      lsm5.checked = true;
      var lsm6 = <HTMLInputElement>document.getElementById("lsm6");
      lsm6.checked = true;
      var lsm7 = <HTMLInputElement>document.getElementById("lsm7");
      lsm7.checked = true;
      var lsm8 = <HTMLInputElement>document.getElementById("lsm8");
      lsm8.checked = true;
      var lsm9 = <HTMLInputElement>document.getElementById("lsm9");
      lsm9.checked = true;
      var lsm10 = <HTMLInputElement>document.getElementById("lsm10");
      lsm10.checked = true;
    } else {
      this.allowlsm = true;
      var lsm0 = <HTMLInputElement>document.getElementById("lsm0");
      lsm0.checked = false;
      var lsm1 = <HTMLInputElement>document.getElementById("lsm1");
      lsm1.checked = false;
      var lsm2 = <HTMLInputElement>document.getElementById("lsm2");
      lsm2.checked = false;
      var lsm3 = <HTMLInputElement>document.getElementById("lsm3");
      lsm3.checked = false;
      var lsm4 = <HTMLInputElement>document.getElementById("lsm4");
      lsm4.checked = false;
      var lsm5 = <HTMLInputElement>document.getElementById("lsm5");
      lsm5.checked = false;
      var lsm6 = <HTMLInputElement>document.getElementById("lsm6");
      lsm6.checked = false;
      var lsm7 = <HTMLInputElement>document.getElementById("lsm7");
      lsm7.checked = false;
      var lsm8 = <HTMLInputElement>document.getElementById("lsm8");
      lsm8.checked = false;
      var lsm9 = <HTMLInputElement>document.getElementById("lsm9");
      lsm9.checked = false;
      var lsm10 = <HTMLInputElement>document.getElementById("lsm10");
      lsm10.checked = false;

    }
  }
  riskall() {
    var risk = <HTMLInputElement>document.getElementById("selectallrisk");
    if (risk.checked == true) {
      this.allowrisk = false;
      var lowrisk = <HTMLInputElement>document.getElementById("lowrisk");
      lowrisk.checked = true;
      var extremelylowrisk = <HTMLInputElement>document.getElementById("extremelylowrisk");
      extremelylowrisk.checked = true;
      var highrisk = <HTMLInputElement>document.getElementById("highrisk");
      highrisk.checked = true;
      var notcontactrisk = <HTMLInputElement>document.getElementById("notcontactrisk");
      notcontactrisk.checked = true;
      var mediumrisk = <HTMLInputElement>document.getElementById("mediumrisk");
      mediumrisk.checked = true;
    } else {
      this.allowrisk = true;
      var lowrisk = <HTMLInputElement>document.getElementById("lowrisk");
      lowrisk.checked = false;
      var extremelylowrisk = <HTMLInputElement>document.getElementById("extremelylowrisk");
      extremelylowrisk.checked = false;
      var highrisk = <HTMLInputElement>document.getElementById("highrisk");
      highrisk.checked = false;
      var notcontactrisk = <HTMLInputElement>document.getElementById("notcontactrisk");
      notcontactrisk.checked = false;
      var mediumrisk = <HTMLInputElement>document.getElementById("mediumrisk");
      mediumrisk.checked = false;
    }
  }
  incomeall() {
    var inc = <HTMLInputElement>document.getElementById("selectallincome");
    if (inc.checked == true) {
      this.allowincome = false;
      var incB = <HTMLInputElement>document.getElementById("incB");
      incB.checked = true;
      var incC = <HTMLInputElement>document.getElementById("incC");
      incC.checked = true;
      var incD = <HTMLInputElement>document.getElementById("incD");
      incD.checked = true;
      var incE = <HTMLInputElement>document.getElementById("incE");
      incE.checked = true;
      var incF = <HTMLInputElement>document.getElementById("incF");
      incF.checked = true;
      var incG = <HTMLInputElement>document.getElementById("incG");
      incG.checked = true;
      var incH = <HTMLInputElement>document.getElementById("incH");
      incH.checked = true;
      var incI = <HTMLInputElement>document.getElementById("incI");
      incI.checked = true;
      var incJ = <HTMLInputElement>document.getElementById("incJ");
      incJ.checked = true;
      var incK = <HTMLInputElement>document.getElementById("incK");
      incK.checked = true;


    } else {
      this.allowincome = true;
      var incB = <HTMLInputElement>document.getElementById("incB");
      incB.checked = false;
      var incC = <HTMLInputElement>document.getElementById("incC");
      incC.checked = false;
      var incD = <HTMLInputElement>document.getElementById("incD");
      incD.checked = false;
      var incE = <HTMLInputElement>document.getElementById("incE");
      incE.checked = false;
      var incF = <HTMLInputElement>document.getElementById("incF");
      incF.checked = false;
      var incG = <HTMLInputElement>document.getElementById("incG");
      incG.checked = false;
      var incH = <HTMLInputElement>document.getElementById("incH");
      incH.checked = false;
      var incI = <HTMLInputElement>document.getElementById("incI");
      incI.checked = false;
      var incJ = <HTMLInputElement>document.getElementById("incJ");
      incJ.checked = false;
      var incK = <HTMLInputElement>document.getElementById("incK");
      incK.checked = false;
    }
  }
  validategen() {
    this.gen1 = +$('#malevalue').val();
    this.gen2 = +$('#femalevalue').val();
    this.gentotal = this.gen1 + this.gen2;
  }
  validatemarital() {
    this.mar1 = +$('#singlevalue').val();
    this.mar2 = +$('#marriedvalue').val();
    this.maritaltotal = this.mar1 + this.mar2;
  }
  validateprovince() {
    this.p1 = + $('#natalvalue').val();
    this.p2 = + $('#northwestvalue').val();
    this.p3 = +$('#northernvalue').val();
    this.p4 = +$('#westernvalue').val();
    this.p5 = +$('#limpopovalue').val();
    this.p6 = +$('#easternvalue').val();
    this.p7 = +$('#gautengvalue').val();
    this.p8 = +$('#mpumalangavalue').val();
    this.p9 = +$('#freestatevalue').val();

    this.provincetotal = this.p1 + this.p2 + this.p3 + this.p4 + this.p5 + this.p6 + this.p7 + this.p8 + this.p9;

  }
  isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
      return false;
    } else {
      return true;
    }
  }
  enablemale() {

    var genmale = <HTMLInputElement>document.getElementById("malebox");
    var genmaleinp = <HTMLInputElement>document.getElementById("malevalue");
    if (genmale.checked == true)
      genmaleinp.disabled = false;
    else {
      genmaleinp.disabled = true;
      genmaleinp.value = null;
    }
  }
  enablefemale() {

    var genfemale = <HTMLInputElement>document.getElementById("femalebox");
    var genfemaleinp = <HTMLInputElement>document.getElementById("femalevalue");
    if (genfemale.checked == true)
      genfemaleinp.disabled = false;
    else {
      genfemaleinp.disabled = true;
      genfemaleinp.value = null;
    }
  }
  enablesingle() {

    var single = <HTMLInputElement>document.getElementById("singlebox");
    var singlevalue = <HTMLInputElement>document.getElementById("singlevalue");
    if (single.checked == true)
      singlevalue.disabled = false;
    else {
      singlevalue.disabled = true;
      singlevalue.value = null;
    }
  }
  enablemarried() {

    var marriedbox = <HTMLInputElement>document.getElementById("marriedbox");
    var marr = <HTMLInputElement>document.getElementById("marriedvalue");
    if (marriedbox.checked == true)
      marr.disabled = false;
    else {
      marr.disabled = true;
      marr.value = null;
    }

  }
  enablenorthwest() {

    var northwestbox = <HTMLInputElement>document.getElementById("northwestbox");
    var northwestvalue = <HTMLInputElement>document.getElementById("northwestvalue");
    if (northwestbox.checked == true)
      northwestvalue.disabled = false;
    else {
      northwestvalue.disabled = true;
      northwestvalue.value = null;
    }

  }
  enablenortherncape() {

    var northernbox = <HTMLInputElement>document.getElementById("northernbox");
    var northernvalue = <HTMLInputElement>document.getElementById("northernvalue");
    if (northernbox.checked == true)
      northernvalue.disabled = false;
    else {
      northernvalue.disabled = true;
      northernvalue.value = null;
    }
  }
  enablewesterncape() {

    var marriedbox = <HTMLInputElement>document.getElementById("westernbox");
    var westernvalue = <HTMLInputElement>document.getElementById("westernvalue");
    if (marriedbox.checked == true)
      westernvalue.disabled = false;
    else {
      westernvalue.disabled = true;
      westernvalue.value = null;
    }
  }
  enablelimpopo() {

    var limpopobox = <HTMLInputElement>document.getElementById("limpopobox");
    var limpopovalue = <HTMLInputElement>document.getElementById("limpopovalue");
    if (limpopobox.checked == true)
      limpopovalue.disabled = false;
    else {
      limpopovalue.disabled = true;
      limpopovalue.value = null;
    }
  }
  enableeasterncape() {

    var easternbox = <HTMLInputElement>document.getElementById("easternbox");
    var easternvalue = <HTMLInputElement>document.getElementById("easternvalue");
    if (easternbox.checked == true)
      easternvalue.disabled = false;
    else {
      easternvalue.disabled = true;
      easternvalue.value = null;
    }
  }
  enablegauteng() {

    var gautengbox = <HTMLInputElement>document.getElementById("gautengbox");
    var gautengvalue = <HTMLInputElement>document.getElementById("gautengvalue");
    if (gautengbox.checked == true)
      gautengvalue.disabled = false;
    else {
      gautengvalue.disabled = true;
      gautengvalue.value = null;
    }
  }
  enablempumalanga() {

    var mpumalangabox = <HTMLInputElement>document.getElementById("mpumalangabox");
    var mpumalangavalue = <HTMLInputElement>document.getElementById("mpumalangavalue");
    if (mpumalangabox.checked == true)
      mpumalangavalue.disabled = false;
    else {
      mpumalangavalue.disabled = true;
      mpumalangavalue.value = null;
    }
  }
  enablefreestate() {

    var freestatebox = <HTMLInputElement>document.getElementById("freestatebox");
    var freestatevalue = <HTMLInputElement>document.getElementById("freestatevalue");
    if (freestatebox.checked == true)
      freestatevalue.disabled = false;
    else {
      freestatevalue.disabled = true;
      freestatevalue.value = null;
    }
  }
  enablenatal() {

    var natalbox = <HTMLInputElement>document.getElementById("natalbox");
    var natalvalue = <HTMLInputElement>document.getElementById("natalvalue");
    if (natalbox.checked == true)
      natalvalue.disabled = false;
    else {
      natalvalue.disabled = true;
      natalvalue.value = null;
    }
  }

  initialvalidation() {
    var genmaleinp = <HTMLInputElement>document.getElementById("malevalue");
    genmaleinp.disabled = true;

    var genfemaleinp = <HTMLInputElement>document.getElementById("femalevalue");
    genfemaleinp.disabled = true;

    var singlevalue = <HTMLInputElement>document.getElementById("singlevalue");
    singlevalue.disabled = true;

    var marr = <HTMLInputElement>document.getElementById("marriedvalue");
    marr.disabled = true;

    var northwestvalue = <HTMLInputElement>document.getElementById("northwestvalue");
    northwestvalue.disabled = true;

    var northernvalue = <HTMLInputElement>document.getElementById("northernvalue");
    northernvalue.disabled = true;

    var westernvalue = <HTMLInputElement>document.getElementById("westernvalue");
    westernvalue.disabled = true;

    var limpopovalue = <HTMLInputElement>document.getElementById("limpopovalue");
    limpopovalue.disabled = true;

    var easternvalue = <HTMLInputElement>document.getElementById("easternvalue");
    easternvalue.disabled = true;

    var gautengvalue = <HTMLInputElement>document.getElementById("gautengvalue");
    gautengvalue.disabled = true;

    var mpumalangavalue = <HTMLInputElement>document.getElementById("mpumalangavalue");
    mpumalangavalue.disabled = true;

    var freestatevalue = <HTMLInputElement>document.getElementById("freestatevalue");
    freestatevalue.disabled = true;

    var natalvalue = <HTMLInputElement>document.getElementById("marriedvalue");
    natalvalue.disabled = true;
  }

  editform() {



    this.inputstring = JSON.parse(localStorage.getItem('leadinput'));
   
    if (this.inputstring.alloylst.length > 0) {
      this.cscorelst = this.inputstring.alloylst;

      

      var higlycontact = <HTMLInputElement>document.getElementById("highlycontact");
      var verygood = <HTMLInputElement>document.getElementById("verygoodcontact");
      var fair = <HTMLInputElement>document.getElementById("faircontact");
      var average = <HTMLInputElement>document.getElementById("averagecontact");
      var poor = <HTMLInputElement>document.getElementById("poorcontact");
      var nocontact = <HTMLInputElement>document.getElementById("nocontact");
      var rightcontact = <HTMLInputElement>document.getElementById("rightcontact");

      if (this.cscorelst.find(t => t.alloyName == "Highly contactable"))
        higlycontact.checked = true;
      else
        higlycontact.checked = false;

      if (this.cscorelst.find(t => t.alloyName == "Very Good"))
        verygood.checked = true;
      else
        verygood.checked = false;

      if (this.cscorelst.find(t => t.alloyName == "Fair"))
        fair.checked = true;
      else
        fair.checked = false;

      if (this.cscorelst.find(t => t.alloyName == "Average"))
        average.checked = true;
      else
        average.checked = false;

      if (this.cscorelst.find(t => t.alloyName == "Poor"))
        poor.checked = true;
      else
        poor.checked = false;
      
      if (this.cscorelst.find(t => t.alloyName == "No Contact"))
        nocontact.checked = true;
      else
        nocontact.checked = false;

      if (this.cscorelst.find(t => t.alloyName == "Right Party Contact"))
        rightcontact.checked = true;
      else
        rightcontact.checked = false;

      if (higlycontact.checked == true && verygood.checked == true && fair.checked == true && average.checked == true && poor.checked == true && nocontact.checked == true && rightcontact.checked == true) {

        var selectall = <HTMLInputElement>document.getElementById("selectallcontact");
        selectall.checked = true;

      }

    }
    if (this.inputstring.genderlst.length > 0) {

      this.genderlst = this.inputstring.genderlst;
      if (this.genderlst.find(t => t.genderName == "Male")) {
        var malegender = <HTMLInputElement>document.getElementById("malebox");
        malegender.checked = true;
        var malevalue = <HTMLInputElement>document.getElementById("malevalue");
        malevalue.value = (this.genderlst.find(t => t.genderName == "Male").percentage).toString();
        this.gen1 = +malevalue.value;
      }
      if (this.genderlst.find(t => t.genderName == "Female")) {
        var femalegender = <HTMLInputElement>document.getElementById("femalebox");
        femalegender.checked = true;
        var femalevalue = <HTMLInputElement>document.getElementById("femalevalue");
        femalevalue.value = (this.genderlst.find(t => t.genderName == "Female").percentage).toString();
        this.gen2 = +femalevalue.value;
      }
      this.gentotal = this.gen1 + this.gen2;
    }
    if (this.inputstring.statuslst.length > 0) {

      this.mstatuslst = this.inputstring.statuslst;
      if (this.mstatuslst.find(t => t.statusName == "Single")) {
        var single = <HTMLInputElement>document.getElementById("singlebox");
        single.checked = true;
        var malevalue = <HTMLInputElement>document.getElementById("singlevalue");
        malevalue.value = (this.mstatuslst.find(t => t.statusName == "Single").percentage).toString();
        this.mar1 = + malevalue.value;
      }
      if (this.mstatuslst.find(t => t.statusName == "Married")) {
        var marital = <HTMLInputElement>document.getElementById("marriedbox");
        marital.checked = true;
        var maritalvalue = <HTMLInputElement>document.getElementById("marriedvalue");
        maritalvalue.value = (this.mstatuslst.find(t => t.statusName == "Married").percentage).toString();
        this.mar2 = + maritalvalue.value;
      }
      this.maritaltotal = this.mar1 + this.mar2;
    }
    if (this.inputstring.inclst.length > 0) {

      this.incomelst = this.inputstring.inclst;
      var incB = <HTMLInputElement>document.getElementById("incB");
      var incC = <HTMLInputElement>document.getElementById("incC");
      var incD = <HTMLInputElement>document.getElementById("incD");
      var incE = <HTMLInputElement>document.getElementById("incE");
      var incF = <HTMLInputElement>document.getElementById("incF");
      var incG = <HTMLInputElement>document.getElementById("incG");
      var incH = <HTMLInputElement>document.getElementById("incH");
      var incI = <HTMLInputElement>document.getElementById("incI");
      var incJ = <HTMLInputElement>document.getElementById("incJ");
      var incK = <HTMLInputElement>document.getElementById("incK");
      
      if (this.incomelst.find(t => t.incomeCategoryName == "B"))
        incB.checked = true;
      else
        incB.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "C"))
        incC.checked = true;
      else
        incC.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "D"))
        incD.checked = true;
      else
        incD.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "E"))
        incE.checked = true;
      else
        incE.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "F"))
        incF.checked = true;
      else
        incF.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "G"))
        incG.checked = true;
      else
        incG.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "H"))
        incH.checked = true;
      else
        incH.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "I"))
        incI.checked = true;
      else
        incI.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "J"))
        incJ.checked = true;
      else
        incJ.checked = false;
      if (this.incomelst.find(t => t.incomeCategoryName == "K"))
        incK.checked = true;
      else
        incK.checked = false;

      if ( incB.checked == true && incC.checked == true && incD.checked == true && incE.checked == true && incF.checked == true && incG.checked == true
        && incH.checked == true && incI.checked == true && incJ.checked == true && incK.checked == true) {
        var selectallincome = <HTMLInputElement>document.getElementById("selectallincome");
        selectallincome.checked = true;
      }

    }
    if (this.inputstring.lsmlst.length > 0) {

      this.lsmlist = this.inputstring.lsmlst;
      var lsm0 = <HTMLInputElement>document.getElementById("lsm0");
      var lsm1 = <HTMLInputElement>document.getElementById("lsm1");
      var lsm2 = <HTMLInputElement>document.getElementById("lsm2");
      var lsm3 = <HTMLInputElement>document.getElementById("lsm3");
      var lsm4 = <HTMLInputElement>document.getElementById("lsm4");
      var lsm5 = <HTMLInputElement>document.getElementById("lsm5");
      var lsm6 = <HTMLInputElement>document.getElementById("lsm6");
      var lsm7 = <HTMLInputElement>document.getElementById("lsm7");
      var lsm8 = <HTMLInputElement>document.getElementById("lsm8");
      var lsm9 = <HTMLInputElement>document.getElementById("lsm9");
      var lsm10 = <HTMLInputElement>document.getElementById("lsm10");

      if (this.lsmlist.find(t => t.lsmName == "0"))
        lsm0.checked = true;
      else
        lsm0.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "1"))
        lsm1.checked = true;
      else
        lsm1.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "2"))
        lsm2.checked = true;
      else
        lsm2.checked = false;
      
      if (this.lsmlist.find(t => t.lsmName == "3"))
        lsm3.checked = true;
      else
        lsm3.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "4"))
        lsm4.checked = true;
      else
        lsm4.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "5"))
        lsm5.checked = true;
      else
        lsm5.checked = false;
      
      if (this.lsmlist.find(t => t.lsmName == "6"))
        lsm6.checked = true;
      else
        lsm6.checked = false;
      
      if (this.lsmlist.find(t => t.lsmName == "7"))
        lsm7.checked = true;
      else
        lsm7.checked = false;
      
      if (this.lsmlist.find(t => t.lsmName == "8"))
        lsm8.checked = true;
      else
        lsm8.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "9"))
        lsm9.checked = true;
      else
        lsm9.checked = false;

      if (this.lsmlist.find(t => t.lsmName == "10"))
        lsm10.checked = true;
      else
        lsm10.checked = false

      if (lsm0.checked == true && lsm1.checked == true && lsm2.checked == true && lsm3.checked == true && lsm4.checked == true && lsm5.checked == true && lsm6.checked == true
        && lsm7.checked == true && lsm8.checked == true && lsm9.checked == true && lsm10.checked == true) {
        var selectallLsm = <HTMLInputElement>document.getElementById("selectallLsm");
        selectallLsm.checked = true;
      }
    }

    if (this.inputstring.provincelst.length > 0) {

      this.provincelst = this.inputstring.provincelst;

      var province1 = <HTMLInputElement>document.getElementById("natalbox");
      var province1val = <HTMLInputElement>document.getElementById("natalvalue");
      var province2 = <HTMLInputElement>document.getElementById("northwestbox");
      var province2val = <HTMLInputElement>document.getElementById("northwestvalue");
      var province3 = <HTMLInputElement>document.getElementById("northernbox");
      var province3val = <HTMLInputElement>document.getElementById("northernvalue");
      var province4 = <HTMLInputElement>document.getElementById("westernbox");
      var province4val = <HTMLInputElement>document.getElementById("westernvalue");
      var province5 = <HTMLInputElement>document.getElementById("limpopobox");
      var province5val = <HTMLInputElement>document.getElementById("limpopovalue");
      var province6 = <HTMLInputElement>document.getElementById("easternbox");
      var province6val = <HTMLInputElement>document.getElementById("easternvalue");
      var province7 = <HTMLInputElement>document.getElementById("gautengbox");
      var province7val = <HTMLInputElement>document.getElementById("gautengvalue");
      var province8 = <HTMLInputElement>document.getElementById("mpumalangabox");
      var province8val = <HTMLInputElement>document.getElementById("mpumalangavalue");
      var province9 = <HTMLInputElement>document.getElementById("freestatebox");
      var province9val = <HTMLInputElement>document.getElementById("freestatevalue");
   
      var province10 = <HTMLInputElement>document.getElementById("selectallprovince"); 

 
      province1.disabled = false;
      province2.disabled = false;
      province3.disabled = false;
      province4.disabled = false;
      province5.disabled = false;
      province6.disabled = false;
      province7.disabled = false;
      province8.disabled = false;
      province9.disabled = false;
      province10.disabled = false;

      province1val.disabled = true;
      province2val.disabled = true;
      province3val.disabled = true;
      province4val.disabled = true;
      province5val.disabled = true;
      province6val.disabled = true;
      province7val.disabled = true;
      province8val.disabled = true;
      province9val.disabled = true;

      this.allowprovince1 = false;
      this.allowprovince2 = false;
      this.allowprovince3 = false;
      this.allowprovince4 = false;
      this.allowprovince5 = false;
      this.allowprovince6 = false;
      this.allowprovince7 = false;
      this.allowprovince8 = false;
      this.allowprovince9 = false;

        if (this.provincelst.find(t => t.provinceName == "Kwazulu Natal")) {
          province1.checked = true;
          this.allowprovince9 = true;
          province1val.value = (this.provincelst.find(t => t.provinceName == "Kwazulu Natal").percentage).toString();
          this.p1 = +province1val.value;
          province1val.disabled = false;
        } else
          province1.checked = false;

        if (this.provincelst.find(t => t.provinceName == "North West")) {
          province2.checked = true;
          this.allowprovince1 = true;
          province2val.value = (this.provincelst.find(t => t.provinceName == "North West").percentage).toString();
          province2val.disabled = false;
          this.p2 = +province2val.value;
        }
        else
          province2.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Northern Cape")) {
          province3.checked = true;
          this.allowprovince2 = true;
          province3val.value = (this.provincelst.find(t => t.provinceName == "Northern Cape").percentage).toString();
          province3val.disabled = false;
          this.p3 = +province3val.value;
        } else
          province3.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Western Cape")) {
          province4.checked = true;
          this.allowprovince3 = true;
          province4val.value = (this.provincelst.find(t => t.provinceName == "Western Cape").percentage).toString();
          province4val.disabled = false;
          this.p4 = +province4val.value;
        } else
          province4.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Limpopo")) {
          province5.checked = true;
          this.allowprovince4 = true;
          province5val.value = (this.provincelst.find(t => t.provinceName == "Limpopo").percentage).toString();
          province5val.disabled = false;
          this.p5 = +province5val.value;
        } else
          province5.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Eastern Cape")) {
          province6.checked = true;
          this.allowprovince5 = true;
          province6val.value = (this.provincelst.find(t => t.provinceName == "Eastern Cape").percentage).toString();
          province6val.disabled = false;
          this.p6 = +province6val.value;
        } else
          province6.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Gauteng")) {
          province7.checked = true;
          this.allowprovince6 = true;
          province7val.value = (this.provincelst.find(t => t.provinceName == "Gauteng").percentage).toString();
          province7val.disabled = false;
          this.p7 = +province7val.value;
        } else
          province7.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Mpumalanga")) {
          province8.checked = true;
          this.allowprovince7 = true;
          province8val.value = (this.provincelst.find(t => t.provinceName == "Mpumalanga").percentage).toString();
          province8val.disabled = false;
          this.p8 = +province8val.value;
        } else
          province8.checked = false;

        if (this.provincelst.find(t => t.provinceName == "Free State")) {
          province9.checked = true;
          this.allowprovince8 = true;
          province9val.value = (this.provincelst.find(t => t.provinceName == "Free State").percentage).toString();
          province9val.disabled = false;
          this.p9 = + province9val.value;
        } else
          province9.checked = false;
      this.provincetotal = this.p1 + this.p2 + this.p3 + this.p4 + this.p5 + this.p6 + this.p7 + this.p8 + this.p9;
      if (province1.checked == true && province2.checked == true && province3.checked == true && province4.checked == true && province5.checked == true && province6.checked == true
        && province7.checked == true && province8.checked == true && province8.checked == true && province9.checked == true) {
        province10.checked = true;
        province1val.value = null;
        province2val.value = null;
        province3val.value = null;
        province4val.value = null;
        province5val.value = null;
        province6val.value = null;
        province7val.value = null;
        province8val.value = null;
        province9val.value = null;
        this.provincetotal = 0;
        this.provinceall();
      }
    }
    if (this.inputstring.risklst.length > 0) {

      this.risklist = this.inputstring.risklst;

      var lowrisk = <HTMLInputElement>document.getElementById("lowrisk");
      var extremelylowrisk = <HTMLInputElement>document.getElementById("extremelylowrisk");
      var highrisk = <HTMLInputElement>document.getElementById("highrisk");
      var notcontactrisk = <HTMLInputElement>document.getElementById("notcontactrisk");
      var mediumrisk = <HTMLInputElement>document.getElementById("mediumrisk");

      if (this.risklist.find(t => t.riskName == "Low Risk"))
        lowrisk.checked = true;
      else
        lowrisk.checked = false;
      

      if (this.risklist.find(t => t.riskName == "Extremely Low Risk"))
        extremelylowrisk.checked = true;
      else
        extremelylowrisk.checked = false;
      

      if (this.risklist.find(t => t.riskName == "High Risk"))
        highrisk.checked = true;
      else
        highrisk.checked = false;

      if (this.risklist.find(t => t.riskName == "Should Not be Contacted"))
        notcontactrisk.checked = true;
      else
        notcontactrisk.checked = false;

      if (this.risklist.find(t => t.riskName == "Medium Risk"))
        mediumrisk.checked = true;
      else
        mediumrisk.checked = false;

      if (lowrisk.checked == true && extremelylowrisk.checked == true && highrisk.checked == true && notcontactrisk.checked == true && mediumrisk.checked == true) {
        var selectallrisk = <HTMLInputElement>document.getElementById("selectallrisk");
        selectallrisk.checked = true;
      }
    }

    if (this.inputstring.dateRange1 != null)
      this.value = this.inputstring.dateRange1;
    if (this.inputstring.dateRange2 != null)
      this.highValue = this.inputstring.dateRange2;

    if (this.inputstring.isAdversed) {
      var excludeadverse = <HTMLInputElement>document.getElementById("excludeadverse");
      excludeadverse.checked = true;
    }
    if (this.inputstring.isCellNumber) {
      var iscellnumber = <HTMLInputElement>document.getElementById("iscellnumber");
      iscellnumber.checked = true;
    }
    if (this.inputstring.isDirector) {
      var isdirector = <HTMLInputElement>document.getElementById("isdirector");
      isdirector.checked = true;
    }
    if (this.inputstring.isEmail) {
      var isemailonly = <HTMLInputElement>document.getElementById("isemailonly");
      isemailonly.checked = true;
    }
    if (this.inputstring.isHomeOwner) {
      var ishomeowner = <HTMLInputElement>document.getElementById("ishomeowner");
      ishomeowner.checked = true;
    }
    if (this.inputstring.isEmployed) {
      var isemployed = <HTMLInputElement>document.getElementById("isemployed");
      isemployed.checked = true;
    }
    if (this.inputstring.isDeceased) {
      var isDeceased = <HTMLInputElement>document.getElementById("isDeceased");
      isDeceased.checked = true;
    }
    if (this.inputstring.requiredLeads) {
      var leadcount = <HTMLInputElement>document.getElementById("leadcount");
      leadcount.value = (this.inputstring.requiredLeads).toString();
    }

  }

}
