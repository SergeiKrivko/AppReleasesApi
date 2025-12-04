import {inject, Injectable} from '@angular/core';
import {AvailableInstallerBuilderEntity} from '../entities/available-installer-builder-entity';
import {UsingInstallerBuilderEntity} from '../entities/using-installer-builder-entity';
import {LoadingStatus} from '../entities/loading-status';
import {AddInstallerBuilderSchema, ApiClient, InstallerBuilderSchema, InstallerBuilderUsage} from './api-client';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApplicationService} from './application.service';
import {first, NEVER, of, switchMap, tap} from 'rxjs';
import {Duration, duration} from 'moment';

interface InstallersStore {
  availableInstallers: AvailableInstallerBuilderEntity[],
  usingInstallers: UsingInstallerBuilderEntity[],
  loadingStatus: LoadingStatus,
}

@Injectable({
  providedIn: 'root'
})
export class InstallersService {
  private readonly apiClient = inject(ApiClient);
  private readonly applicationService = inject(ApplicationService);

  private readonly store$$ = signalState<InstallersStore>({
    availableInstallers: [],
    usingInstallers: [],
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly availableInstallers$ = toObservable(this.store$$.availableInstallers);
  readonly usingInstallers$ = toObservable(this.store$$.usingInstallers);

  readonly loadInstallersOnApplicationChange$ = this.applicationService.selectedApplication$.pipe(
    tap(() => patchState(this.store$$, {usingInstallers: [], loadingStatus: LoadingStatus.InProgress,})),
    switchMap(application => {
      if (application)
        return this.apiClient.installersAll(application.id);
      return of([]);
    }),
    tap(installers => patchState(this.store$$, {
      usingInstallers: installers.map(installerUsageToEntity),
      loadingStatus: LoadingStatus.Completed,
    })),
    switchMap(() => NEVER),
  );

  loadAvailableInstallers() {
    return this.apiClient.installersAll2().pipe(
      tap(installers => patchState(this.store$$, {
        availableInstallers: installers.map(installerBuilderToEntity),
      })),
      switchMap(() => NEVER),
    );
  }

  createNewInstaller(name: string | null, builderKey: string, lifetime: Duration, platforms: string[]) {
    return this.applicationService.selectedApplication$.pipe(
      first(),
      switchMap(application => {
        if (application)
          return this.apiClient.installers(application.id, AddInstallerBuilderSchema.fromJS({
            name: name,
            key: builderKey,
            installerLifetime: lifetime,
            platforms: platforms,
          }));
        return NEVER;
      })
    );
  }
}

const installerUsageToEntity = (installer: InstallerBuilderUsage): UsingInstallerBuilderEntity => ({
  id: installer.id ?? "",
  name: installer.name ?? null,
  builderKey: installer.builderKey ?? "",
  settings: installer.settings ?? {},
  installerLifetime: duration(installer.installerLifetime) ?? duration(24, 'hours'),
});

const installerBuilderToEntity = (installer: InstallerBuilderSchema): AvailableInstallerBuilderEntity => ({
  key: installer.key ?? "",
  displayName: installer.displayName ?? "",
  description: installer.description ?? "",
});
