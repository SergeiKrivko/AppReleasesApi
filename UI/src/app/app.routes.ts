import {Routes} from '@angular/router';
import {HomePage} from './pages/home-page/home-page';
import {ApplicationPage} from './pages/application-page/application-page';

export const routes: Routes = [
  {path: '', component: HomePage},
  {path: 'app/:id', component: ApplicationPage},
  {path: "**", redirectTo: "/", pathMatch: "full"}
];
