import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {Header} from '../../components/header/header';
import {ReleaseService} from '../../services/release.service';
import {AsyncPipe} from '@angular/common';
import {ReleaseCard} from '../../components/release-card/release-card';

@Component({
  standalone: true,
  selector: 'app-releases-page',
  imports: [
    Header,
    AsyncPipe,
    ReleaseCard
  ],
  templateUrl: './releases-page.html',
  styleUrl: './releases-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleasesPage {
  private readonly releaseService = inject(ReleaseService);


  protected readonly releases$ = this.releaseService.releases$;
}
