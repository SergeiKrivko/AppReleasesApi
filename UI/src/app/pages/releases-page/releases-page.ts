import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {ApplicationService} from '../../services/application.service';
import {ActivatedRoute} from '@angular/router';
import {NEVER, switchMap, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {Header} from '../../components/header/header';

@Component({
  standalone: true,
  selector: 'app-releases-page',
  imports: [
    Header
  ],
  templateUrl: './releases-page.html',
  styleUrl: './releases-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleasesPage implements OnInit {
  private readonly applicationService = inject(ApplicationService);
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
}
