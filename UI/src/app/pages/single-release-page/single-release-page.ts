import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {TuiButton, TuiLabel} from '@taiga-ui/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {ReleaseService} from '../../services/release.service';
import {NEVER, switchMap, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {AsyncPipe} from '@angular/common';
import {TuiLet} from '@taiga-ui/cdk';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {BranchByIdPipe} from '../../pipes/branch-by-id-pipe';
import {TuiAccordion} from '@taiga-ui/kit';

@Component({
  standalone: true,
  selector: 'app-single-release-page',
  imports: [
    TuiButton,
    RouterLink,
    TuiLabel,
    AsyncPipe,
    TuiLet,
    DateFromNowPipe,
    BranchByIdPipe,
    TuiAccordion
  ],
  templateUrl: './single-release-page.html',
  styleUrl: './single-release-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SingleReleasePage implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit() {
    this.route.params.pipe(
      switchMap(params => {
        const releaseId = params['releaseId'];
        if (releaseId)
          return this.releaseService.releaseById(releaseId);
        return NEVER;
      }),
      tap(app => {
          if (app)
            this.releaseService.selectRelease(app);
        }
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  protected selectedRelease$ = this.releaseService.selectedRelease$;
}
