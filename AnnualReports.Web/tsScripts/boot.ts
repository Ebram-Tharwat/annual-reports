///<reference path="./../typings/globals/core-js/index.d.ts"/>
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { AppComponent } from './app';
import { FundsAnnualReportComponent } from './components/funds-annual-report.component';
import {ReportsService} from "./services/reports.service";
import { DataTableModule, SharedModule, CalendarModule, DropdownModule } from 'primeng/primeng';

@NgModule({
    imports: [BrowserModule, BrowserAnimationsModule, FormsModule, HttpModule, DataTableModule, SharedModule, CalendarModule, DropdownModule],
    declarations: [AppComponent, FundsAnnualReportComponent],
    bootstrap: [AppComponent],
    providers: [ReportsService]
})
export class AppModule { }