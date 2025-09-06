import {ChangeDetectionStrategy, Component, input} from '@angular/core';
import {TuiCard} from '@taiga-ui/layout';
import {TuiAppearance, TuiButton, TuiDataList, TuiDropdown} from '@taiga-ui/core';
import {BranchEntity} from '../../entities/branch-entity';

@Component({
  standalone: true,
  selector: 'app-branch-card',
  imports: [
    TuiCard,
    TuiAppearance,
    TuiButton,
    TuiDataList,
    TuiDropdown
  ],
  templateUrl: './branch-card.html',
  styleUrl: './branch-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchCard {
  branch = input.required<BranchEntity>();

  protected open = false;
}
