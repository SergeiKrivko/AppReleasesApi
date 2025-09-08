import {ChangeDetectionStrategy, ChangeDetectorRef, Component, inject} from '@angular/core';
import {TuiButton, TuiLoader, TuiTextfield} from '@taiga-ui/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {TuiButtonLoading, TuiCopyComponent, TuiInputDate} from '@taiga-ui/kit';
import {TokensService} from '../../services/tokens.service';
import {first, tap} from 'rxjs';
import {TuiDay} from '@taiga-ui/cdk';
import moment from 'moment';

enum Status {
  NotStarted = 0,
  InProgress = 1,
  Completed = 2,
}

@Component({
  standalone: true,
  selector: 'app-new-branch-dialog',
  imports: [
    TuiTextfield,
    ReactiveFormsModule,
    TuiButtonLoading,
    TuiButton,
    TuiLoader,
    TuiCopyComponent,
    TuiInputDate
  ],
  templateUrl: './new-token-dialog.html',
  styleUrl: './new-token-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewTokenDialog {
  private readonly tokensService = inject(TokensService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  protected readonly control = new FormGroup({
    name: new FormControl<string>(""),
    mask: new FormControl<string>("*"),
    expiresAt: new FormControl<TuiDay>(TuiDay.currentLocal().append({day: 30}))
  })

  protected status: Status = Status.NotStarted;
  protected token: string | undefined;

  protected submit() {
    const value = this.control.value;
    if (!value.name || !value.mask)
      return;
    this.status = Status.InProgress;
    this.tokensService.createToken(value.name, value.mask, moment(value.expiresAt?.toJSON())).pipe(
      tap(token => {
        this.status = Status.Completed;
        this.token = token;
        this.changeDetectorRef.detectChanges();
      }),
      first(),
    ).subscribe();
  }

  protected readonly Status = Status;
}
