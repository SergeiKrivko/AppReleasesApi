import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import {Router, RouterOutlet} from '@angular/router';
import {TuiRoot} from '@taiga-ui/core';
import {ApplicationService} from './services/application.service';
import {merge, NEVER, Observable, switchMap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {BranchService} from './services/branch.service';
import {ReleaseService} from './services/release.service';
import {AuthService} from './services/auth.service';
import {TokensService} from './services/tokens.service';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class App implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly tokensService = inject(TokensService);
  private readonly branchService = inject(BranchService);
  private readonly releaseService = inject(ReleaseService);
  private readonly authService = inject(AuthService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly router = inject(Router);

  ngOnInit() {
    this.mainObservables().pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe()
  }

  private mainObservables(): Observable<undefined> {
    return merge(
      this.branchService.loadBranchesOnApplicationChange$$,
      this.releaseService.loadReleasesOnApplicationChange$$,
      this.authService.isAuthorized$.pipe(
        switchMap(isAuthorized => {
          if (isAuthorized)
            return merge(
              this.applicationService.loadApplications(),
              this.tokensService.loadTokens(),
            );
          void this.router.navigateByUrl('auth');
          return NEVER;
        }),
      )
    );
  }
}
