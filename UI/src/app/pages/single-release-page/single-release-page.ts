import {ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {TuiButton, TuiIcon, TuiLabel, TuiNotification, TuiTextfieldComponent} from '@taiga-ui/core';
import {ActivatedRoute, RouterLink} from '@angular/router';
import {ReleaseService} from '../../services/release.service';
import {catchError, first, map, NEVER, Observable, of, switchMap, tap} from 'rxjs';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {AsyncPipe} from '@angular/common';
import {EMPTY_ARRAY, TuiHandler, TuiLet} from '@taiga-ui/cdk';
import {DateFromNowPipe} from '../../pipes/date-from-now-pipe';
import {BranchByIdPipe} from '../../pipes/branch-by-id-pipe';
import {TuiAccordion, TuiButtonLoading, TuiTextarea, TuiTree} from '@taiga-ui/kit';
import {FormControl, ReactiveFormsModule} from '@angular/forms';
import {BundleService} from '../../services/bundle.service';
import {TuiCard} from '@taiga-ui/layout';
import {MetricService} from '../../services/metric.service';
import {AsIntegerPipe} from '../../pipes/as-integer-pipe';

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
    TuiButtonLoading,
    TuiCard,
    TuiNotification,
    AsIntegerPipe
  ],
  templateUrl: './single-release-page.html',
  styleUrl: './single-release-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SingleReleasePage implements OnInit {
  private readonly releaseService = inject(ReleaseService);
  private readonly installerService = inject(BundleService);
  private readonly metricService = inject(MetricService);
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

  protected releaseAssets$: Observable<TreeNode | null> = this.selectedRelease$.pipe(
    switchMap(release => {
      if (release)
        return this.releaseService.listReleaseAssets(release?.id);
      return [];
    }),
    map(assetsListToTree)
  );

  protected readonly handler: TuiHandler<TreeNode, readonly TreeNode[]> = (item) =>
    item?.children || EMPTY_ARRAY;

  protected isAssetsDownloading: boolean = false;

  protected downloadReleaseAssets() {
    this.isAssetsDownloading = true;
    this.selectedRelease$.pipe(
      first(),
      switchMap(release => {
        if (release)
          return this.releaseService.getDownloadReleaseAssetsUrl(release.id);
        return NEVER;
      }),
      catchError(() => {
        this.isAssetsDownloading = false;
        this.changeDetectorRef.detectChanges();
        return NEVER;
      }),
      tap(url => {
        this.isAssetsDownloading = false;
        this.changeDetectorRef.detectChanges();
        if (url)
          window.location.href = url;
      })
    ).subscribe();
  }

  protected readonly releaseInstallers$ = this.installerService.bundles$;

  protected downloadReleaseInstaller(id: string) {
    this.installerService.getDownloadInstallerUrl(id).pipe(
      tap(url => {
        console.log(url);
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

  protected releaseDownloadCount$ = this.selectedRelease$.pipe(
    switchMap(release => {
      if (release)
        return this.metricService.getReleaseDownloadsCount(release);
      return of(0);
    })
  )
}

const assetsListToTree = (assetsList: string[]) : TreeNode | null => {
  if (assetsList.length == 0)
    return null;
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
