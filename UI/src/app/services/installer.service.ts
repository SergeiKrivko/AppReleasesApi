import {inject, Injectable} from '@angular/core';
import {LoadingStatus} from '../entities/loading-status';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Installer} from './api-client';
import {EMPTY, first, map, Observable, switchMap, tap} from 'rxjs';
import {InstallerEntity} from '../entities/installer-entity';
import {ReleaseService} from './release.service';

interface InstallerStore {
  installers: InstallerEntity[];
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class InstallerService {
  private readonly apiClient = inject(ApiClient);
  private readonly releaseService = inject(ReleaseService);

  private readonly installers$$ = signalState<InstallerStore>({
    installers: [],
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly installers$ = toObservable(this.installers$$.installers);

  readonly loadInstallersOnApplicationChange$$ = this.releaseService.selectedRelease$.pipe(
    switchMap(release => {
      if (release)
        return this.loadInstallers(release?.id);
      return EMPTY;
    })
  );

  private loadInstallers(releaseId: string): Observable<undefined> {
    patchState(this.installers$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.installersAll(releaseId).pipe(
      map(resp => resp.map(installerToEntity)),
      tap(installers => patchState(this.installers$$, {
        installers: installers,
        loadingStatus: LoadingStatus.Completed
      })),
      switchMap(() => EMPTY)
    );
  }

  installerById(id: string): Observable<InstallerEntity | undefined> {
    return this.installers$.pipe(
      map(apps => apps.find(app => app.id == id)),
    );
  }

  getDownloadInstallerUrl(installerId: string): Observable<string | undefined> {
    return this.releaseService.selectedRelease$.pipe(
      first(),
      switchMap(release => {
        if (release)
         return this.apiClient.downloadGET2(release.id, installerId)
        return EMPTY;
      }),
      map(resp => resp.url)
    );
  }
}

const installerToEntity = (installer: Installer): InstallerEntity => ({
  id: installer.installerId ?? "",
  releaseId: installer.releaseId ?? "",
  fileName: installer.fileName ?? "",
  createdAt: installer.createdAt ?? null,
  deletedAt: installer.deletedAt ?? null,
});

