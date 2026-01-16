import {ChangeDetectionStrategy, Component, inject, OnInit} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {
  TuiButtonLoading,
  TuiChevron,
  TuiDataListWrapperComponent, TuiFade,
  TuiInputChipDirective, TuiInputNumber,
  TuiMultiSelectGroupDirective, TuiSelect
} from '@taiga-ui/kit';
import {
  TuiAppearance,
  TuiButton,
  TuiDialogContext,
  TuiSelectLike,
  TuiSurface,
  TuiTextfield,
  TuiTitle
} from '@taiga-ui/core';
import {InstallersService} from '../../services/installers.service';
import {AsyncPipe} from '@angular/common';
import {TuiCard, TuiCell} from '@taiga-ui/layout';
import {AvailableInstallerBuilderEntity} from '../../entities/available-installer-builder-entity';
import {first, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {ReleaseService} from '../../services/release.service';
import {duration} from 'moment';
import {injectContext} from '@taiga-ui/polymorpheus';
import {TuiLet} from '@taiga-ui/cdk';
import {
  ConsoleInstallerSettings
} from '../installer-builder-settings/console-installer-settings/console-installer-settings.component';

@Component({
  standalone: true,
  selector: 'app-update-installer-builder-dialog',
  imports: [
    ReactiveFormsModule,
    TuiButtonLoading,
    TuiTextfield,
    AsyncPipe,
    TuiCard,
    TuiSurface,
    TuiChevron,
    TuiDataListWrapperComponent,
    TuiInputChipDirective,
    TuiMultiSelectGroupDirective,
    TuiSelectLike,
    TuiTitle,
    TuiCell,
    TuiFade,
    TuiSelect,
    TuiInputNumber,
    TuiButton,
    TuiLet,
    TuiAppearance,
    ConsoleInstallerSettings
  ],
  templateUrl: './update-installer-builder-dialog.html',
  styleUrl: './update-installer-builder-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UpdateInstallerBuilderDialog implements OnInit {
  private readonly installersService = inject(InstallersService);
  private readonly releaseService = inject(ReleaseService);
  private readonly context = injectContext<TuiDialogContext>();

  protected readonly installer$ = this.installersService.selectedInstaller$;

  ngOnInit() {
    this.installer$.pipe(
      first(),
      tap(installer => {
        this.control.setValue({
          name: installer?.name ?? null,
          installerLifetime: installer?.installerLifetime.asHours() || 24,
          platforms: installer?.platforms ?? [],
          settings: installer?.settings,
        })
      })
    ).subscribe();
  }

  protected getInstallerBuilder(key: string | undefined): Observable<AvailableInstallerBuilderEntity | undefined> {
    return this.installersService.availableInstallers$.pipe(
      map(builders => builders.find(e => e.key == key)),
    );
  }

  protected readonly control = new FormGroup({
    name: new FormControl<string | null>(null),
    installerLifetime: new FormControl<number>(24),
    platforms: new FormControl<string[]>([]),
    settings: new FormControl<any>({}),
  });

  protected readonly platforms$ = this.releaseService.releases$.pipe(
    map(releases => {
      const platforms: string[] = [];
      for (const release of releases) {
        if (release.platform && !platforms.includes(release.platform))
          platforms.push(release.platform)
      }
      return platforms;
    })
  );

  protected loading: boolean = false;

  protected submit() {
    this.loading = true;
    this.installer$.pipe(
      switchMap(installer => {
        if (installer)
          return this.installersService.updateInstaller(
            installer?.id,
            this.control.value.name ?? null,
            duration(this.control.value.installerLifetime, 'hours'),
            this.control.value.platforms ?? [],
            this.control.value.settings,
          );
        return NEVER;
      })
    ).pipe(
      tap(() => {
        this.context.completeWith();
        this.loading = false;
      }),
      first()
    ).subscribe();
  }
}
