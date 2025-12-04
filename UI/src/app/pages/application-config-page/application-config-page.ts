import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {ApplicationService} from '../../services/application.service';
import {Router} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {first, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {
  TuiAppearance,
  TuiButton,
  tuiDialog,
  TuiLabel,
  TuiTextfieldComponent,
  TuiTextfieldDirective
} from '@taiga-ui/core';
import {TUI_CONFIRM, TuiCheckbox, TuiConfirmData, TuiInputNumber, TuiTextarea} from '@taiga-ui/kit';
import {ApplicationEntity} from '../../entities/application-entity';
import {AsyncPipe} from '@angular/common';
import {Duration} from 'moment';
import {TuiResponsiveDialogService} from '@taiga-ui/addon-mobile';
import {InputLifetime} from '../../components/input-lifetime/input-lifetime';
import {TuiCard} from '@taiga-ui/layout';
import {TuiLet} from '@taiga-ui/cdk';
import {InstallersService} from '../../services/installers.service';
import {UsingInstallerBuilderEntity} from '../../entities/using-installer-builder-entity';
import {AvailableInstallerBuilderEntity} from '../../entities/available-installer-builder-entity';
import {NewInstallerBuilderDialog} from '../../components/new-installer-builder-dialog/new-installer-builder-dialog';
import {
  UpdateInstallerBuilderDialog
} from '../../components/update-installer-builder-dialog/update-installer-builder-dialog';

@Component({
  standalone: true,
  selector: 'app-application-page',
  imports: [
    FormsModule,
    ReactiveFormsModule,
    TuiTextfieldComponent,
    TuiTextfieldDirective,
    TuiTextarea,
    TuiLabel,
    TuiButton,
    TuiCheckbox,
    TuiInputNumber,
    AsyncPipe,
    InputLifetime,
    TuiCard,
    TuiLet,
    TuiAppearance
  ],
  templateUrl: './application-config-page.html',
  styleUrl: './application-config-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationConfigPage implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly installersService = inject(InstallersService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialogs = inject(TuiResponsiveDialogService);
  private readonly router = inject(Router);

  private applicationId: string | undefined;

  protected control = new FormGroup({
    key: new FormControl<string>(""),
    name: new FormControl<string>(""),
    description: new FormControl<string>(""),
    mainBranch: new FormControl<string>(""),
    latestReleaseLifetime: new FormControl<Duration | null>(null),
    releaseLifetime: new FormControl<Duration | null>(null),
  })

  ngOnInit() {
    this.applicationService.selectedApplication$.pipe(
      tap(app => {
        if (app)
          this.loadApplication(app);
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  private loadApplication(application: ApplicationEntity) {
    this.applicationId = application.id;
    this.control.setValue({
      key: application.key,
      name: application.name,
      description: application.description ?? "",
      mainBranch: application.mainBranch,
      latestReleaseLifetime: application.defaultLatestReleaseLifetime,
      releaseLifetime: application.defaultReleaseLifetime,
    });
    this.control.disable();
  }

  protected isEditing: boolean = false;

  protected startEditing() {
    this.isEditing = true;
    this.control.enable();
  }

  protected saveChanges() {
    this.isEditing = false;
    this.control.disable();
    if (this.applicationId)
      this.applicationService.updateApplication(
        this.applicationId,
        this.control.value.name ?? undefined,
        this.control.value.description ?? undefined,
        this.control.value.mainBranch ?? undefined,
        this.control.value.latestReleaseLifetime ?? null,
        this.control.value.releaseLifetime ?? null,
      ).subscribe();
  }

  protected cancelChanges() {
    this.isEditing = false;
    this.control.disable();
    if (this.applicationId) {
      this.applicationService.applicationById(this.applicationId).pipe(
        tap(app => {
          if (app)
            this.loadApplication(app);
        }),
        first(),
      ).subscribe();
    }
  }

  protected deleteApplication(): void {
    const data: TuiConfirmData = {
            content: 'Вы уверены, что хотите удалить приложение?',
            yes: 'Да',
            no: 'Нет',
        };

        this.dialogs
            .open<boolean>(TUI_CONFIRM, {
                label: 'Удаление приложения',
                size: 's',
                data,
            })
            .pipe(
              switchMap(result => {
                if (result && this.applicationId) {
                  void this.router.navigate([".."]);
                  return this.applicationService.deleteApplication(this.applicationId);
                }
                return NEVER;
              }),
            )
            .subscribe();
  }

  protected readonly usingInstallers$ = this.installersService.usingInstallers$;
  protected readonly availableInstallers$ = this.installersService.availableInstallers$;

  getBuilder(usingBuilder: UsingInstallerBuilderEntity): Observable<AvailableInstallerBuilderEntity | undefined> {
    return this.availableInstallers$.pipe(
      map(available => available.find(e => e.key == usingBuilder.builderKey))
    );
  }

  private readonly newInstallerDialog = tuiDialog(NewInstallerBuilderDialog, {
    dismissible: false,
    label: 'Новый установщик',
  });

  private readonly updateInstallerDialog = tuiDialog(UpdateInstallerBuilderDialog, {
    dismissible: false,
    label: 'Редактирование установщика',
  });

  protected newInstaller(): void {
    this.newInstallerDialog(undefined).subscribe();
  }

  protected updateInstaller(installer: UsingInstallerBuilderEntity): void {
    this.installersService.selectInstaller(installer);
    this.updateInstallerDialog(undefined).subscribe();
  }

  protected removeInstaller(installer: UsingInstallerBuilderEntity): void {
    this.installersService.removeInstaller(installer.id).subscribe();
  }
}
