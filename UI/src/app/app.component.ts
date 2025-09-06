import {Component, DestroyRef, inject, OnInit, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {TuiRoot} from '@taiga-ui/core';
import {ApplicationService} from './services/application.service';
import {merge, Observable} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class App implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly title = signal('Avalux Releases');

  ngOnInit() {
    this.mainObservables().pipe(
      takeUntilDestroyed(this.destroyRef),
    ).subscribe()
  }

  private mainObservables(): Observable<undefined> {
    return merge(
      this.applicationService.loadApplications(),
    );
  }
}
