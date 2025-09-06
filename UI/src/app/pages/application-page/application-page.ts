import {ChangeDetectionStrategy, Component, DestroyRef, inject, OnInit} from '@angular/core';
import {Header} from '../../components/header/header';
import {ApplicationService} from '../../services/application.service';
import {ActivatedRoute} from '@angular/router';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {first, NEVER, switchMap, tap} from 'rxjs';
import {FormControl, FormGroup, FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TuiButton, TuiLabel, TuiTextfieldComponent, TuiTextfieldDirective} from '@taiga-ui/core';
import {TuiTextarea} from '@taiga-ui/kit';
import {ApplicationEntity} from '../../entities/application-entity';

@Component({
  standalone: true,
  selector: 'app-application-page',
  imports: [
    Header,
    FormsModule,
    ReactiveFormsModule,
    TuiTextfieldComponent,
    TuiTextfieldDirective,
    TuiTextarea,
    TuiLabel,
    TuiButton
  ],
  templateUrl: './application-page.html',
  styleUrl: './application-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ApplicationPage implements OnInit {
  private readonly applicationService = inject(ApplicationService);
  private readonly route = inject(ActivatedRoute);
  private readonly destroyRef = inject(DestroyRef);

  private applicationId: string | undefined;

  ngOnInit() {
    this.route.params.pipe(
      switchMap(params => {
        const appId = params['id'];
        if (appId)
          return this.applicationService.applicationById(appId);
        return NEVER;
      }),
      tap(app => {
          if (app) {
            this.applicationService.selectApplication(app);
            this.loadApplication(app);
          }
        }
      ),
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
    });
  }

  protected control = new FormGroup({
    key: new FormControl<string>(""),
    name: new FormControl<string>(""),
    description: new FormControl<string>(""),
    mainBranch: new FormControl<string>(""),
  })

  protected isEditing: boolean = false;

  protected startEditing() {
    this.isEditing = true;
  }

  protected saveChanges() {
    this.isEditing = false;
    if (this.applicationId)
      this.applicationService.updateApplication(
        this.applicationId,
        this.control.value.name ?? undefined,
        this.control.value.description ?? undefined,
        this.control.value.mainBranch ?? undefined,
        undefined
      ).subscribe();
  }

  protected cancelChanges() {
    this.isEditing = false;
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
}
