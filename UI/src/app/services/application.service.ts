import {inject, Injectable} from '@angular/core';
import {ApplicationEntity} from '../entities/application-entity';
import {LoadingStatus} from '../entities/loading-status';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Application} from './api-client';
import {EMPTY, map, Observable, switchMap, tap} from 'rxjs';

interface ApplicationStore {
  applications: ApplicationEntity[];
  selectedApplication: ApplicationEntity | null;
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private readonly apiClient = inject(ApiClient);

  private readonly applications$$ = signalState<ApplicationStore>({
    applications: [],
    selectedApplication: null,
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly applications$ = toObservable(this.applications$$.applications);
  readonly selectedApplication$ = toObservable(this.applications$$.selectedApplication);

  selectApplication(application: ApplicationEntity | null) {
    patchState(this.applications$$, {selectedApplication: application});
  }

  loadApplications(): Observable<undefined> {
    patchState(this.applications$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.appsAll().pipe(
      map(resp => resp.map(applicationToEntity)),
      tap(apps => patchState(this.applications$$, {
        applications: apps,
        loadingStatus: LoadingStatus.InProgress
      })),
      switchMap(() => EMPTY)
    );
  }
}

const applicationToEntity = (application: Application): ApplicationEntity => ({
  id: application.id,
  key: application.key ?? "",
  name: application.name ?? "",
  description: application.description,
  createdAt: application.createdAt,
  deletedAt: application.deletedAt ?? null,
});

