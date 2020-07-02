import { mergeMap, debounceTime, distinctUntilChanged, tap } from 'rxjs/operators';
import { Component } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ApplicationModel } from './application.model';
import { FormControl } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ApplicationResponseModel } from './application-response.model';

@Component({
  selector: 'app-applications',
  templateUrl: './applications.component.html',
  styleUrls: ['./applications.component.scss']
})
export class ApplicationsComponent {
  applications$: BehaviorSubject<ApplicationModel[]> = new BehaviorSubject<ApplicationModel[]>([]);
  total$: BehaviorSubject<number> = new BehaviorSubject<number>(0);
  filter = new FormControl('');
  searching = false;
  constructor(httpClient: HttpClient) {
    this.filter.valueChanges.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      tap(() => this.searching = true),
      mergeMap(value => httpClient.get<ApplicationResponseModel>(`/api/application?searchText=${value}`))
      , tap(() => this.searching = false))
      .subscribe(result => {
        this.applications$.next(result.list);
        this.total$.next(result.total);
      });
  }
}
