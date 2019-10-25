import { Component, OnInit, Inject } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Title } from '@angular/platform-browser';
import { Thread } from '../../models/thread';
import { Response } from '../../models/response';
import { Board } from '../../models/board';
import { Observable } from 'rxjs';
import { AppState, selectAuthState } from 'src/app/store/app.states';
import { Store } from '@ngrx/store';

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

  public thread: Thread;
  public responses: Response[];
  getState: Observable<any>;
  isAdmin = false;
  constructor(
    private route: ActivatedRoute,
    private httpClient: HttpClient,
    private title: Title,
    private store: Store<AppState>,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {
    this.getState = this.store.select(selectAuthState);
  }

  ngOnInit() {
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.boardKey = params.get('boardKey');
      this.threadKey = +params.get('threadKey');
      this.initializeComponent();
    });
  }
  initializeComponent() {
    this.httpClient
      .get<Board>(this.baseUrl + 'boards/' + this.boardKey)
      .subscribe(
        x => {
          this.boardDefaultName = x.boardDefaultName;
        },
        error => console.error(error)
      );
    this.httpClient
      .get<Thread>(
        this.baseUrl + 'boards/' + this.boardKey + '/' + this.threadKey
      )
      .subscribe(
        x => {
          this.thread = x;
          this.threadKey = this.thread.threadId;
          this.boardKey = this.thread.boardKey;
          this.threadTitle = this.thread.title;
          this.responses = this.thread.responses;
          this.title.setTitle(this.threadTitle);
          /* TODO: replace this loop by states.isAdmin */
          this.responses.forEach(y => {
            if (y.hostAddress) {
              this.isAdmin = true;
            }
          });
        },
        error => console.error(error)
      );
  }
  toAboneResponse(responseId: number) {
    this.httpClient
      .delete(
        this.baseUrl +
          'boards/' +
          this.boardKey +
          '/' +
          this.threadKey +
          '/' +
          responseId +
          '?remove=false'
      )
      .subscribe(
        x => {
          this.initializeComponent();
        },
        error => console.error(error)
      );
  }
  editResponse(id: number) {
    this.responses[id].isEditMode = true;
    this.responses[id].editedText = JSON.stringify(this.responses[id]);
  }
  excuteEditedResponse(index: number, responseId: number) {
    this.httpClient
      .put(
        `${this.baseUrl}boards/${this.boardKey}/${this.threadKey}/${responseId}`,
        JSON.parse(this.responses[index].editedText)
      )
      .subscribe(
        x => {
          this.initializeComponent();
          this.responses[index].isEditMode = false;

        },
        error => console.error(error)
      );
  }
  cancelEditingResponse(index: number) {
    this.responses[index].isEditMode = false;
    this.responses[index].editedText = '';
  }
}
