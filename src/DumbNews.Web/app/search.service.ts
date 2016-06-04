import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable()
export class SearchService {

    constructor(private http: Http) { }

    searchNews(query: string) {
        let headers = new Headers({ 'api-key': 'DDD2358D1B1FB7D753F2ECAD26BD9308' });
        let options = new RequestOptions({ headers: headers });

        return this.http.get('https://dumbnews.search.windows.net/indexes/feeds/docs?api-version=2015-02-28&search=' + query, options)
            .map(this.extractData)
            
    }

    private extractData(res: Response) {
        if (res.status < 200 || res.status >= 300) {
            throw new Error('Response status: ' + res.status);
        }

        let body = res.json();
        return body || {};
    }

    private handleError(error: any) {
        // We should use a remote logging infrastructure
        let errMsg = error.message || 'Server error';
        console.error(errMsg); // log to console instead
        return Observable.throw(errMsg);
    }
}