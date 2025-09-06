import {Component, inject} from '@angular/core';
import {Logo} from '../logo/logo';
import {TuiButton} from '@taiga-ui/core';
import {RouterLink} from '@angular/router';
import {TuiSegmented} from '@taiga-ui/kit';
import {ApplicationService} from '../../services/application.service';
import {AsyncPipe} from '@angular/common';

@Component({
  standalone: true,
  selector: 'app-header',
  imports: [
    Logo,
    TuiButton,
    RouterLink,
    AsyncPipe,
    TuiSegmented
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  private readonly applicationService = inject(ApplicationService);

  protected readonly selectedApplication$ = this.applicationService.selectedApplication$;
}
