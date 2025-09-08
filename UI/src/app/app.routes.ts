import {Routes} from '@angular/router';
import {HomePage} from './pages/home-page/home-page';
import {ApplicationConfigPage} from './pages/application-config-page/application-config-page';
import {ReleasesPage} from './pages/releases-page/releases-page';
import {BranchesPage} from './pages/branches-page/branches-page';
import {AuthPage} from './pages/auth-page/auth-page';
import {TokensPage} from './pages/tokens-page/tokens-page';
import {SingleReleasePage} from './pages/single-release-page/single-release-page';
import {ApplicationPage} from './pages/application-page/application-page';

export const routes: Routes = [
  {path: '', component: HomePage},
  {path: 'auth', component: AuthPage},
  {path: 'tokens', component: TokensPage},
  {
    path: 'app/:id', component: ApplicationPage, children: [
      {path: 'config', component: ApplicationConfigPage},
      {path: 'releases', component: ReleasesPage,},
      {path: 'releases/:releaseId', component: SingleReleasePage},
      {path: 'branches', component: BranchesPage},
    ]
  },
  {path: "**", redirectTo: "/", pathMatch: "full"}
];
