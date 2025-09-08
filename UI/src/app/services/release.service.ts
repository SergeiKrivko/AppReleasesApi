import {inject, Injectable} from '@angular/core';
import {LoadingStatus} from '../entities/loading-status';
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Release} from './api-client';
import {EMPTY, map, Observable, switchMap, tap} from 'rxjs';
import {ApplicationService} from './application.service';
import {ReleaseEntity} from '../entities/release-entity';

interface ReleaseStore {
  releases: ReleaseEntity[];
  selectedRelease: ReleaseEntity | null;
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class ReleaseService {
  private readonly apiClient = inject(ApiClient);
  private readonly applicationService = inject(ApplicationService);

  private readonly releases$$ = signalState<ReleaseStore>({
    releases: [],
    selectedRelease: null,
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly releases$ = toObservable(this.releases$$.releases);
  readonly selectedRelease$ = toObservable(this.releases$$.selectedRelease);

  selectRelease(release: ReleaseEntity | null) {
    if (getState(this.releases$$).selectedRelease?.id !== release?.id)
      patchState(this.releases$$, {selectedRelease: release});
  }

  readonly loadReleasesOnApplicationChange$$ = this.applicationService.selectedApplication$.pipe(
    switchMap(app => {
      if (app)
        return this.loadReleases(app?.id);
      return EMPTY;
    })
  );

  private loadReleases(appId: string): Observable<undefined> {
    patchState(this.releases$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.releasesAll(appId).pipe(
      map(resp => resp.map(releaseToEntity)),
      tap(releases => patchState(this.releases$$, {
        releases: releases,
        loadingStatus: LoadingStatus.Completed
      })),
      switchMap(() => EMPTY)
    );
  }

  releaseById(id: string): Observable<ReleaseEntity | undefined> {
    return this.releases$.pipe(
      map(apps => apps.find(app => app.id == id)),
    );
  }

  getDownloadReleaseAssetsUrl(releaseId: string): Observable<string | undefined> {
    return this.apiClient.downloadGET(releaseId).pipe(
      map(resp => resp.url)
    );
  }
}

const releaseToEntity = (release: Release): ReleaseEntity => ({
  id: release.id ?? "",
  branchId: release.branchId ?? null,
  platform: release.platform ?? null,
  version: release.version ?? "1.0.0",
  releaseNotes: release.releaseNotes,
  createdAt: release.createdAt ?? null,
  deletedAt: release.deletedAt ?? null,
});

