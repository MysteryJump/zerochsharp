/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { PluginsListComponent } from './plugins-list.component';

describe('PluginsListComponent', () => {
  let component: PluginsListComponent;
  let fixture: ComponentFixture<PluginsListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PluginsListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PluginsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
