import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {ApplicationService} from '../../services/application.service';
import {Router} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {first, map, NEVER, switchMap, tap} from 'rxjs';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TuiButton, TuiLabel, TuiTextfieldComponent, TuiTextfieldDirective} from '@taiga-ui/core';
import {TUI_CONFIRM, TuiCheckbox, TuiConfirmData, TuiInputNumber, TuiTextarea} from '@taiga-ui/kit';
import {ApplicationEntity} from '../../entities/application-entity';
import {AsyncPipe} from '@angular/common';
import {duration} from 'moment';
import {TuiResponsiveDialogService} from '@taiga-ui/addon-mobile';

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
    AsyncPipe
  ],
  templateUrl: './application-config-page.html',
  styleUrl: './application-config-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationConfigPage implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialogs = inject(TuiResponsiveDialogService);
  private readonly router = inject(Router);

  private applicationId: string | undefined;

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
      unlimitedDuration: application.defaultDuration === null,
      durationDays: application.defaultDuration?.asDays() ?? 0,
    });
    this.control.disable();
  }

  protected control = new FormGroup({
    key: new FormControl<string>(""),
    name: new FormControl<string>(""),
    description: new FormControl<string>(""),
    mainBranch: new FormControl<string>(""),
    unlimitedDuration: new FormControl<boolean>(false),
    durationDays: new FormControl<number>(0),
  })

  protected durationIsLimited$ = this.control.valueChanges.pipe(
    map(value => !(value.unlimitedDuration ?? false))
  );

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
        this.control.value.unlimitedDuration ? null : duration(this.control.value.durationDays, 'day')
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
}
