import {ChangeDetectionStrategy, Component, DestroyRef, inject, input, OnInit} from '@angular/core';
import {TuiCard} from '@taiga-ui/layout';
import {
  TuiAppearance,
  TuiButton,
  TuiDataList,
  TuiDialogContext,
  TuiDialogService,
  TuiDropdown,
  TuiLabel
} from '@taiga-ui/core';
import {BranchEntity} from '../../entities/branch-entity';
import {type PolymorpheusContent} from '@taiga-ui/polymorpheus';
import {TuiButtonLoading, TuiCheckbox, TuiRadio} from '@taiga-ui/kit';
import {BranchService} from '../../services/branch.service';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {Duration} from 'moment';
import {InputLifetime} from '../input-lifetime/input-lifetime';
import {AsyncPipe} from '@angular/common';
import {map, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-branch-card',
  imports: [
    TuiCard,
    TuiAppearance,
    TuiButton,
    TuiDataList,
    TuiDropdown,
    TuiLabel,
    TuiRadio,
    TuiCheckbox,
    ReactiveFormsModule,
    InputLifetime,
    AsyncPipe,
    TuiButtonLoading
  ],
  templateUrl: './branch-card.html',
  styleUrl: './branch-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchCard implements OnInit {
  private readonly branchService = inject(BranchService);
  private readonly dialogs = inject(TuiDialogService);
  private readonly destroyRef = inject(DestroyRef);

  branch = input.required<BranchEntity>();

  ngOnInit() {
    this.control.setValue({
      useDefault: this.branch().useDefaultReleaseLifetime,
      latestReleaseLifetime: this.branch().latestReleaseLifetime,
      releaseLifetime: this.branch().releaseLifetime,
    });
    this.useDefault = this.branch().useDefaultReleaseLifetime;
    this.control.valueChanges.pipe(
      tap(value => {
        this.useDefault = value.useDefault ?? true;
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  protected useDefault = true;

  protected showDialog(content: PolymorpheusContent<TuiDialogContext>): void {
    this.dialogs.open(content, {
      dismissible: true,
      label: this.branch().name,
    }).subscribe();
  }

  protected deleteBranch() {
    this.branchService.deleteBranch(this.branch().id).subscribe();
  }

  protected readonly control = new FormGroup({
    useDefault: new FormControl<boolean>(true),
    latestReleaseLifetime: new FormControl<Duration | null>(null),
    releaseLifetime: new FormControl<Duration | null>(null),
  });

  protected canSave$ = this.control.valueChanges.pipe(
    map(value => value.useDefault != this.branch().useDefaultReleaseLifetime ||
      value.releaseLifetime != this.branch().releaseLifetime ||
      value.latestReleaseLifetime != this.branch().latestReleaseLifetime),
  );

  protected saveChanges() {
    const value = this.control.value;
    this.branchService.updateBranch(this.branch().id,
      value.useDefault ?? true,
      value.latestReleaseLifetime ?? null,
      value.releaseLifetime ?? null
    ).subscribe();
  }
}
