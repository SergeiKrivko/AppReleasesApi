import {inject, Injectable} from "@angular/core";
import {AuthService} from './auth.service';

@Injectable()
export class ApiClientBase {
  private readonly authService = inject(AuthService);

  transformOptions(options: any): Promise<any> {
    const authorization = this.authService.getAuthorizationHeader();
    if (authorization) {
      options.headers = options.headers.set("Authorization", authorization);
    }
    return Promise.resolve(options);
  }
}
