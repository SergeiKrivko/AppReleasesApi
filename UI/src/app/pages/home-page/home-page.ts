import { Component } from '@angular/core';
import {Logo} from '../../components/logo/logo';

@Component({
  standalone: true,
  selector: 'app-home-page',
  imports: [
    Logo
  ],
  templateUrl: './home-page.html',
  styleUrl: './home-page.scss'
})
export class HomePage {

}
