import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { Title } from '@angular/platform-browser';
import { HttpClient } from '@angular/common/http';
import { Board } from '../../models/board';
import { Thread } from '../../models/thread';

@Component({
  selector: 'app-thread-list',
  templateUrl: './thread-list.component.html',
  styleUrls: ['./thread-list.component.css'],
  providers: [Title]
})
export class ThreadListComponent implements OnInit {
  constructor(
    private route: ActivatedRoute,
    private title: Title,
    private httpClient: HttpClient,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {}
  public boardDefaultName: string;
  public boardKey: string;
  private param: any;
  public boardName: string;
  public threads: Thread[];
  public board: Board;
  ngOnInit() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.boardKey = params.get('boardKey');
      this.initializeComponent();
    });
  }
  initializeComponent() {
    this.httpClient
      .get<Board>(this.baseUrl + 'boards/' + this.boardKey + '/')
      .subscribe(
        th => {
          this.board = th;
          this.threads = this.board.child;
          this.boardName = this.board.boardName;
        this.boardKey = this.board.boardKey;
        this.threads = this.board.child;
        this.boardDefaultName = this.board.boardDefaultName;
        this.title.setTitle(this.boardName);
        },
        error => console.error(error)
      );
  }
}
