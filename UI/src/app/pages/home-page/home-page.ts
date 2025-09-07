import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {Logo} from '../../components/logo/logo';
import {ApplicationService} from '../../services/application.service';
import {AsyncPipe} from '@angular/common';
import {ApplicationCard} from '../../components/application-card/application-card';
import {TuiButton, tuiDialog} from '@taiga-ui/core';
import {NewApplicationDialog} from '../../components/new-application-dialog/new-application-dialog';
import {AuthService} from '../../services/auth.service';

@Component({
  standalone: true,
  selector: 'app-home-page',
  imports: [
    Logo,
    AsyncPipe,
    ApplicationCard,
    TuiButton
  ],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class HomePage {
  private readonly applicationService = inject(ApplicationService);
  private readonly authService = inject(AuthService);

  protected readonly applications$ = this.applicationService.applications$;

  private readonly newAppDialog = tuiDialog(NewApplicationDialog, {
    dismissible: false,
    label: 'Новое приложение',
  });

  protected newApp(): void {
    this.newAppDialog(undefined).subscribe();
  }

  protected logOut(): void {
    this.authService.logOut();
  }
}
