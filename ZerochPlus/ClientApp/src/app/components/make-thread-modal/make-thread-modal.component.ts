import {
  Component,
  OnInit,
  Output,
  Input,
  EventEmitter,
  Inject
} from '@angular/core';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core/testing';
import { Response } from '../../models/response';
import { PostingThreadMessage } from './posting-thread-message';

@Component({
  selector: 'app-make-thread-modal',
  templateUrl: './make-thread-modal.component.html',
  styleUrls: ['./make-thread-modal.component.css']
})
export class MakeThreadModalComponent implements OnInit {
  public closeResult: string;
  @Input() public boardKey: string;
  @Input() public boardDefaultName: string;
  @Output() public threadListInitialier = new EventEmitter();
  public name: string;
  public mail: string;
  public body: string;
  public title: string;

  constructor(
    private modalService: NgbModal,
    private httpClient: HttpClient,
    @Inject('BASE_API_URL') private baseUrl: string
  ) {}

  ngOnInit() {}
  open(content: any) {
    this.modalService
      .open(content, { ariaLabelledBy: 'modal-basic-title' })
      .result.then(
        x => {
          this.closeResult = `Closed with: ${x}`;
          if (x === 'Submit') {
            this.sendResponseMessage();
          }
        },
        x => {
          this.closeResult = `Dismissed ${this.getDismissReason(x)}`;
        }
      );
  }
  getDismissReason(reason: any) {
    if (reason === ModalDismissReasons.ESC) {
      return 'by pressing ESC';
    } else if (reason === ModalDismissReasons.BACKDROP_CLICK) {
      return 'by clicking on a backdrop';
    } else {
      return `with: ${reason}`;
    }
  }
  sendResponseMessage() {
    const response = new Response();
    response.name = this.name;
    response.mail = this.mail;
    response.body = this.body;
    const thread = new PostingThreadMessage();
    thread.response = response;
    thread.title = this.title;
    this.httpClient
      .post(this.baseUrl + 'boards/' + this.boardKey, thread)
      .subscribe(
        x => {
          this.name = this.mail = this.body = this.title = '';
          this.threadListInitialier.emit();
        },
        x => {
          console.error(x);
        }
      );
  }
}
