import {inject, Injectable} from '@angular/core';
import {ApiClient, Metric} from './api-client';
import {map, Observable} from 'rxjs';
import {MetricEntity} from '../entities/metric-entity';
import {PieMetricEntity} from '../entities/pie-metric-entity';
import {ReleaseEntity} from '../entities/release-entity';
import moment from 'moment';
import {ApplicationEntity} from '../entities/application-entity';

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private readonly apiClient = inject(ApiClient);

  private getMetrics(query: string): Observable<MetricEntity[]> {
    return this.apiClient.metrics(query, undefined).pipe(
      map(metrics => metrics.map(metricToEntity))
    )
  }

  getReleaseDownloadsCount(release: ReleaseEntity): Observable<number> {
    return this.getMetrics(`increase(download_release_total{release="${release.id}"}[${moment().diff(release.createdAt, 'days')}d])`).pipe(
      map(metrics => metrics[0]),
      map(metric => metric ? Number(metric.value) ?? 0 : 0)
    )
  }

  getApplicationDownloadForPlatformsCount(application: ApplicationEntity): Observable<PieMetricEntity[]> {
    return this.getMetrics(`increase(download_release_total{application="${application.key}"}[${moment().diff(application.createdAt, 'days')}d])`).pipe(
      map(metrics => {
        const map = new Map<string, number>;
        for (const metric of metrics)
          map.set(metric.fields["platform"], (map.get(metric.fields["platform"]) ?? 0) + (Number(metric.value) ?? 0));
        const result: PieMetricEntity[] = [];
        map.forEach((value, key) => result.push({key, value}));
        return result;
      })
    )
  }
}

const metricToEntity = (metric: Metric): MetricEntity => ({
  name: metric.name ?? "",
  fields: metric.fields ?? {},
  timestamp: metric.timestamp ?? moment(),
  value: metric.value
});

