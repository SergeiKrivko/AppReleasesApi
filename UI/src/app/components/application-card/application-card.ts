import {ChangeDetectionStrategy, Component, input} from '@angular/core';
import {ApplicationEntity} from '../../entities/application-entity';
import {TuiCard, TuiHeader} from '@taiga-ui/layout';
import {TuiSurface} from '@taiga-ui/core';
import {RouterLink} from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-application-card',
  imports: [
    TuiCard,
    TuiHeader,
    TuiSurface,
    RouterLink
  ],
  templateUrl: './application-card.html',
  styleUrl: './application-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationCard {
  application = input.required<ApplicationEntity>();
}
