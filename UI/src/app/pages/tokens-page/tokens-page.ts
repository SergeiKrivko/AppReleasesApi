import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {TokensService} from '../../services/tokens.service';
import {Logo} from '../../components/logo/logo';
import {ApplicationCard} from '../../components/application-card/application-card';
import {AsyncPipe} from '@angular/common';
import {TuiButton} from '@taiga-ui/core';
import {TuiCardLarge} from '@taiga-ui/layout';

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

  protected readonly tokens$ = this.tokensService.tokens$;

  protected newToken() {

  }
}
