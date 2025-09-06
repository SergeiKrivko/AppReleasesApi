import {Routes} from '@angular/router';
import {HomePage} from './pages/home-page/home-page';
import {ApplicationPage} from './pages/application-page/application-page';
import {ReleasesPage} from './pages/releases-page/releases-page';
import {BranchesPage} from './pages/branches-page/branches-page';

export const routes: Routes = [
  {path: '', component: HomePage},
  {path: 'app/:id', component: ApplicationPage},
  {path: 'app/:id/releases', component: ReleasesPage},
  {path: 'app/:id/branches', component: BranchesPage},
  {path: "**", redirectTo: "/", pathMatch: "full"}
];
