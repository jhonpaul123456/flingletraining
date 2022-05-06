import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import {FormsModule} from '@angular/forms';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './components/home/home.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { RegisterComponent } from './components/register/register.component';
import { MemberListComponent } from './components/member-list/member-list.component';
import { MemberDetailComponent } from './components/member-detail/member-detail.component';
import { ListComponent } from './components/list/list.component';
import { MessagesComponent } from './components/messages/messages.component';
import { ToastrModule } from 'ngx-toastr';
import { SharedModule } from './_modules/shared.module';
import { TestErrorsComponent } from './_errors/test-errors/test-errors.component';
import { ErrorInterceptor } from './_interceptor/error.interceptor';
import { NotFoundComponent } from './_errors/not-found/not-found.component';
import { ServerErrorComponent } from './_errors/server-error/server-error.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtInterceptor } from './_interceptor/jwt.interceptor';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './_interceptor/loading.interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MemberDetailComponent,
    ListComponent,
    MessagesComponent,
    TestErrorsComponent,
    NotFoundComponent,
    ServerErrorComponent,
    MemberCardComponent,
    MemberEditComponent,
    PhotoEditorComponent
 
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    SharedModule,
    NgxSpinnerModule
  ], 
  providers: [
    {provide: HTTP_INTERCEPTORS,useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS,useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS,useClass: LoadingInterceptor, multi: true}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
