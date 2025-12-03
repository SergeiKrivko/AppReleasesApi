import {inject, Injectable} from '@angular/core';
import {LoadingStatus} from '../entities/loading-status';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Bundle} from './api-client';
import {EMPTY, first, map, Observable, switchMap, tap} from 'rxjs';
import {BundleEntity} from '../entities/bundle-entity';
import {ReleaseService} from './release.service';

interface InstallerStore {
  bundles: BundleEntity[];
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class BundleService {
  private readonly apiClient = inject(ApiClient);
  private readonly releaseService = inject(ReleaseService);

  private readonly bundles$$ = signalState<InstallerStore>({
    bundles: [],
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly bundles$ = toObservable(this.bundles$$.bundles);

  readonly loadBundlesOnApplicationChange$$ = this.releaseService.selectedRelease$.pipe(
    switchMap(release => {
      if (release)
        return this.loadBundles(release?.id);
      return EMPTY;
    })
  );

  private loadBundles(releaseId: string): Observable<undefined> {
    patchState(this.bundles$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.bundlesAll(releaseId).pipe(
      map(resp => resp.map(bundleToEntity)),
      tap(bundles => patchState(this.bundles$$, {
        bundles: bundles,
        loadingStatus: LoadingStatus.Completed
      })),
      switchMap(() => EMPTY)
    );
  }

  bundleById(id: string): Observable<BundleEntity | undefined> {
    return this.bundles$.pipe(
      map(apps => apps.find(app => app.id == id)),
    );
  }

  getDownloadInstallerUrl(bundleId: string): Observable<string | undefined> {
    return this.releaseService.selectedRelease$.pipe(
      first(),
      switchMap(release => {
        if (release)
         return this.apiClient.downloadGET2(release.id, bundleId)
        return EMPTY;
      }),
      map(resp => resp.url)
    );
  }
}

const bundleToEntity = (bundle: Bundle): BundleEntity => ({
  id: bundle.bundleId ?? "",
  releaseId: bundle.releaseId ?? "",
  fileName: bundle.fileName ?? "",
  createdAt: bundle.createdAt ?? null,
  deletedAt: bundle.deletedAt ?? null,
});

