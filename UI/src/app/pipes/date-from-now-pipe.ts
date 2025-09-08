import { Pipe, PipeTransform } from '@angular/core';
import {Moment} from 'moment';
import 'moment/locale/ru';

@Pipe({
  standalone: true,
  name: 'dateFromNow'
})
export class DateFromNowPipe implements PipeTransform {

  transform(value: Moment | null | undefined, ...args: unknown[]): unknown {
    if (!value)
      return value;
    return value.locale('ru').fromNow();
  }

}
