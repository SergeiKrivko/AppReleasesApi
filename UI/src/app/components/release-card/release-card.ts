import {ChangeDetectionStrategy, Component, input} from '@angular/core';
import {ReleaseEntity} from '../../entities/release-entity';
import {TuiAppearance, TuiButton} from '@taiga-ui/core';
import {TuiCard} from '@taiga-ui/layout';

@Component({
  standalone: true,
  selector: 'app-release-card',
  imports: [
    TuiButton,
    TuiCard,
    TuiAppearance
  ],
  templateUrl: './release-card.html',
  styleUrl: './release-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleaseCard {
  release = input.required<ReleaseEntity>();
}
