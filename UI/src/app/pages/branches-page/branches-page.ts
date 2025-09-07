import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {ApplicationService} from '../../services/application.service';
import {ActivatedRoute} from '@angular/router';
import {NEVER, switchMap, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {Header} from '../../components/header/header';
import {BranchService} from '../../services/branch.service';
import {AsyncPipe} from '@angular/common';
import {BranchCard} from '../../components/branch-card/branch-card';
import {TuiButton, tuiDialog} from '@taiga-ui/core';
import {NewBranchDialog} from '../../components/new-branch-dialog/new-branch-dialog';

@Component({
  standalone: true,
  selector: 'app-branches-page',
  imports: [
    Header,
    AsyncPipe,
    BranchCard,
    TuiButton
  ],
  templateUrl: './branches-page.html',
  styleUrl: './branches-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BranchesPage implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly branchService = inject(BranchService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit() {
    this.route.params.pipe(
      switchMap(params => {
        const appId = params['id'];
        if (appId)
          return this.applicationService.applicationById(appId);
        return NEVER;
      }),
      tap(app => {
          if (app)
            this.applicationService.selectApplication(app);
        }
      ),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  protected readonly branches$ = this.branchService.branches$;

  private readonly newBranchDialog = tuiDialog(NewBranchDialog, {
    dismissible: false,
    label: 'Новая ветка',
  });

  protected newBranch(): void {
    this.newBranchDialog(undefined).subscribe();
  }
}
