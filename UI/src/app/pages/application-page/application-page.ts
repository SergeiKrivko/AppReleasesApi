import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {Header} from '../../components/header/header';
import {ActivatedRoute, RouterOutlet} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {NEVER, switchMap, tap} from 'rxjs';
import {ApplicationService} from '../../services/application.service';

@Component({
  standalone: true,
  selector: 'app-application-page',
  imports: [
    Header,
    RouterOutlet
  ],
  templateUrl: './application-page.html',
  styleUrl: './application-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationPage implements OnInit {
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
