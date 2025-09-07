import {Injectable} from "@angular/core";
import {getState, patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';
import {map, Observable} from 'rxjs';

interface CredentialsStore {
  login: string | null;
  password: string | null;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly credentials$$ = signalState<CredentialsStore>({
    login: localStorage.getItem('avalux-releases-login'),
    password: localStorage.getItem('avalux-releases-password'),
  });

  getAuthorizationHeader(): string | null {
    const credentials = getState(this.credentials$$);
    if (credentials.login && credentials.password) {
      return "Basic " + btoa(credentials.login + ':' + credentials.password);
    }
    return null;
  }

  isAuthorized$: Observable<boolean> = toObservable(this.credentials$$).pipe(
    map(store => store.login !== null && store.password !== null)
  );

  authorize(login: string | null, password: string | null): boolean {
    if (login && password) {
      patchState(this.credentials$$, {login, password});
      localStorage.setItem('avalux-releases-login', login);
      localStorage.setItem('avalux-releases-password', password);
      return true;
    }
    return false;
  }
}
