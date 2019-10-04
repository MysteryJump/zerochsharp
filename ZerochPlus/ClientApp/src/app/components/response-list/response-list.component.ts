import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
import { Thread } from '../../models/thread';
import { Response } from '../../models/response';
import { Board } from '../../models/board';

@Component({
  selector: 'app-response-list',
  templateUrl: './response-list.component.html',
  styleUrls: ['./response-list.component.css']
})
export class ResponseListComponent implements OnInit {
  public threadKey: number;
  public boardKey: string;
  public threadTitle: string;
  public boardDefaultName: string;

  private param: any;
  public thread: Thread;
  public responses: Response[];
  constructor(
    private route: ActivatedRoute,
    private httpClient: HttpClient,
    private title: Title,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {}

  ngOnInit() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.boardKey = params.get('boardKey');
      this.threadKey = +params.get('threadKey');
      this.initializeComponent();
    });
  }
  initializeComponent() {
    this.httpClient.get(this.baseUrl + 'boards/' + this.boardKey)
    .subscribe(x => {
      this.param = x;
      this.boardDefaultName = (this.param as Board).boardDefaultName;    }, error => console.error(error));
      this.httpClient.get(this.baseUrl + 'boards/' + this.boardKey + '/' + this.threadKey).subscribe(x => {
        this.param = x;
        this.thread = this.param as Thread;
        this.threadKey = this.thread.threadId;
        this.boardKey = this.thread.boardKey;
        this.threadTitle = this.thread.title;
        this.responses = this.thread.responses;
        this.title.setTitle(this.threadTitle);
      }, error => console.error(error));
  }
}
