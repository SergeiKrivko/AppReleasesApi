import {inject, Injectable} from '@angular/core';
import {BranchEntity} from '../entities/branch-entity';
import {LoadingStatus} from '../entities/loading-status';
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Branch} from './api-client';
import {EMPTY, map, Observable, switchMap, tap} from 'rxjs';
import {duration} from 'moment';
import {ApplicationService} from './application.service';

interface BranchStore {
  branches: BranchEntity[];
  selectedBranch: BranchEntity | null;
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class BranchService {
  private readonly apiClient = inject(ApiClient);
  private readonly applicationService = inject(ApplicationService);

  private readonly branches$$ = signalState<BranchStore>({
    branches: [],
    selectedBranch: null,
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly branches$ = toObservable(this.branches$$.branches);
  readonly selectedBranch$ = toObservable(this.branches$$.selectedBranch);

  selectBranch(branch: BranchEntity | null) {
    if (getState(this.branches$$).selectedBranch?.id !== branch?.id)
      patchState(this.branches$$, {selectedBranch: branch});
  }

  readonly loadBranchesOnApplicationChange$$ = this.applicationService.selectedApplication$.pipe(
    switchMap(app => {
      if (app)
        return this.loadBranches(app?.id);
      return EMPTY;
    })
  );

  private loadBranches(appId: string): Observable<undefined> {
    patchState(this.branches$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.branchesAll(appId).pipe(
      map(resp => resp.map(branchToEntity)),
      tap(branches => patchState(this.branches$$, {
        branches: branches,
        loadingStatus: LoadingStatus.Completed
      })),
      switchMap(() => EMPTY)
    );
  }

  branchById(id: string): Observable<BranchEntity | undefined> {
    return this.branches$.pipe(
      map(apps => apps.find(app => app.id == id)),
    );
  }
}

const branchToEntity = (branch: Branch): BranchEntity => ({
  id: branch.id ?? "",
  name: branch.name ?? "",
  duration: branch.duration ? duration(branch.duration) : null,
  createdAt: branch.createdAt ?? null,
  deletedAt: branch.deletedAt ?? null,
});

