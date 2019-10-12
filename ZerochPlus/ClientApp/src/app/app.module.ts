import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';
import { MakeThreadModalComponent } from './components/make-thread-modal/make-thread-modal.component';
import { StoreModule } from '@ngrx/store';
import { BoardListComponent } from './components/board-list/board-list.component';
import { ResponseFormComponent } from './components/response-form/response-form.component';
import { ResponseListComponent } from './components/response-list/response-list.component';
import { ThreadListComponent } from './components/thread-list/thread-list.component';
import { AppRoutingModule } from './app-routing.module';
import { LoginComponent } from './components/login/login.component';
import { EffectsModule } from '@ngrx/effects';
import { AppEffects } from './app.effects';
import { AuthService } from './services/auth.service';
import { AuthEffects } from './store/effects/auth.effects';
import { reducers } from './store/app.states';
import { AuthTokenInterceptor } from './services/auth-token.interceptor';
import { SignupComponent } from './components/signup/signup.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    MakeThreadModalComponent,
    BoardListComponent,
    ResponseFormComponent,
    ResponseListComponent,
    ThreadListComponent,
    LoginComponent,
    SignupComponent
  ],
  imports: [
    BrowserModule,
    StoreModule.forRoot(reducers, {}),
    HttpClientModule,
    FormsModule,
    NgbModule,
    AppRoutingModule,
    EffectsModule.forRoot([AuthEffects])
  ],
  providers: [AuthService,
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthTokenInterceptor,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule {}
