import {ChangeDetectionStrategy, Component, inject, input} from '@angular/core';
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
import {TuiRadio} from '@taiga-ui/kit';
import {BranchService} from '../../services/branch.service';

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
    TuiRadio
  ],
  templateUrl: './branch-card.html',
  styleUrl: './branch-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchCard {
  private readonly branchService = inject(BranchService);
  private readonly dialogs = inject(TuiDialogService);

  branch = input.required<BranchEntity>();

  protected showDialog(content: PolymorpheusContent<TuiDialogContext>): void {
    this.dialogs.open(content).subscribe();
  }

  protected deleteBranch() {
    this.branchService.deleteBranch(this.branch().id).subscribe();
  }
}
