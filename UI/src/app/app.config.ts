import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {NG_EVENT_PLUGINS} from '@taiga-ui/event-plugins';
import {provideHttpClient} from '@angular/common/http';
import {API_BASE_URL} from './services/api-client';
import {provideAnimations} from '@angular/platform-browser/animations';

export const appConfig: ApplicationConfig = {
  providers: [
    // { provide: API_BASE_URL, useValue: "." },
    { provide: API_BASE_URL, useValue: "http://localhost:5000" },
    // { provide: API_BASE_URL, useValue: "https://releases.nachert.art" },
    provideAnimations(),
    provideZoneChangeDetection({eventCoalescing: true}),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(),
    NG_EVENT_PLUGINS
  ]
};
