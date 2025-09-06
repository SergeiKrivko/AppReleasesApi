import {Component, input} from '@angular/core';
import {ApplicationEntity} from '../../entities/application-entity';
import {TuiCard, TuiHeader} from '@taiga-ui/layout';
import {TuiSurface} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-application-card',
  imports: [
    TuiCard,
    TuiHeader,
    TuiSurface
  ],
  templateUrl: './application-card.html',
  styleUrl: './application-card.scss'
})
export class ApplicationCard {
  application = input.required<ApplicationEntity>();
}
