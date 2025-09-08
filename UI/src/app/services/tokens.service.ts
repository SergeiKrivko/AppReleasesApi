import {inject, Injectable} from '@angular/core';
import {TokenEntity} from '../entities/token-entity';
import {LoadingStatus} from '../entities/loading-status';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {EMPTY, Observable, switchMap, tap} from 'rxjs';
import {ApiClient, Token} from './api-client';

interface TokensStore {
  tokens: TokenEntity[];
  loadingStatus: LoadingStatus;
}

@Injectable({
  providedIn: 'root'
})
export class TokensService {
  private readonly apiClient = inject(ApiClient);

  private readonly tokens$$ = signalState<TokensStore>({
    tokens: [],
    loadingStatus: LoadingStatus.NotStarted,
  });

  readonly tokens$ = toObservable(this.tokens$$.tokens);

  loadTokens(): Observable<never> {
    patchState(this.tokens$$, {loadingStatus: LoadingStatus.InProgress});
    return this.apiClient.tokensAll().pipe(
      tap(tokens => {
        patchState(this.tokens$$, {tokens: tokens.map(tokenToEntity), loadingStatus: LoadingStatus.Completed});
      }),
      switchMap(() => EMPTY),
    )
  }
}

const tokenToEntity = (token: Token): TokenEntity => ({
  id: token.id ?? "",
  name: token.name ?? "",
  mask: token.mask ?? "*",
  createdAt: token.issuedAt,
  expiresAt: token.expiresAt,
  revokedAt: token.revokedAt ?? null,
});

