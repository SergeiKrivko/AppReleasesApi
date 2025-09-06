import {Component, inject} from '@angular/core';
import {Logo} from '../../components/logo/logo';
import {ApplicationService} from '../../services/application.service';
import {AsyncPipe} from '@angular/common';
import {ApplicationCard} from '../../components/application-card/application-card';

@Component({
  standalone: true,
  selector: 'app-home-page',
  imports: [
    Logo,
    AsyncPipe,
    ApplicationCard
  ],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss'
})
export class HomePage {
  private readonly applicationService = inject(ApplicationService);

  protected readonly applications$ = this.applicationService.applications$;
}
