import { Component } from '@angular/core';
import { SearchComponent } from './search.component';

@Component({
  selector: 'dumb-news',
  templateUrl: 'app/app.component.html',
  directives: [SearchComponent]
})
export class AppComponent { }