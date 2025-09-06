import {Injectable} from '@angular/core';
import {ApplicationEntity} from '../entities/application-entity';
import {LoadingStatus} from '../entities/loading-status';
import {patchState, signalState} from '@ngrx/signals';
import {Observable} from 'rxjs';
import {toObservable} from '@angular/core/rxjs-interop';

interface ApplicationStore {
  applications: ApplicationEntity[];
  selectedApplication: ApplicationEntity | null;
  status: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private readonly applications$$ = signalState<ApplicationStore>({
    applications: [],
    selectedApplication: null,
    status: LoadingStatus.NotStarted,
  });

  readonly applications$ = toObservable(this.applications$$.applications);
  readonly selectedApplication$ = toObservable(this.applications$$.selectedApplication);

  selectApplication(application: ApplicationEntity | null) {
    patchState(this.applications$$, {selectedApplication: application});
  }
}
