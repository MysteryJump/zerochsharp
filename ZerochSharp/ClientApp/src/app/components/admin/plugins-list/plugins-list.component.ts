import { Component, OnInit, Inject } from '@angular/core';
import { ZerochSharpPlugin } from 'src/app/models/zerochsharpplugin';
import { HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-plugins-list',
  templateUrl: './plugins-list.component.html',
  styleUrls: ['./plugins-list.component.css']
})
export class PluginsListComponent implements OnInit {

  constructor(private http: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { }
  plugins: ZerochSharpPlugin[];
  ngOnInit() {
    this.http.get<ZerochSharpPlugin[]>(this.baseUrl + 'plugin/').subscribe(x => {
      this.plugins = x;
    }, error => console.error(error));
  }

}
