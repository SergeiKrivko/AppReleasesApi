import { Pipe, PipeTransform } from '@angular/core';
import {Moment} from 'moment';
import 'moment/locale/ru';

@Pipe({
  standalone: true,
  name: 'formatDate'
})
export class FormatDatePipe implements PipeTransform {

  transform(value: Moment, ...args: unknown[]): unknown {
    return value.locale('ru').format('DD MMMM YYYY');
  }

}
