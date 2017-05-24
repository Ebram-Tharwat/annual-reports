import { Component, OnInit } from '@angular/core';
import { ReportsService } from '../services/reports.service';
import { SelectItem } from 'primeng/primeng';

@Component({
    selector: 'funds-annual-report',
    templateUrl: '/Scripts/app/dist/components/funds-annual-report.component.html'
})

export class FundsAnnualReportComponent implements OnInit {
    value: Date = new Date();
    fundNumber: string = null;
    funds: Array<SelectItem> = [];
    constructor(private reportsService: ReportsService) {
    }

    ngOnInit() {
        this.loadFunds();
    }

    loadFunds() {
        this.reportsService.getPrimaryFunds(2016)
            .subscribe(
            funds => {
                this.funds = funds.map(fund => {
                    return { label: `${fund.displayName} - ${fund.fundNumber}`, value: fund.id };
                });
            },
            err => {
                console.log(err);
            });
    }
}