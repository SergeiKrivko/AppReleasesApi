import { Pipe, PipeTransform } from '@angular/core';
import {Moment} from 'moment';
import 'moment/locale/ru';

@Pipe({
  standalone: true,
  name: 'dateFromNow'
})
export class DateFromNowPipe implements PipeTransform {

  transform(value: Moment, ...args: unknown[]): unknown {
    return value.locale('ru').fromNow();
  }

}
