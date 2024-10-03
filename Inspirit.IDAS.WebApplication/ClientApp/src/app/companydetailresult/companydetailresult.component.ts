import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { CompanyService, CompanyProfile, CompanyRequest, CompanySearchRequest, TracingService } from '../services/services';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ContactDetailsComponent } from '../contactdetails/contactdetails';
import { headernavService } from '../header-nav/header-nav.service';
import { Ng4LoadingSpinnerService } from 'ng4-loading-spinner';

import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import { dashCaseToCamelCase } from '@angular/compiler/src/util';

@Component({
  selector: 'tracingSearch/companydetailresult',
  templateUrl: './companydetailresult.component.html'
})

export class ComapanydtetailResultComponent implements OnInit, OnDestroy {
  _request: CompanyRequest = new CompanyRequest();
  response: CompanyProfile = new CompanyProfile();
  dtOptions: DataTables.Settings = {};

  id: any;
  userid: any;
  customerid: any;
  private sub: any;
  public loading: boolean = false;
  fullName: any;
  comId: any;
  name: any;
  points: number;
  searchRequest: CompanySearchRequest = new CompanySearchRequest();
  isuserExists: any;
  globalSearch: any;
  istrailuser: boolean = false;
  profilPresent: boolean = false;
  judgePresent: boolean = false;
  propertyPresent: boolean = false;
  directorPresent: boolean = false;
  auditorPresene: boolean = false;
  isTimelinepresent: boolean = false;
  contactpresent: boolean = false;
  addresspresent: boolean = false;
  printprofile: boolean = false;
  oldTime: any;
  newTime: any;
  timeTaken: any;
  Client_Logo: string
  // krishna start 
  isXDS: any
  userName: any
  // krishna end


  constructor(public router: Router, public route: ActivatedRoute, public headernavService: headernavService, private spinnerService: Ng4LoadingSpinnerService, public tracingService: TracingService, private modalService: NgbModal, public companyService: CompanyService) {
  }

