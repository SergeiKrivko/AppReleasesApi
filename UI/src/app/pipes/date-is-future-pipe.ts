import { Pipe, PipeTransform } from '@angular/core';
import moment, {Moment} from 'moment';

@Pipe({
  standalone: true,
  name: 'dateIsFuture'
})
export class DateIsFuturePipe implements PipeTransform {

  transform(value: Moment, ...args: unknown[]): unknown {
    return value > moment();
  }

}
