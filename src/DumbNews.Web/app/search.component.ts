import { Component, OnInit } from '@angular/core';
import { SearchService } from './search.service';

@Component({
    selector: 'search',
    templateUrl: 'app/search.component.html',
    providers: [SearchService]
})
export class SearchComponent implements OnInit {
    query: string;
    searchResult: Array<any>;
    
    constructor(private searchService: SearchService) {

    }

    ngOnInit() { }

    submit() {
        console.log(this.query);
        this.searchService.searchNews(this.query)
        .subscribe(result=> {
            this.searchResult = result.value;
        },
        error => {
           alert(error.message); 
        });
    }
}