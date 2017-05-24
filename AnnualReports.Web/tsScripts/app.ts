import { Component } from '@angular/core';
import { ReportsService } from './services/reports.service';
@Component({
    selector: 'my-app',
    template: `        
    <div>
        <funds-annual-report></funds-annual-report>
    </div>
  `
})
export class AppComponent {
    title = 'ASP.NET MVC 5 with Angular 2';
    skills = ['MVC 8', 'Angular 2', 'TypeScript', 'Visual Studio 2015'];
    myskills = this.skills[1];
    value: Date;
    constructor(private reportsService: ReportsService) {
        this.init();
    }

    init(): void {
        console.log(this.skills);
    }
}