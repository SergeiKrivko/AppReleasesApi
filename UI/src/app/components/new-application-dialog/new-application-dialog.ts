import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {TuiButton, TuiDialogContext, TuiLabel, TuiTextfield} from '@taiga-ui/core';
import {injectContext} from '@taiga-ui/polymorpheus';
import {TuiButtonLoading, TuiTextarea} from '@taiga-ui/kit';
import {ApplicationService} from '../../services/application.service';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {first, tap} from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-new-application-dialog',
  imports: [
    TuiLabel,
    TuiTextfield,
    TuiTextarea,
    TuiButton,
    TuiButtonLoading,
    ReactiveFormsModule
  ],
  templateUrl: './new-application-dialog.html',
  styleUrl: './new-application-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class NewApplicationDialog {
  private readonly applicationService = inject(ApplicationService);

  private readonly context = injectContext<TuiDialogContext>();

  protected readonly control = new FormGroup({
    key: new FormControl<string>(""),
    name: new FormControl<string>(""),
    description: new FormControl<string>(""),
  })

  protected loading: boolean = false;

  protected submit() {
    this.loading = true;
    this.applicationService.createNewApplication(
      this.control.value.key ?? "",
      this.control.value.name ?? "",
      this.control.value.description ?? "").pipe(
        tap(() => {
          this.context.completeWith();
          this.loading = false;
        }),
        first()
    ).subscribe();
  }
}
