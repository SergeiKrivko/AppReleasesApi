import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  standalone: true,
  name: 'asInteger'
})
export class AsIntegerPipe implements PipeTransform {

  transform(value: number | null | undefined): string | undefined {
    return value?.toFixed();
  }

}
