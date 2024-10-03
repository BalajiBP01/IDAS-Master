import { Component, Compiler } from '@angular/core';

@Component({
  selector: 'body',
  
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  
    title = 'app';
  constructor(private _compiler: Compiler ) {
    this._compiler.clearCache();
  }
}
