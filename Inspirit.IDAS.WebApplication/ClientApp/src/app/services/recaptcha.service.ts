import { Injectable, InjectionToken, Inject, Optional } from '@angular/core'
import { Observable } from 'rxjs'
import { HttpClient, HttpHeaders } from '@angular/common/http'

export const API_BASE_URL = new InjectionToken<string>('API_BASE_URL')
@Injectable()
export class RecaptchaService {
  private baseUrl: string
  //readonly APIUrl = 'https://www.google.com/recaptcha/api/siteverify?'
  // readonly APIUrl = 'https://localhost:5001/api/Security/getRECAPTCHAResponse?'

  constructor(
    private http: HttpClient,
    @Optional() @Inject(API_BASE_URL) baseUrl?: string,
  ) {
    this.baseUrl = baseUrl ? baseUrl : ''
  }

  getRecaptchaResponse(token: string) {
    // const url = this.baseUrl + '/api/Security/getRECAPTCHAResponse?'
    const url = this.baseUrl + '/api/Security/getRECAPTCHAResponse?'
    var data = `token=${token}`
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Accept: 'application/json',
    })
    return this.http.post(url + data, {}, { headers: reqHeader })
  }
}
