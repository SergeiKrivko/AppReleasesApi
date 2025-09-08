import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {TokensService} from '../../services/tokens.service';
import {Logo} from '../../components/logo/logo';
import {ApplicationCard} from '../../components/application-card/application-card';
import {AsyncPipe} from '@angular/common';
import {TuiButton, tuiDialog, TuiDialogService} from '@taiga-ui/core';
import {TuiCardLarge} from '@taiga-ui/layout';
import {NewTokenDialog} from '../../components/new-token-dialog/new-token-dialog';
import {TUI_CONFIRM, TuiConfirmData} from '@taiga-ui/kit';
import {NEVER, switchMap} from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-tokens-page',
  imports: [
    Logo,
    ApplicationCard,
    AsyncPipe,
    TuiButton,
    TuiCardLarge
  ],
  templateUrl: './tokens-page.html',
  styleUrl: './tokens-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TokensPage {
  private readonly tokensService = inject(TokensService);
  private readonly dialogs = inject(TuiDialogService);

  protected readonly tokens$ = this.tokensService.tokens$;

  private readonly newTokenDialog = tuiDialog(NewTokenDialog, {
    dismissible: false,
    label: 'Новый токен',
  });

  protected newToken(): void {
    this.newTokenDialog(undefined).subscribe();
  }

  protected deleteToken(id: string): void {
    const data: TuiConfirmData = {
            content: 'Вы уверены, что хотите отозвать токен?',
            yes: 'Да',
            no: 'Нет',
        };
        this.dialogs
            .open<boolean>(TUI_CONFIRM, {
                label: 'Удаление токена',
                size: 's',
                data,
            })
            .pipe(
              switchMap(result => {
                if (result) {
                  return this.tokensService.revokeToken(id);
                }
                return NEVER;
              }),
            )
            .subscribe();
  }
}
