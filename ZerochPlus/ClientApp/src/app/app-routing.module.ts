import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from 'src/app/home/home.component';
import { from } from 'rxjs';
import { ThreadListComponent } from 'src/app/thread-list/thread-list.component';
import { ResponseListComponent } from 'src/app/response-list/response-list.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: ':boardKey', component: ThreadListComponent },
  { path: ':boardKey/:threadKey', component: ResponseListComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