  ngOnInit() {
    this.Client_Logo = localStorage.getItem('client_logo')

    // krishna start
    this.isXDS = localStorage.getItem('isXDS')
    this.userName = localStorage.getItem('username')
    // krishna end

    if (this.Client_Logo == 'null' || this.Client_Logo == 'undefined' || this.Client_Logo == null) {


      this.Client_Logo = 'data:image/png;base64,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'
      //this.Client_Logo = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAOIAAABaCAYAAACliYrPAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAEkRJREFUeNrsXX1sHMUVn3Oc78BtCBVSAr117JBCVfnSFolWar2pqgqptD4oBSQabt1SAaWVL1WhIJC8kVqFFtScVVRoaeULhJLw5YuERAtIXkv9gz9aWFdqGwiO9wiIUNHmDkjq+OOuM84bM56b3du9W9sX+/2klX13c3Mzb97vfe3ubKxSqRBEeCRueyBRPPLXUsk+VERpIBpFC4qgPsS0T+TpnxRKAoFEXCS03/fHB8jJ95JIRAQScZGwbe/zesv5mzKT77/DXnZfdFWPhlJBIBEXGJVTH7xcaV3dOnX6I/6WgVJBIBEXMiS96w83xdaf387+H//fKf42hqcIJOKChaQ/HdDIipWPzLw4+a74UWrLtT/C8BSBRFwITH14MktWrd4wE55OjosfxdErIpCICxGS7v6N3nrexjR/XZ44IzdBIiKQiAsAS3xRLr0vf9598a57dRQTAok4j96Q/kkHaGqitBBIxAXyhj7IJO74FRZtEEjERfSGDKxok0GpIZCIC+QNW1atKXh5Rf3ORzFXRCARI/SGmpc3XLmlg13wXfLwihZKD4FEjA6m1wfldVrRh3Dprfc9gbkiAok430RkKNmHsvTPsFeIiuJDIBEbD0tZntfp08QQyKoKUTMde59Hr4iYfyJue6qQXMJyMWp8roFXdD1C1GVVQT0XdKHZxxiaiB1PvZWiR7FCYq/Rvw49lqLlr7Vos97yvT8NeIWoffojr+pLmYAKXdCbcIwGPVwYo9uMY6yLiNNnJrP0iNOD/d9Jj8wyJCJpe/hvYpuUR4iaW8pEVOiC2YRjtOiRgDGyv1YzyrI19Decl7SqMG1X+3IM6xn5HPbPO8/9unjxrnuZQRqQ2nTpj4+m3F3t+SUpAecl+R296XTBeUk/F/Q1tEf8YOQvWXoQOErs9TLNrzNtv//nrFF6+/Gf51Qh6voVLQ/d8ucTS7Jwo9AFqwnHmBHGSJpVX0MT8T8vH7Cmpibb6NFDD52+dpegjgXxYKqT91Vh+mSlsqU4Mb0kPSJd+yzowjXNqgt0THlBXzfS13ZDOafelqWHDUdkpI7hvqbVgKtqTs6xWKvWkvKadaRlzXpSWb2BTK89j5CVa0msJbbz2Hcvm11c/c5Hsy2r1/W2rI+TqY2bqUuMky9esHZimpD0oa9vPojSPbfBCMhSDng5/KY7ZiyKR1zqSAz8yxzd9wN25Ux/UO+pDxwRQ0/mJecUbo6cnly1adWKgaUaoiKQiJGi7cBocqbg4EEonxDVbht4fYZk7gPfZyQ2xQal6ZmoY810uXIYpYxAItYGy/FmCAVeMWjxgZ1XnM0Xjv3sJpYTzincvDM+TTavbf1y+oV3b0QxI2S0NvPgaDye5MQAFGlM7szLbz3pMk+YFglEyZil+WJKyAn8kNZzbxRd81JesGFe0QGPSY6cmSY74qvIBStX3E9fHpxHmemCV593uS2WLtD52MuaiFQgphh61ZusQtLLkaP95ARFYp4oxZVY+h67F5C1zdLvRPkAGK8NoOYQqgZ6E/uPOoX0ttyb91zt6o+8OufcYmmiTNa1xhotFKhkpsE4TeJxjSxtw8LsPHzPjpAc2Qh0QdQph/aTkXQhrfjOjiDGpZExyt8FiBdyJKU1IfXKoh6PqAf0ELWQFJTbhokzoffVqqdAG5O2T0Vo6ZVXCFGv6FKvyMa1L2A/A4nH3nQLN3fY7m2fzemPjxpckU5OlcmmNQ0FIaLcucxSoCyJALksG0eafod5fZPKzm1QZlpEulClU0DOAa8vhFj3RsZY67vxiOa/qDmiKEiNCj6nIOGwcJQUhMyDN2gI259+KykosqYgo98tTyrkKQGTAsFHZjziZJlQpxhluMaUdVBBwmHpUBHaAYvfjGGoLwlDrgXmiCFgCt6xBBa+KvSEBcoKbROg6FaDv5+SCi9ebdyAISprk2s7MGq4u9qL7QddNm775HQ5fnoqMiampLGykN3i4aqHclsCaWeqvfR9o8nyR00KB0dgXnkpD14oZ7FTei8ryH2ERHSXTbMQMS5MzPDK/ZiS0UVgZBiSQspGiVgznj/6i55ie98h7oGCoBNy2dTojbqz/Znj5mSFDH40VRmPSGYiCft5XuUTyjHZ5UGR0hIZ9Yjz7ajmtZ+Oy1TMxV2IgYBMbMmgFaUiWCT5djOdvmCeMFVLIWDi4vm4eAQh1pw4f+tDryj7G91zA1Pk/hD9dm994tiMkXj9ukvy61tizxYnp09ELLfdtUgoKhYo9mHJCFqLvPYqYu1RkXCpopmImA1h6fKKcKa+/PCZ44ZHeKT2jPdcPZvzBURf+5NnK2gvdm+5juaID0cos8NUZvVc72hKOXdvFLl2hEQskepqJRJxoYgYoq0dNrT0QbKOUNUkwa66mTUcNE+cUfTnvrH5UIQyqys/gagj65MnN4NRLiIRFx4jYQQfcY6gByTnx7//Y8MJSYI4if4m4eEG5ZCL0JhFDZssMzQLERfT+iXDEpFh7PbP5aRcq2a+uO1QwWwWZQUSl2oYpMWCg0REMCTaf/liEMVkxCqECbm2PVXQmkhZm1Lhl1tYikT0R81QbeyWy6vutAgQouKepwgkYpREZICbgveE6Ddz6dOR7HwXxVUxOi4zErHZkeqwng1EGLfnUxYJfkojqkd9N0REuDol0SR5OhIRReCpgGEJw0LUoKc0oiBid4OXeskhdR5VAYm4mPArWFhBOxnr2e6EaN8d0djrOukNJ+8zSEQkYjPB9vks0XH3Y4GLK655aTZoiLr9meNR5GfdcDF3PQQWL17fvxwrlUjEJsLr111i1wgpra2Zh8PkY0GJoUc0hQG4jzOoN8yRuTfalghWcsPAFf7viurSQPSItcOymTsUtt7RH4iMhfQ2FqL2h1zQeiB63j6qEA7cJOxFQBPuXJHvdjfQG9ZNRGX64rcOXmhFuc4KM12TjLc+aBz77U+cgP2ZxPvexRL1xG4ExoN58154zW4fGoQtMRxBYZjnTSrGwtqZ5/peNosAFlGIN7D3AvFEeSfoezvD3CKFHpGFp9/+JBNirbu+mSK/pn9vb80wsHBzh+qi6qB5aWDA7U89UmjNt29Iw9GlICGba1K82RYRWOZMV3bLtQSQcxepvWVJZB4xiNIGgePxf1AMRxjmcS82FKBdn77LMiuTZ6zCwb05v4JIbGribrJy9WqPYklUisFv+DWJz+ZRAHZtbDbCHdCKEelCVP0sSN/s1jOh8qyKegok5HlZ3HJfQNuB0Xx5cqJ7xakSiZ3+gB4fEjJ+ipTpQSYnCJmaJGV6kIkzhBKRlCfGC9OTE3n6Xv7EC7+bVe4Lv7pLL1fKVvzyK9KxK7/JtuWnko6Rlhj7SwpHr0/UVaihiy8uFrtxVpWf6BAeafDXAeWxcYWjBZAxKcm6rq0rMUecC5ME35eGhyQsR+u96KpbSKVSJpXy9OyH48f+QdZSItZK7uchdHJxKRckTC1GlWZgjihg7DvtYS/i9sX46Y9I67/HxLdGqDfMoaQRSMRahZaey/KKZLxuTLz9hvgSz9chkIhBceyHV7Jiyv4o+opNzG7a1n/0hgTmaQgkYhiM3vU1FqLuabSfynkbWeHnOFn8ndIQSMRzNBm3vsXIcw0Jt1HULNas20BWJj49MfV3+0ujN+p49QoCiVg3Ge+/meWMrEQd6tmGa+ObyPrPf+W/lRPHrnD7ewsoSYQf8PRFkJwxe7tL/6S23vogIyQLWQ3ic+J8w2e+UFqpXfjK0b7rr0LpIZCIURPy7HWms5XPS67dLT+/kcFxzz7kFIFAIi4Ejj+3Dy+YRmCOiEAgEREIRGTAi74RCPSICAQCiYhAIBERCAQSEYFAIiIQiHkjYofeZjS4DfySB5VPkh0oCcS8EJGRkJzdfCmFYvVFjszT8+HZHiqwDohzTCZRekR+zSVe9uUPdrG4O48kH6png9sljCzIpKl3R4jyWlMebtWtZPw5DmyLwCVqnfV5lpEDBtFG/s2RCZN7frkQUQcFacTaW6CkuSWqFHoEUcMAOXtvZJWMVNsrLnewPUjnKxVYdCIKe2cyL+jAnpns9YhP/si/k5f3fYTiBbPkbHtCl8f08l6c0E783XydY0/yPpj3kJ/9wOfHfh9+0wADYXs9JwLa8fHZHvuI8qihKM9dMVcuLzbPopTn8Ndz9tCE91yVMRT24OTzdhTz1mCufA5OSNlyWSn7V+hDEdq5Hn0xHXCgvcENjWru8rpJv6Xca1Qhk7plJ/2e4aMDSoS+1hSePCTvcHyYLwD9cUMSZo5U30TbD9vF83Yuqd6qfIS2SQqCYFYt7dUmSNIetA+YYx/MS3yWIbvTPikuAiw+m2OX1C971Jmp6pe+HxPes+G7baISwPtJUKyitLkwx7Akb9amauNhyBlz0pqVQLkdIeQdkPrfDR4liGxtaZ1HiPSAGx9Z9cihNsyfRxAJIGVM0BdmvDRpDC6QTpdkcpi+l1LIJCvpHX8eSL6G7JgepCQjmITwN1GPfrbUQcI+sAo7mfKQs1sPdsNAvSxoD7Tl+7+Y0ucm+XgLw93Qt6kI69hmTjvgYNuod4asiAXtQxc82B4Yz2EQsunRrzhHtlBpxSO7DFK9/01e+ExcfKasWUGR2Rj6JRllJEs8x9sKfQ2Ckl4jrFlRYaRG4PMdMIegjxyzgISs343COHXpN/i2I7vhN3rgd7IKWSUFpe6B9hyMpHFp3bhzsKTfrEoFgDSD8FKUiRytGILs+Bj6YVwZhQw0gRf7IXIJJMPWECTUgIQjohUGISbB0xSl+Nzh4Ri0KcKkOqV2tnBuzZFdOiijIYdsCsvqlyuo+rA9+uAKlOHWkX6HgMHRpH5d3l6YowOLpUtKoCmMlS0QMSdU+kpibgMy4nLPKUIjr6p1VvB+ReG9rPTdOF8/WDe9nkId/IaqQmlysgpe1gGZ9UIf4rrHYdxJxVxt0LeZEFB4DsWw5Fm9Cog5QSauh0zEdimhXQaMW1oyypogP5eE3Kg6TI6YEgYsw5WUSrQ8Fql+VLVqMyVNlRcKRsAi/o86C2JIgvbBlLDgkYO6irzEUoS8RJGXdBJpEyrIgUqCkTCBxLsVCpgUjErNqjXIPwGpQNHHSLG8fASM0hj9vwBkD1r8ycJ3B8FgsTla0vy5Yu6jbfYFyDUJ1BNU47alKKLKG0oG1ZXWqxNSB7dGLYF75DGYlx9scFRDsJ55kIEbNRF1D+sifibnA6+BRdkjCC/v0YcqbBPDty4IC2z4HSb87hAJsWcfCi+SINUPG/Wavw3txTlmZc8pnLpwPMaWFkhd8MjNNOL9ZCNV1VpThaseZEyCpTfA6LKHnxaD5IjMYNG2bfA9A2RqSDLQwQB7VcRV487VMhySN7QDVKn99Fj13WGiPh0kR38WPJXLAKOThnmkoiZiUSCMLVmvlMIDpITwLifE3H65pONhHbsg4c5IVaxA+43W6INIBYWkh/JWLargvfYIFb0kWNzhEIaMh1o82e/xmEpXDSIWaoSOs3OUCg0aFD/Y7+dBoYaC5ohCf1lIVVQhP09TspK8TcU5USOAAeHFobyHN/SKIIpyTs4NpWTEikK4LeedhhwtCTJwQAaVEDl2KCLmYLJ9QuKdEkK9gkfOYkCVSxfC2qKHImlglXXBEvN+ksIVIxYobNDn3qn6yHj0oanC7BphoQFGRhPm6NYKHRWhVifk4Dk/S83L/4p8yFHk3mxduoFcOZg38yQzlWtYSwdkb8H4LA8ZeBbw6F9+bnPW6Cnm2AU5XVbwFjNeTfK8mkdoL/fXC33u94iMqiIISAWG4Xfzgh53c5kI7UYgUuH6b/D8F4jnwv85aNcP/WV8op/GqqZCsaMEAhgCElpAQlVCXABLPwQKmq8ROsahSmWJyiRULAfhsEOQkPfRL/XhEPV5Ty9rrCna52EcXTDHQSGccgMWU3g4OSIYCC8MwxyGFMUAr0gjBX13w/i4V80K62rB9/dBGx1OKQQJ+7NS/33QvymHbrAGnXCaZB+QtkcR/iZ9vPtsOCxVLL0iCOJROBqGMQ/A32FFKMxlx3W4D9bVkLxnVuJFN1RNg+bY9e1Zw/OdIImoeGI2YL86UZ9s1ol0srbOgk0y6od2Njo24RyYI1WkveSphf2tIHOvt28p1Hf8CkNCiuI2eBVWFOsWaMxB9cZPf+eFiIjIFSIL1nQnPtl3eQI3GF5ca5yBkLUXCklIQiQiYoGRhJyDqHIqxPIChqYIRBMA96xBIJoA/xdgAIquIowpEdrZAAAAAElFTkSuQmCC'
      //this.Client_Logo = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAIwAAABPCAYAAAA5vC0kAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAC9VJREFUeNrsXV9sW1cZP9d21n+ivaGTGGXUN2k7bQhWRxraG7ZfOsFLHaFSlaLG7mCiAyk2FPYy5BjYEAjJjgStGGi2JzSoeIjDHtBeyK3ENO1hqishjfVPctOOgdZCb1v617Ev53O+kxyfXDvXW5zZyfeTXDvXx/d85zu/8/0595xTxggEAoFAIBAIBAKBQCAQCAQCYSWx4/lTIdLC6sLXq4IP/uqtWOW9dyeoC4kwy2LPz/+ia9VKvvLPC8aD+46QlSHCLINqNV27dV13KveYpvki1I1EmKbY9fyrIadWTVZv3ZhvgN8/St1IhGmOWjULb2BdANzCGDsOpMgtEWFcAt3UiRhzanUXNGdfWbiu+f0j1JVEmKVwWNb1ur8vHvz2L3TqTiLMonVJ/jrO3wy37zSm6Zw0SepOIoyMRrfj99uNfwdGgz98mawMEaZuXSCobUift+wZGudvZcnM6JrPR1aGCONiXerhjBbUNC3R4Jp8/tFdL/yZrEwHEZD/2HVqFpQNsYJ98WCw0EVyLk2bNc2wp/5Y3rH/WIb/lRZWxunbAJ9T3aZorlvQK+i3wHVr92pdDRbGufZBlt34L7zygyfeineRviNLCeOrk+j9yZNjnDyLrimwIRnM/8PoKpcKuuQ6Bd3WdbxaddlX8x21MHf/VtJZrYpzG4HuNu2aphsvnY1Yz+w1tcAD4JrOLHzl94Oiot0i6r03X9Od6hwOUb++inV11iX97/J5MOXChHWTS7LcU2ofuB/zciFdDn7vZI5/rge9G/2+yLde/3fst089VOoG4W9eOge63ItuIrUKdelYX8azGwsaY6LfL85aVtNx2u1BlvGbsu57543kfJyiMW3TFqZt0Znzie2sulmHUZSxnn5sLHj8Jd23YfMZbfvDRu3Bh9nj/Zvsz2zwD5zc95DNCF4I4+DHKCeM2bNZkuPvA5+fkyyfinTw5Xcis798xuauaGH0XqnU9E0+LU9UWEdptfHKhThkQ9O579itTbk2Ecy/G5r+8aGS4w/U3dDVSo3HxVrsB3/9IE7d3Km0OmgYmFYzbpbGPJixOMYWJpgx/ncE50xi6EcBYN6K/Pv2YyJtcf6Fk6YwmDwx4poxzdc1ESyeH2J3bqawjP7e/Sr7/JY+TxmT6sOltkSk+KmMr3FeptymyfcUI7j1A/6dlvQKA2io2X281CXXoWAE2y5gyi4qoA7qhTkNxsY86EEodC+vZL8IOl1S4gh8zyse9pwe/mHGcKq1iDNPOJEZiWxId5uX4f9OOZu3RpmvXm7i/XtV9sTWB7xWKdpd5rJmsXPc5oPgFedlYACkeJvsNu9vYhDf0riK8rwesJhTSpv1ZUjnpS65r2W4kchcaZcUQ7KA8qCzhvhrgL+G2eL0fQwtUjv35EFMbcFCTGePWcwfSLT4DXRm1vrGrhK3Trm5msNuctfUJvISWQqYnvdjmxJSe6AtU7xNnZ5+EGQxsf7oCk0ZWJhFZZRsqqBcN5u6pI8IIMuAMuLAtEOFM9joUa/puqZpYQjbNcdpcCkXf3q4tOfF1wqO+0gANxY3fn/x7PmvBVOfKl0O2ffbJoyOpEgobke4owJvUx4JE8JR2slUGeTJeAkR2gFaqDHJRQlrU1ytLGnYzTzjtYJkATzOyy3GKgPjbzTGIf4A7yCt3GKuIDv46nTkC5v7hiuOU/4QxE+0ilH4dwlp5CU7bGXMlSZLN2RJVitWcpxu52aP/OkSJ4imK/52Aeef+7LttHZNgInX/3OHndzX9uRdwWNAm1niPjuD8bWYVlseRm1bGXXjZExtSWY0/d0ny47ma5Vq69ytfZh5mEmPJt1sKu/KorwWCdNZ1Kpht8uz3/wcTOiVWrim2O5TVqST7gLfw52qYLkUnAjjTpjI7p+UXOMER6tnDi0smJbuoGSRbrQCRJh5t+QaJ8wefczW5tPNpsHz7npM5Bn7vRTiga4cwM8SYT5eLDHDzlylqaWwjj5aclqk65r77HAzxD1mPbI8JSLMx4hzB3ZywjgqaYzB479rtWY31cI1tWNhgCwtJ+RwAjImZVXWGuKE3pMuyXFYccnFajU7+Gwu7lb+UvwRm/uflZhAA9KBu5nhxEjiMxdBFHjEASdG5KXYJbXGjMioGCzY3kiPEMYpuF+v5Y2nfzbmGs8c2V1gylS2x7Rfhpj+B6VlkTgOrheZkiwL1BNt41lStyMnBfLXpPa2JIyNijDbmCMwPWQJ7d6XTR8asJqRhtMmvfPwj8589qvfX2JtKjN/LyqWyjp/YGehHQvDSSCeG7nJC/EKzAS3SxbRfi+/aVtfH6EuAZiILLjIYSnxYPfCeOWCzu7dntGuX9F9N64y56bNnLu3mHP/Lqvdvc2cO7fZ3N3bdrVaMWu16tlabS7I32PbDh7X/Vs/yXw+DdKkzIWDwTEPWY+nFWfrHV2dVltHdtts7v5yjwB0dBNpjWlxThH93rm3561LfXQ4OermdTQPYx17osSqc4l2flP517TwR6kLBw1a07ueCAOYee6pgtMGaQLbdwBbChcPDZSoi9chYepB8AuHC6xWjS6X8fi3f5ptfPxLhemvDyaoe1cegV4Sdnr8WQhGB4JHX4zzAAUe+BlS2mf3PfrF8sYnv1K05tNrQgegkQooS1qTLolALqnbIFbQWaQKAoFAIBB6PnPQ1cfbPdiGyCpsNqMsCQHrQqZ6vI1TrI39UArZYLdmsgcHeb5Tg2Q5whjM4yN2WOPa5lbYVVEevHuZV0FLpMoP63t7zToZOEA6InfAQ+VeH97BE2OYfS10kfJCbcgPrnevLD/ucOwp4Ca8oU7dXz3uA0ZYli0eKQFzEpPSaJU3qoNg9UVE/LuFlVkwY8qvafhZ7EFmeK+hZouO8IiKUanuqNiBiPcXnQpY2GuMMVaeLa7blfchGyinKAf36UeZ4W9YbjmA727yX2PzW4DNVjLiaQ/CdZWanVKh6FfWny7LwObXCScUuaF8CGTDZaK2VAa+h92nCZQxDAu8hOXHOt3uLbfHwrYKfcltyvHrqQaXJCk+gwpL4Qi1kLk2kqcfX+LoC4bCgVJTQtmI09gh/dKIb4YyEkrDzzFl9Nt4nxRr3DhmKzKHFQtpSW7Jlu4LCoFzXmxJ/oQivy4RLo7KHcYyMNFn4ZpfUGwUR3arnZNZ/B20oygNngmUsx/1FXNJNoqSbKclgkdQr2LicRtr3Jos3zsqGQDRnqik81Hpnkn8zbB8PzmGGUH25VDBBckyCAsDnQEnMVxrYv7LSvwQxtFxjS2/cr8eYOMznYgah7DFs1hsF7c5gr9TjzQFFyPvGQJShKWDkHJK/ZYygJjUqUI/Jl7PSfKUsWPSrPVyVQtlnJD0amB7Q3g97+Iqy6JfhBUD+dF6pJH4luqGsQ1wlkwCBwZs7C9J7YFyWbRQIamPLPxuBsuZboTR1SBQGvkMG6LjKBpyCSZ11jitLspHcdQw1mTaHTOREWn0WpKQwopZalyFygYlT+Lv1LPwdKUDxchMo1VqVVZ3Cfivu8QMYg3wMFtcON4svhC6syRiLLhS6SWfQ7NNlQN1YWE7QgrxDeZ9J6ap1JsS9+evfpQjJLmmhhgGTBWkY0KJcWWE6VIHNttUBlsUJpFIS8q32L8jyhp4TonRIlMDq3FW+Z2N/jjJGk9ViCh/m9ihljxipV2MsK1EHNEVcnGZ0D74HAS5wJVhnBZC/enNskrJBUxieWGpTNT5KBI6jPKVJP2dbtLZcRfiy4kK6FvHmOesNCjFWTdxtMDbUHcDUkxk4290eRD6JNYWJEbp+FnOEsRGMah0nC3dnppTlCyXL7LWZ8bmUAFCoRnJeln4e5nYppQRpLCObfhZ7rCMMtos6bqaWeSU+MeUy2HQl5HKiP1I4j/JCGOc0WyfUkkqZyr6EwvDwtg++R7FJplnUQnwhfvOiMAVB6g4rSrMpLP5lPboGMuo+gljKLA+10WDFeKvGZqv7dw8zFohSghjhlAn5yiIMGsHFpp5a43tgyYQCAQCgUAgEAgEAoFAIBAIBAKBQOgk/i/AAIMd8sJDGqlfAAAAAElFTkSuQmCC'

    }
    else {
      this.Client_Logo = localStorage.getItem('client_logo')
    }
   
    this.oldTime = new Date();
    this.isuserExists = localStorage.getItem('userid');
    if (this.isuserExists != null && this.isuserExists != "undefined") {
      this.name = localStorage.getItem('name');
      if (this.name != null && this.name != 'undefined') {
        this.headernavService.updateUserName(this.name);
      }
      this.headernavService.toggle(true);

      this.dtOptions = {
        pagingType: 'full_numbers',
        scrollX: true, 
        order: [3, "desc"],
        language: {
          search: "Filter:"
        }
      };
      this.userid = localStorage.getItem('userid');
      this.customerid = localStorage.getItem('customerid');
      let usertype = localStorage.getItem('trailuser');
      if (usertype == "YES") {
        this.istrailuser = true;
      } else
        this.istrailuser = false;

      this.spinnerService.show();
      this.loading = true;
      this.sub = this.route.queryParams.subscribe(params => {
        this.id = params['commercialId'];


        this.searchRequest.companyName = params['companyName'];
        this.searchRequest.Client_Logo = params['Client_Logo']
        this.searchRequest.companyRegNumber = params['companyRegNumber'];
        this.searchRequest.type = params['type'];

        if (params['companyName'] != "undefined" && params['companyName'] != null) {
          this._request.searchCriteria = params['companyName'];
          this._request.inputType = "Company Name";
        }

        
        if (params['companyRegNumber'] != "undefined" && params['companyRegNumber'] != null) {
          if (this._request.searchCriteria != "undefined" && this._request.searchCriteria != null) {
            this._request.searchCriteria = this._request.searchCriteria + params['companyRegNumber'];
            this._request.inputType = this._request.inputType + " " + "Company Reg Number";
          }
          this._request.searchCriteria = params['companyRegNumber'];
          this._request.inputType = "Company Reg Number";
        }
        if (params['globalSearch'] != "undefined" && params['globalSearch'] != null) {
          if (this._request.searchCriteria != "undefined" && this._request.searchCriteria != null) {
            this._request.searchCriteria = this._request.searchCriteria + params['globalSearch'];
            this._request.inputType = this._request.inputType + " " + "Global Search";
          }
          this._request.searchCriteria = params['globalSearch'];
          this._request.inputType = "Global Search";
        }
        if (params['commercialAddress'] != "undefined" && params['commercialAddress'] != null) {
          if (this._request.searchCriteria != "undefined" && this._request.searchCriteria != null) {
            this._request.searchCriteria = this._request.searchCriteria + params['commercialAddress'];
            this._request.inputType = this._request.inputType + " " + "Commercial Address";
          }
          this._request.searchCriteria = params['commercialAddress'];
          this._request.inputType = "Commercial Address";
        }
        if (params['commercialTelephone'] != "undefined" && params['commercialTelephone'] != null) {
          if (this._request.searchCriteria != "undefined" && this._request.searchCriteria != null) {
            this._request.searchCriteria = this._request.searchCriteria + params['commercialTelephone'];
            this._request.inputType = this._request.inputType + " " + "Commercial Telephone";
          }
          this._request.searchCriteria = params['commercialTelephone'];
          this._request.inputType = "Commercial Telephone";
        }


        this._request.searchType = params['type'];
      });
      this._request.id = this.id;
      this._request.customerId = this.customerid;
      this._request.userId = this.userid;
      this._request.istrailuser = this.istrailuser;


      this.companyService.getCompanyDetails(this._request).subscribe((result) => {
        this.loading = false;
        this.newTime = new Date();
        this.timeTaken = ((this.newTime.getTime() - this.oldTime.getTime()) / 1000).toFixed(2);

        this.response = result;
        console.log(this.response);
        if (this.response == null) {
          document.getElementById('nodata').click();
        }
        this.fullName = this.response.commercialName;
        this.comId = this.response.registrationNo;
        if (this.response.tabSelected.includes('CommercialProfile')) {
          this.printprofile = true;
          this.profilPresent = true;
        }
        if (this.response.timelines.length > 0 && this.response.tabSelected.includes('CommercialTimeline'))
          this.isTimelinepresent = true;
        if (this.response.commercialJudgements.length > 0 && this.response.tabSelected.includes('CommercialJudgement'))
          this.judgePresent = true;
        if (this.response.propertyOwners.length > 0 && this.response.tabSelected.includes('CommercialProperty'))
          this.propertyPresent = true;
        if (this.response.directorShips.length > 0 && this.response.tabSelected.includes('CommercialDirector'))
          this.directorPresent = true;
        if (this.response.commercialAuditors.length > 0 && this.response.tabSelected.includes('CommercialAuditor'))
          this.auditorPresene = true;
        if (this.response.contacts.length > 0 && this.response.tabSelected.includes('CommercialTelephone'))
          this.contactpresent = true;
        if (this.response.addresses.length > 0 && this.response.tabSelected.includes('CommercialAddress'))
          this.addresspresent = true;
        // pending points
        //if (this.isXDS == 'NO') {
          this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
            this.points = result;
            this.headernavService.updatePoints(this.points);
          });
        //}
      });
    } else {
      this.router.navigate(['/login']);
    }
  }
  downloadpdf() {
    if (this.response.tabSelected.includes('CommercialProfile')) {
      this.profilPresent = true;
      this.printprofile = true;
    } else {
      this.profilPresent = true;
      this.printprofile = false;
    }
    let popupWinindow
   // document.getElementById('logoprint').style.display = "block";
    document.getElementById('logoprint').style.display = "block";
    document.getElementById('DetailsList').style.display = "block";
    let innerContents = document.getElementById('Commercialdetail').innerHTML;
    popupWinindow = window.open('', '_blank', 'width=600,height=700,scrollbars=no,menubar=no,toolbar=no,location=no,status=no,titlebar=no');
    popupWinindow.document.open();
    popupWinindow.document.write('<html><head><style>@page { size: auto;  margin: 0mm; } @media print { body {-webkit-print-color-adjust: exact;} } </style><link rel="stylesheet" type="text/css" href="../../assets/demo/demo2/base/style.bundle.css" /></head><body onload="window.print()">' + innerContents + '</body></html>');
    popupWinindow.document.close();
    document.getElementById('logoprint').style.display = "none";
    document.getElementById('DetailsList').style.display = "none";
  }
  showModalContact(id: string) {
    this.searchRequest.commercialTelephone = id;
    this.searchRequest.type = "Company";
    this.istrailuser = this.istrailuser;
    this.router.navigate(['tracingSearch/commercialSearchResult'], { queryParams: this.searchRequest, skipLocationChange: true });
  }

  ngOnDestroy() {
    // pending points
    //if (this.isXDS == 'NO') {
      this.tracingService.getPoints(this.userid, this.customerid).subscribe((result) => {
        this.points = result;
        this.headernavService.updatePoints(this.points);
      });
    //}

    this.sub.unsubscribe();
    this._request = null;
  }

  tracing() {
    this.istrailuser = this.istrailuser;
    this.router.navigate(['tracingSearch'], { queryParams: { type: 'Company' }, skipLocationChange: true });
  }

  commerciallist() {
    this.istrailuser = this.istrailuser;
    this.router.navigate(['tracingSearch/commercialSearchResult'], { queryParams: this.searchRequest, skipLocationChange: true });
  }
  contactmodal(id: any) {
    const modalRef = this.modalService.open(ContactDetailsComponent, { size: 'lg' });
    modalRef.componentInstance.consumer = false;
    modalRef.componentInstance.contact = this.response.contacts.find(x => x.id == id);
  }
  GlobalSearch() {
    this.searchRequest = new CompanySearchRequest;
    if ((this.points != null && this.points > 0) || (this.isXDS == 'YES')) {
      if (this.globalSearch != null && this.globalSearch != undefined && this.globalSearch != ' ') {
        this.searchRequest.globalSearch = this.globalSearch;
        this.searchRequest.type = "Company";
        this.istrailuser = this.istrailuser;
        this.router.navigate(['tracingSearch/commercialSearchResult'], { queryParams: this.searchRequest, skipLocationChange: true });
      }
    }
  }
}
