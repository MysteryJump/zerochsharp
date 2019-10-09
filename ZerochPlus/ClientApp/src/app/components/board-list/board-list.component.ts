import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Board } from '../../models/board';

@Component({
  selector: 'app-board-list',
  templateUrl: './board-list.component.html',
  styleUrls: ['./board-list.component.css']
})
export class BoardListComponent implements OnInit {
  public boards: Board[];
  public isCollapsed = false;

  constructor(
    private http: HttpClient,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {}

  ngOnInit(): void {
    this.http.get<Board[]>(this.baseUrl + 'boards/').subscribe(
      b => {
        this.boards = b;
      },
      error => console.error(error)
    );
  }

}
