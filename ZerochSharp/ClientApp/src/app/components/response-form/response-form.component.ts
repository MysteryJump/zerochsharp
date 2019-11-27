import { Component, OnInit, EventEmitter, Output, Input, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Response } from '../../models/response';

@Component({
  selector: 'app-response-form',
  templateUrl: './response-form.component.html',
  styleUrls: ['./response-form.component.css']
})
export class ResponseFormComponent implements OnInit {
  public model: Response;
  public name: string;
  public mail: string;
  public body: string;
  @Input() public boardDefaultName: string;
  @Input() public boardKey: string;
  @Input() public threadKey: number;
  @Output() ResponseListInitilizer = new EventEmitter();
  constructor(private httpclient: HttpClient, @Inject('BASE_API_URL') private baseUrl: string) { this.name = this.mail = this.body = ''; }

  ngOnInit() {
  }

  writeToThread() {
    this.model = new Response();
    this.model.body = this.body;
    this.model.name = this.name;
    this.model.mail = this.mail;
    this.httpclient.post(this.baseUrl + 'boards/' + this.boardKey + '/' + this.threadKey, this.model)
    .subscribe(x => {
      this.ResponseListInitilizer.emit();
      this.body = this.mail = this.name = '';
    }, error => console.error(error));
  }

}
