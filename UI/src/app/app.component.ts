import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {TuiRoot} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class App {
  protected readonly title = signal('Avalux Releases');
}
