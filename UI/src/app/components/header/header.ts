import { Component } from '@angular/core';
import {Logo} from '../logo/logo';
import {TuiButton} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-header',
  imports: [
    Logo,
    TuiButton
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {

}
