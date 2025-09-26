import {ChangeDetectionStrategy, Component, inject} from '@angular/core';
import {MetricService} from '../../services/metric.service';
import {ApplicationService} from '../../services/application.service';
import {map, Observable, of, switchMap} from 'rxjs';
import {PieChart} from '../../components/pie-chart/pie-chart';
import {TuiLabel} from '@taiga-ui/core';
import {HistogramChart} from '../../components/histogram-chart/histogram-chart';
import {HistogramMetricEntity} from '../../entities/metric-entity';

@Component({
  standalone: true,
  selector: 'app-metrics-page',
  imports: [
    PieChart,
    TuiLabel,
    HistogramChart
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
        return this.metricService.getApplicationDownloadForPlatformsCount(selectedApplication);
      return of([]);
    })
  );

  protected readonly downloadAssetsMetrics$: Observable<HistogramMetricEntity[]> = this.applicationService.selectedApplication$.pipe(
    switchMap(selectedApplication => {
      if (selectedApplication)
        return this.metricService.getAssetsDownloadDurations(selectedApplication).pipe(
          map(m => [m]),
        );
      return of([]);
    })
  );
}
