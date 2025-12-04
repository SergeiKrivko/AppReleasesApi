import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {
  TuiButtonLoading,
  TuiChevron,
  TuiDataListWrapperComponent, TuiFade,
  TuiInputChipDirective,
  TuiMultiSelectGroupDirective, TuiSelect
} from '@taiga-ui/kit';
import {TuiSelectLike, TuiSurface, TuiTextfield, TuiTitle} from '@taiga-ui/core';
import {InstallersService} from '../../services/installers.service';
import {AsyncPipe} from '@angular/common';
import {TuiCard, TuiCell} from '@taiga-ui/layout';
import {AvailableInstallerBuilderEntity} from '../../entities/available-installer-builder-entity';

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
    TuiSelect
  ],
  templateUrl: './new-installer-builder-dialog.html',
  styleUrl: './new-installer-builder-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewInstallerBuilderDialog {
  private readonly installersService = inject(InstallersService);

  protected readonly availableBuilders$ = this.installersService.availableInstallers$;

  protected readonly control = new FormGroup({
    name: new FormControl<string | null>(null),
    builder: new FormControl<AvailableInstallerBuilderEntity | null>(null),
  });

  protected loading: boolean = false;

  protected submit(){

  }
}
