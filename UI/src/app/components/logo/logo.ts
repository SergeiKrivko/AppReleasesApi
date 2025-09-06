import { Component } from '@angular/core';
import {RouterLink} from "@angular/router";

@Component({
  standalone: true,
  selector: 'app-logo',
    imports: [
        RouterLink
    ],
  templateUrl: './logo.html',
  styleUrl: './logo.scss'
})
export class Logo {

}
