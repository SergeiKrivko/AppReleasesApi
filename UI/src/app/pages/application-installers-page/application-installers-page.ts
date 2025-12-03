import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {InstallersService} from '../../services/installers.service';
import {AsyncPipe} from '@angular/common';
import {TuiAppearance, TuiButton} from '@taiga-ui/core';
import {TuiCardLarge} from '@taiga-ui/layout';
import {TuiLet} from '@taiga-ui/cdk';
import {UsingInstallerBuilderEntity} from '../../entities/using-installer-builder-entity';
import {map, Observable} from 'rxjs';
import {AvailableInstallerBuilderEntity} from '../../entities/available-installer-builder-entity';

@Component({
  selector: 'app-application-installers-page',
  imports: [
    AsyncPipe,
    TuiAppearance,
    TuiButton,
    TuiCardLarge,
    TuiLet
  ],
  templateUrl: './application-installers-page.html',
  styleUrl: './application-installers-page.scss',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ApplicationInstallersPage {
  private readonly installersService = inject(InstallersService);

  protected readonly usingInstallers$ = this.installersService.usingInstallers$;
  protected readonly availableInstallers$ = this.installersService.availableInstallers$;

  getBuilder(usingBuilder: UsingInstallerBuilderEntity): Observable<AvailableInstallerBuilderEntity | undefined> {
    return this.availableInstallers$.pipe(
      map(available => available.find(e => e.key == usingBuilder.builderKey))
    );
  }
}
