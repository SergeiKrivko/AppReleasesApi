import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {injectContext} from '@taiga-ui/polymorpheus';
import {TuiDialogContext, TuiTextfield} from '@taiga-ui/core';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {first, tap} from 'rxjs';
import {BranchService} from '../../services/branch.service';
import {TuiButtonLoading} from '@taiga-ui/kit';

@Component({
  standalone: true,
  selector: 'app-new-branch-dialog',
  imports: [
    TuiTextfield,
    ReactiveFormsModule,
    TuiButtonLoading
  ],
  templateUrl: './new-branch-dialog.html',
  styleUrl: './new-branch-dialog.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NewBranchDialog {
  private readonly branchService = inject(BranchService);

  private readonly context = injectContext<TuiDialogContext>();

  protected readonly control = new FormGroup({
    name: new FormControl<string>(""),
  })

  protected loading: boolean = false;

  protected submit() {
    this.loading = true;
    this.branchService.createNewBranch(this.control.value.name ?? "").pipe(
        tap(() => {
          this.context.completeWith();
          this.loading = false;
        }),
        first()
    ).subscribe();
  }
}
