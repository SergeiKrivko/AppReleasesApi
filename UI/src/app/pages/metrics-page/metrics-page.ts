import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {MetricService} from '../../services/metric.service';
import {ApplicationService} from '../../services/application.service';
import {of, switchMap} from 'rxjs';
import {PieChart} from '../../components/pie-chart/pie-chart';
import {TuiLabel} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-metrics-page',
  imports: [
    PieChart,
    TuiLabel
  ],
  templateUrl: './metrics-page.html',
  styleUrl: './metrics-page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MetricsPage {
  private readonly metricService = inject(MetricService);
  private readonly applicationService = inject(ApplicationService);

  protected readonly platformMetrics$ = this.applicationService.selectedApplication$.pipe(
    switchMap(selectedApplication => {
      if (selectedApplication)
        return this.metricService.getApplicationDownloadForPlatformsCount(selectedApplication.key);
      return of([]);
    })
  );
}
