import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {NG_EVENT_PLUGINS} from '@taiga-ui/event-plugins';
import {provideHttpClient} from '@angular/common/http';
import {API_BASE_URL} from './services/api-client';
import {provideAnimations} from '@angular/platform-browser/animations';

export const appConfig: ApplicationConfig = {
  providers: [
    // { provide: API_BASE_URL, useValue: "https://scheduleai-bff.nachert.art" },
    { provide: API_BASE_URL, useValue: "http://localhost:5282" },
    provideAnimations(),
    provideZoneChangeDetection({eventCoalescing: true}),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    NG_EVENT_PLUGINS
  ]
};
