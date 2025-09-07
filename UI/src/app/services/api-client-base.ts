import {Injectable} from "@angular/core";

@Injectable()
export class ApiClientBase {

  transformOptions(options: any): Promise<any> {
    options.headers = options.headers.set("Authorization", "Basic YWRtaW46YWRtaW4=");
    return Promise.resolve(options);
  }
}
