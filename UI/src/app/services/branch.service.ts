import {inject, Injectable} from '@angular/core';
import {BranchEntity} from '../entities/branch-entity';
import {LoadingStatus} from '../entities/loading-status';
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {ApiClient, Branch, CreateBranchSchema, UpdateBranchSchema} from './api-client';
import {EMPTY, first, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {Duration, duration} from 'moment';
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

  deleteBranch(id: string): Observable<undefined> {
    return this.applicationService.selectedApplication$.pipe(
      first(),
      switchMap(app => {
        if (app) {
          const branches = getState(this.branches$$).branches;
          patchState(this.branches$$, {branches: branches.filter(b => b.id != id)})
          return this.apiClient.branchesDELETE(app.id, id);
        }
        return EMPTY;
      }),
      switchMap(() => EMPTY)
    )
  }

  createNewBranch(name: string): Observable<BranchEntity | undefined> {
    return this.applicationService.selectedApplication$.pipe(
      first(),
      switchMap(app => {
        if (app)
          return this.apiClient.branchesPOST(app.id, CreateBranchSchema.fromJS({name}));
        return EMPTY;
      }),
      map(branchToEntity),
      tap(app => {
        const branches = getState(this.branches$$).branches;
        patchState(this.branches$$, {
          branches: branches.concat(app),
        })
      })
    )
  }

  updateBranch(
    id: string,
    useDefaultReleaseLifetime: boolean,
    latestReleaseLifetime: Duration | null,
    releaseLifetime: Duration | null
  ): Observable<any> {
    return this.branchById(id).pipe(
      first(),
      switchMap(branch => {
        if (branch)
          return this.apiClient.branchesPUT(branch.applicationId, branch.id, UpdateBranchSchema.fromJS({
            useDefaultReleaseLifetime, latestReleaseLifetime, releaseLifetime
          })).pipe(
            switchMap(() => this.loadBranches(branch.applicationId))
          );
        return NEVER;
      })
    );
  }
}

const branchToEntity = (branch: Branch): BranchEntity => ({
  id: branch.id ?? "",
  applicationId: branch.applicationId ?? "",
  name: branch.name ?? "",
  releaseLifetime: branch.releaseLifetime ? duration(branch.releaseLifetime) : null,
  latestReleaseLifetime: branch.latestReleaseLifetime ? duration(branch.latestReleaseLifetime) : null,
  useDefaultReleaseLifetime: branch.useDefaultReleaseLifetime ?? true,
  createdAt: branch.createdAt ?? null,
  deletedAt: branch.deletedAt ?? null,
});

