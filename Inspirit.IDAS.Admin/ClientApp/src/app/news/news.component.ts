import { Component } from '@angular/core';
import { ToolbarService, LinkService, ImageService, HtmlEditorService, TableService } from '@syncfusion/ej2-angular-richtexteditor';
import { HttpClient } from '@angular/common/http';
import { NgForm } from '@angular/forms';
import { NewsService, News, NewsCrudResponse, UserService, UserPermission } from '../services/services';
import { Router, ActivatedRoute } from '@angular/router';
import { AsideNavService } from '../aside-nav/aside-nav.service';

@Component({
  selector: 'app-container',
  templateUrl: './news.component.html',
  providers: [ToolbarService, LinkService, ImageService, HtmlEditorService, TableService]
})
export class NewsComponent {

  public editor;
  public editorOptions = {
    placeholder: 'Compose an epic...',
    modules: {
      toolbar: [[{ 'font': ['Roboto', 'arial','monospace'] }, 'bold', 'italic', 'underline', 'strike',
        'blockquote', 'code-block', { 'header': 1 }, { 'header': 2 },
      { 'list': 'ordered' }, { 'list': 'bullet' },
      { 'script': 'sub' }, { 'script': 'super' },
      { 'indent': '-1' }, { 'indent': '+1' },
      { 'direction': 'rtl' }, { 'size': ['small', false, 'large', 'huge'] },
      { 'header': [1, 2, 3, 4, 5, 6, false] }, { 'color': [] }, { 'background': [] },
      { 'align': [] }, 'clean', 'link', 'image']]
    }
  };
  content: any;
  sub: any;
  id: any;
  mode: any;
  currentObj: News = new News();
  readonly: boolean = true;
  message: any;
  userid: any;
  userper: UserPermission = new UserPermission();
  start: any = "<startsummary>"
  end: any = "<endsummary>"
  loading: boolean = false;

  constructor(public router: Router, private route: ActivatedRoute, public asideNavService: AsideNavService, public newsService: NewsService, public userService: UserService) {

    this.asideNavService.toggle(true);
    this.userid = localStorage.getItem('userid');
    this.userService.getPermission(this.userid, "News Blog").subscribe(result => {
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

    if (typeof this.id == 'undefined' || typeof this.id == null) {
      this.mode = "add";
      this.currentObj = new News();
      this.readonly = false;
    }
    else {
      this.mode = "view";
      this.loading = true;
      this.newsService.view(this.id).subscribe((result) => {
        this.currentObj = result;
        this.loading = false;
        this.readonly = true;
      });
    }
  }
  ngOnDestroy() {
    this.sub.unsubscribe();
  }
  save() {
    this.readonly = true;
    if (this.mode == "add") {
      if (this.currentObj.blogName == null || this.currentObj.blogName == "undefined") {
        this.message = "Blog name is required";
        this.readonly = false;
        document.getElementById('error').click();
        return;
      }
      else if (this.currentObj.content == null || this.currentObj.content == "undefined") {
        this.message = "Blog Content is required";
        this.readonly = false;
        document.getElementById('error').click();
        return;
      } else {
        this.newsService.insert(this.currentObj).subscribe((result) => {
          this.readonly = false;
          if (result.isSuccess != true) {
            if (result.message != null && result.message != "undefined") {
              this.message = result.message;
              this.readonly = false;
              document.getElementById('errormsg').click();
              return;
            }
            return;
          }
          else
            this.router.navigate(['news']);
        });
      }
    }
    else if (this.mode == "edit") {
      if (this.currentObj.blogName == null || this.currentObj.blogName == "undefined") {
        this.message = "Blog name is required";
        this.readonly = false;
        document.getElementById('error').click();
        return;
      }
      else if (this.currentObj.content == null || this.currentObj.content == "undefined") {
        this.message = "Blog Content is required";
        this.readonly = false;
        document.getElementById('error').click();
        return;
      } else {
        this.newsService.update(this.currentObj).subscribe((result) => {
          if (result.isSuccess != true) {
            if (result.message != null && result.message != "undefined") {
              this.message = result.message;
              this.readonly = false;
              document.getElementById('errormsg').click();
              return;
            }
            return;
          }
          else {
            this.router.navigate(['news']);
          }
        });
      }
    }
  }
  edit() {
    this.mode = "edit";
    this.readonly = true;
    this.newsService.view(this.id).subscribe((result) => {
      this.currentObj = result;
      this.readonly = false;
      this.readonly = false;
    });
  }
  list() {
    this.router.navigate(['news']);
  }
  delete() {
    this.newsService.detele(this.currentObj.id).subscribe((result) => {
      if (result.isSuccess != true) {
        if (result.message != null && result.message != "undefined") {
          this.message = result.message;
          document.getElementById('errormsg').click();
          return;
        }
      }
      else {
        this.router.navigate(['news']);
      }
    });
  }
  dash() {
    this.router.navigate(['dashboard']);
  }
}
