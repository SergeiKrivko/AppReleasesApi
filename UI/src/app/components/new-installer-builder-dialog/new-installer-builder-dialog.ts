import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {
  TuiButtonLoading,
  TuiChevron,
  TuiDataListWrapperComponent, TuiFade,
  TuiInputChipDirective, TuiInputNumber,
  TuiMultiSelectGroupDirective, TuiSelect
} from '@taiga-ui/kit';
import {TuiButton, TuiDialogContext, TuiSelectLike, TuiSurface, TuiTextfield, TuiTitle} from '@taiga-ui/core';
import {InstallersService} from '../../services/installers.service';
import {AsyncPipe} from '@angular/common';
import {TuiCard, TuiCell} from '@taiga-ui/layout';
import {AvailableInstallerBuilderEntity} from '../../entities/available-installer-builder-entity';
import {first, map, tap} from 'rxjs';
import {ReleaseService} from '../../services/release.service';
import {duration} from 'moment';
import {injectContext} from '@taiga-ui/polymorpheus';

@Component({
  standalone: true,
  selector: 'app-new-installer-builder-dialog',
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
    TuiButton
  ],
  templateUrl: './new-installer-builder-dialog.html',
  styleUrl: './new-installer-builder-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewInstallerBuilderDialog {
  private readonly installersService = inject(InstallersService);
  private readonly releaseService = inject(ReleaseService);
  private readonly context = injectContext<TuiDialogContext>();

  protected readonly availableBuilders$ = this.installersService.availableInstallers$;

  protected readonly control = new FormGroup({
    name: new FormControl<string | null>(null),
    builder: new FormControl<AvailableInstallerBuilderEntity | null>(null),
    installerLifetime: new FormControl<number>(24),
    platforms: new FormControl<string[]>([]),
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
    const builderKey = this.control.value.builder?.key;
    if (!builderKey)
      return;
    this.installersService.createNewInstaller(
      this.control.value.name ?? null,
      builderKey,
      duration(this.control.value.installerLifetime, 'hours'),
      this.control.value.platforms ?? [],
    ).pipe(
      tap(() => {
        this.context.completeWith();
        this.loading = false;
      }),
      first()
    ).subscribe();
  }
}
