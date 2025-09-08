import {ChangeDetectionStrategy, Component, inject, input} from '@angular/core';
import {ReleaseEntity} from '../../entities/release-entity';
import {TuiAppearance, TuiButton, TuiSurface} from '@taiga-ui/core';
import {TuiCard, TuiHeader} from '@taiga-ui/layout';
import {ReleaseService} from '../../services/release.service';
import {tap} from 'rxjs';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {RouterLink} from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-release-card',
  imports: [
    TuiButton,
    TuiCard,
    TuiAppearance,
    DateFromNowPipe,
    TuiSurface,
    RouterLink,
    TuiHeader
  ],
  templateUrl: './release-card.html',
  styleUrl: './release-card.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleaseCard {
  private readonly releaseService = inject(ReleaseService);

  release = input.required<ReleaseEntity>();

  protected downloadRelease() {
    this.releaseService.getDownloadReleaseAssetsUrl(this.release().id).pipe(
      tap(url => {
        if (url)
          window.location.href = url;
          // window.open(url);
      })
    ).subscribe();
  }
}
