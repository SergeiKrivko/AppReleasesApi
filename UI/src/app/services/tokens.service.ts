import {inject, Injectable} from '@angular/core';
import {TokenEntity} from '../entities/token-entity';
import {LoadingStatus} from '../entities/loading-status';
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {EMPTY, map, Observable, switchMap, tap} from 'rxjs';
import {ApiClient, CreateTokenSchema, Token} from './api-client';
import {Moment} from 'moment';

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

  createToken(name: string, mask: string, expiresAt: Moment): Observable<string | undefined> {
    return this.apiClient.tokensPOST(CreateTokenSchema.fromJS({
      name,
      mask,
      expiresAt,
    })).pipe(
      map(resp => resp.token),
    );
  }

  revokeToken(id: string) {
    return this.apiClient.tokensDELETE(id).pipe(
      tap(() => {
        const tokens = getState(this.tokens$$).tokens;
        patchState(this.tokens$$, {
          tokens: tokens.filter(a => a.id != id)
        });
      }),
    );
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

