import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {ReleaseService} from '../../services/release.service';
import {AsyncPipe} from '@angular/common';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {TuiButton, TuiSurface} from '@taiga-ui/core';
import {TuiCardLarge, TuiHeader} from '@taiga-ui/layout';
import {RouterLink} from '@angular/router';
import {BranchByIdPipe} from '../../pipes/branch-by-id-pipe';
import {TuiChip} from '@taiga-ui/kit';

@Component({
  standalone: true,
  selector: 'app-releases-page',
  imports: [
    AsyncPipe,
    DateFromNowPipe,
    TuiButton,
    TuiCardLarge,
    TuiHeader,
    TuiSurface,
    RouterLink,
    BranchByIdPipe,
    TuiChip
  ],
  templateUrl: './releases-page.html',
  styleUrl: './releases-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleasesPage {
  private readonly releaseService = inject(ReleaseService);


  protected readonly releases$ = this.releaseService.releases$;
}
