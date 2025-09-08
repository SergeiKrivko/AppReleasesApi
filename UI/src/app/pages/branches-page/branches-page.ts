import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {BranchService} from '../../services/branch.service';
import {AsyncPipe} from '@angular/common';
import {BranchCard} from '../../components/branch-card/branch-card';
import {TuiButton, tuiDialog} from '@taiga-ui/core';
import {NewBranchDialog} from '../../components/new-branch-dialog/new-branch-dialog';

@Component({
  standalone: true,
  selector: 'app-branches-page',
  imports: [
    AsyncPipe,
    BranchCard,
    TuiButton
  ],
  templateUrl: './branches-page.html',
  styleUrl: './branches-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchesPage {
  private readonly branchService = inject(BranchService);

  protected readonly branches$ = this.branchService.branches$;

  private readonly newBranchDialog = tuiDialog(NewBranchDialog, {
    dismissible: false,
    label: 'Новая ветка',
  });

  protected newBranch(): void {
    this.newBranchDialog(undefined).subscribe();
  }
}
