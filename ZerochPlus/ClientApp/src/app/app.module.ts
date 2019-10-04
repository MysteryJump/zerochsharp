import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { CounterComponent } from './counter/counter.component';
import { FetchDataComponent } from './fetch-data/fetch-data.component';
import { MakeThreadModalComponent } from './make-thread-modal/make-thread-modal.component';
import { StoreModule } from '@ngrx/store';
import { reducers, metaReducers } from './reducers';
import { BoardListComponent } from './board-list/board-list.component';
import { ResponseFormComponent } from './response-form/response-form.component';
import { ResponseListComponent } from './response-list/response-list.component';
import { ThreadListComponent } from './thread-list/thread-list.component';
import { AppRoutingModule } from './app-routing.module';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    CounterComponent,
    FetchDataComponent,
    MakeThreadModalComponent,
    BoardListComponent,
    ResponseFormComponent,
    ResponseListComponent,
    ThreadListComponent
  ],
  imports: [
    BrowserModule,
    StoreModule.forRoot(reducers),
    HttpClientModule,
    FormsModule,
    NgbModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule {}
