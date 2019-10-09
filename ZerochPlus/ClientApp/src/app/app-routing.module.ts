import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from 'src/app/components/home/home.component';
import { from } from 'rxjs';
import { ThreadListComponent } from 'src/app/components/thread-list/thread-list.component';
import { ResponseListComponent } from 'src/app/components/response-list/response-list.component';
import { LoginComponent } from './components/login/login.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: ':boardKey', component: ThreadListComponent },
  { path: ':boardKey/:threadKey', component: ResponseListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
