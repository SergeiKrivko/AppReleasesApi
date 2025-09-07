import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {Logo} from '../../components/logo/logo';
import {TuiAppearance, TuiButton, TuiLabel, TuiTextfield} from '@taiga-ui/core';
import {TuiCard} from '@taiga-ui/layout';
import {TuiPassword} from '@taiga-ui/kit';
import {FormControl, FormGroup, ReactiveFormsModule} from '@angular/forms';
import {AuthService} from '../../services/auth.service';
import {Router} from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-auth-page',
  imports: [
    Logo,
    TuiAppearance,
    TuiCard,
    TuiLabel,
    TuiTextfield,
    TuiPassword,
    TuiButton,
    ReactiveFormsModule
  ],
  templateUrl: './auth-page.html',
  styleUrl: './auth-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AuthPage {
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  protected readonly control = new FormGroup({
    login: new FormControl<string>(""),
    password: new FormControl<string>(""),
  })

  protected authorize() {
    if (this.authService.authorize(this.control.value.login ?? null, this.control.value.password ?? null))
      void this.router.navigateByUrl('/');
  }
}
