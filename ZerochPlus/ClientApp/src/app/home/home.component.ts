import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  providers: [Title]
})
export class HomeComponent implements OnInit {
  ngOnInit(): void {
    this.title.setTitle('Home');
  }
  constructor(private title: Title) {
  }
}
