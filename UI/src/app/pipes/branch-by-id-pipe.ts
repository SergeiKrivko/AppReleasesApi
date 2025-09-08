import {inject, Pipe, PipeTransform} from '@angular/core';
import {BranchService} from '../services/branch.service';
import {Observable, of} from 'rxjs';
import {BranchEntity} from '../entities/branch-entity';

@Pipe({
  standalone: true,
  name: 'branchById'
})
export class BranchByIdPipe implements PipeTransform {
  private readonly branchService = inject(BranchService);

  transform(branchId: string | undefined, ...args: unknown[]): Observable<BranchEntity | undefined> {
    if (!branchId)
      return of(undefined);
    return this.branchService.branchById(branchId);
  }

}
