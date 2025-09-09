import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {ReleaseService} from '../../services/release.service';
import {AsyncPipe} from '@angular/common';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {
  TuiButton, TuiDataList,
  TuiLabel,
  TuiSelectLike,
  TuiSurface,
  TuiTextfieldDropdownDirective,
  TuiTextfieldMultiComponent
} from '@taiga-ui/core';
import {TuiCardLarge, TuiHeader} from '@taiga-ui/layout';
import {RouterLink} from '@angular/router';
import {BranchByIdPipe} from '../../pipes/branch-by-id-pipe';
import {TuiChevron, TuiChip, TuiDataListWrapper, TuiInputChip, TuiMultiSelect} from '@taiga-ui/kit';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {ApplicationService} from '../../services/application.service';
import {BranchService} from '../../services/branch.service';
import {combineLatest, map, Observable, startWith} from 'rxjs';
import {BranchEntity} from '../../entities/branch-entity';

@Component({
  standalone: true,
  selector: 'app-releases-page',
  imports: [
    AsyncPipe,
    DateFromNowPipe,
    TuiButton,
    TuiCardLarge,
    TuiHeader,
    TuiSurface,
    RouterLink,
    BranchByIdPipe,
    TuiChip,
    TuiTextfieldMultiComponent,
    TuiChevron,
    TuiLabel,
    TuiInputChip,
    TuiSelectLike,
    TuiDataListWrapper,
    TuiMultiSelect,
    TuiTextfieldDropdownDirective,
    FormsModule,
    TuiDataList,
    ReactiveFormsModule
  ],
  templateUrl: './releases-page.html',
  styleUrl: './releases-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ReleasesPage {
  private readonly releaseService = inject(ReleaseService);
  private readonly applicationService = inject(ApplicationService);
  private readonly branchService = inject(BranchService);

  protected readonly mainBranches$: Observable<BranchEntity[]> = combineLatest(
    this.applicationService.selectedApplication$,
    this.branchService.branches$,
  ).pipe(
    map(([app, branches, ..._]) => {
      if (app)
        return branches.filter(b => b.name == app.mainBranch);
      return [];
    })
  );

  protected readonly otherBranches$: Observable<BranchEntity[]> = combineLatest(
    this.applicationService.selectedApplication$,
    this.branchService.branches$,
  ).pipe(
    map(([app, branches, ..._]) => {
      if (app)
        return branches.filter(b => b.name != app.mainBranch);
      return branches;
    })
  );

  protected readonly control = new FormGroup({
    branches: new FormControl<BranchEntity[]>([]),
  });

  protected readonly releases$ = combineLatest(
    this.releaseService.releases$,
    this.control.valueChanges.pipe(
      startWith({
        branches: [],
      })
    )
  ).pipe(
    map(([releases, filters, ..._]) => {
      const branches = filters.branches;
      console.log(branches);
      if (branches && branches.length > 0)
        releases = releases.filter(r => branches.map(b => b.id).includes(r.branchId));
      return releases;
    })
  );

  protected readonly stringify = ({name}: BranchEntity): string => name;
}
