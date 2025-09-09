import {ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {TuiButton, TuiIcon, TuiLabel, TuiTextfieldComponent} from '@taiga-ui/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {ReleaseService} from '../../services/release.service';
import {first, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {AsyncPipe} from '@angular/common';
import {EMPTY_ARRAY, TuiHandler, TuiLet} from '@taiga-ui/cdk';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {BranchByIdPipe} from '../../pipes/branch-by-id-pipe';
import {TuiAccordion, TuiButtonLoading, TuiTextarea, TuiTree} from '@taiga-ui/kit';
import {FormControl, ReactiveFormsModule} from '@angular/forms';

interface TreeNode {
  name: string;
  children: TreeNode[];
}

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
    TuiAccordion,
    TuiTree,
    TuiIcon,
    ReactiveFormsModule,
    TuiTextarea,
    TuiTextfieldComponent,
    TuiButtonLoading
  ],
  templateUrl: './single-release-page.html',
  styleUrl: './single-release-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SingleReleasePage implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

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

  protected selectedRelease$ = this.releaseService.selectedRelease$.pipe(
    tap(release => this.control.setValue(release?.releaseNotes ?? ""))
  );

  protected releaseAssets$: Observable<TreeNode> = this.selectedRelease$.pipe(
    switchMap(release => {
      if (release)
        return this.releaseService.listReleaseAssets(release?.id);
      return [];
    }),
    map(assetsListToTree)
  );

  protected readonly handler: TuiHandler<TreeNode, readonly TreeNode[]> = (item) =>
    item?.children || EMPTY_ARRAY;

  protected downloadReleaseAssets() {
    this.selectedRelease$.pipe(
      first(),
      switchMap(release => {
        if (release)
          return this.releaseService.getDownloadReleaseAssetsUrl(release.id);
        return NEVER;
      }),
      tap(url => {
        if (url)
          window.location.href = url;
      })
    ).subscribe();
  }

  protected editingDescription: boolean = false;
  protected savingChanges: boolean = false;
  protected control = new FormControl<string>("");

  protected editDescription() {
    this.editingDescription = true;
  }

  protected saveChanges() {
    const newDescription = this.control.value;
    this.savingChanges = true;
    this.selectedRelease$.pipe(
      first(),
      switchMap(release => {
        if (release)
          return this.releaseService.updateRelease(release.id, newDescription ?? null).pipe(
            tap(() => {
              this.savingChanges = false;
              this.editingDescription = false;
              this.changeDetectorRef.detectChanges();
            }),
          );
        this.savingChanges = false;
        this.editingDescription = false;
        return NEVER;
      })
    ).subscribe();
  }

  protected cancelChanges() {
    this.editingDescription = false;
  }
}

const assetsListToTree = (assetsList: string[]) => {
  const root: TreeNode = {name: '/', children: []};

  assetsList.forEach(filePath => {
    const parts = filePath.replace('\\', '/').split('/').filter(part => part !== '');
    let currentNode = root;

    parts.forEach(part => {
      let childNode = currentNode.children.find(child => child.name === part);

      if (!childNode) {
        childNode = {name: part, children: []};
        currentNode.children.push(childNode);
      }

      currentNode = childNode;
    });
  });

  return root;
}
