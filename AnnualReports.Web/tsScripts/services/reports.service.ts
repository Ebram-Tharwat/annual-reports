import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Rx';

// Import RxJs required methods
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable()
export class ReportsService {
    constructor(private http: Http) { }

    getPrimaryFunds(year) {
        return this.http.get(`/api/funds/primary/${year}`)
            .map(data => data.json());
    };

}