import {ChangeDetectionStrategy, Component} from '@angular/core';

@Component({
  standalone: true,
  selector: 'app-single-release-page',
  imports: [],
  templateUrl: './single-release-page.html',
  styleUrl: './single-release-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SingleReleasePage {

}
