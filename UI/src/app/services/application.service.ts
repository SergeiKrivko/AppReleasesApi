import {inject, Injectable} from '@angular/core';
import {ApplicationEntity} from '../entities/application-entity';
import {LoadingStatus} from '../entities/loading-status';
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Application, CreateApplicationSchema, UpdateApplicationSchema} from './api-client';
import {EMPTY, first, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {Duration, duration} from 'moment';

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
    if (getState(this.applications$$).selectedApplication?.id !== application?.id)
      patchState(this.applications$$, {selectedApplication: application});
  }

  loadApplications(): Observable<undefined> {
    patchState(this.applications$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.appsAll().pipe(
      map(resp => resp.map(applicationToEntity)),
      tap(apps => patchState(this.applications$$, {
        applications: apps,
        loadingStatus: LoadingStatus.Completed
      })),
      switchMap(() => EMPTY)
    );
  }

  applicationById(id: string): Observable<ApplicationEntity | undefined> {
    return this.applications$.pipe(
      map(apps => apps.find(app => app.id == id)),
    );
  }

  createNewApplication(key: string, name: string, description: string): Observable<ApplicationEntity> {
    return this.apiClient.appsPOST(CreateApplicationSchema.fromJS({key, name, description})).pipe(
      map(app => applicationToEntity(app)),
      tap(app => {
        const applications = getState(this.applications$$).applications;
        patchState(this.applications$$, {
          applications: applications.concat(app),
        })
      })
    )
  }

  updateApplication(
    id: string,
    name: string | undefined,
    description: string | undefined,
    mainBranch: string | undefined,
    defaultLatestReleaseLifetime: Duration | null,
    defaultReleaseLifetime: Duration | null
  ): Observable<undefined> {
    return this.applicationById(id).pipe(
      first(),
      switchMap(old => this.apiClient.appsPUT(
        id, UpdateApplicationSchema.fromJS({
          name: name ?? old?.name,
          description: description ?? old?.description,
          mainBranch: mainBranch ?? old?.mainBranch,
          defaultLatestReleaseLifetime: defaultLatestReleaseLifetime?.asMilliseconds(),
          defaultReleaseLifetime: defaultReleaseLifetime?.asMilliseconds(),
        }))),
      switchMap(() => this.reloadApplicationById(id))
    );
  }

  private reloadApplicationById(id: string): Observable<undefined> {
    return this.apiClient.appsGET(id).pipe(
      map(applicationToEntity),
      tap(app => {
        const state = getState(this.applications$$);
        const applications = state.applications.filter(a => a.id != id);
        if (state.selectedApplication?.id == id)
          patchState(this.applications$$, {
            applications: applications.concat(app),
            selectedApplication: app,
          });
        else
          patchState(this.applications$$, {applications: applications.concat(app)});
      }),
      switchMap(() => NEVER),
    )
  }

  deleteApplication(id: string): Observable<never> {
    return this.apiClient.appsDELETE(id).pipe(
      tap(() => {
        const state = getState(this.applications$$);
        if (state.selectedApplication?.id == id)
          patchState(this.applications$$, {
            applications: state.applications.filter(a => a.id != id),
            selectedApplication: null,
          });
        else
          patchState(this.applications$$, {
            applications: state.applications.filter(a => a.id != id),
          });
      }),
      switchMap(() => NEVER),
    );
  }
}

const applicationToEntity = (application: Application): ApplicationEntity => ({
  id: application.id,
  key: application.key ?? "",
  name: application.name ?? "",
  description: application.description,
  mainBranch: application.mainBranch ?? "main",
  defaultReleaseLifetime: application.defaultReleaseLifetime ? duration(application.defaultReleaseLifetime) : null,
  defaultLatestReleaseLifetime: application.defaultLatestReleaseLifetime ? duration(application.defaultLatestReleaseLifetime) : null,
  createdAt: application.createdAt,
  deletedAt: application.deletedAt ?? null,
});

