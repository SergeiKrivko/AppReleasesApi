import {inject, Injectable} from '@angular/core';
import {ApiClient, Metric} from './api-client';
import {combineLatest, map, NEVER, Observable, switchMap, tap} from 'rxjs';
import {MetricEntity} from '../entities/metric-entity';
import moment from 'moment';
import {patchState, signalState} from '@ngrx/signals';
import {toObservable} from '@angular/core/rxjs-interop';

interface MetricsStore {
  downloadReleaseMetrics: MetricEntity[];
  downloadAssetsMetrics: MetricEntity[];
  downloadInstallerMetrics: MetricEntity[];
}

@Injectable({
  providedIn: 'root'
})
export class MetricService {
  private readonly apiClient = inject(ApiClient);

  private readonly metrics$$ = signalState<MetricsStore>({
    downloadReleaseMetrics: [],
    downloadAssetsMetrics: [],
    downloadInstallerMetrics: [],
  });

  private readonly downloadReleaseMetrics$ = toObservable(this.metrics$$.downloadReleaseMetrics);

  private getMetrics(query: string): Observable<MetricEntity[]> {
    return this.apiClient.metrics(query, undefined).pipe(
      map(metrics => metrics.map(metricToEntity))
    )
  }

  loadMetrics(): Observable<never> {
    return combineLatest(
      this.getMetrics("download_assets_duration_seconds").pipe(
        tap(m => patchState(this.metrics$$, {downloadAssetsMetrics: m}))
      ),
      this.getMetrics("download_installer_total").pipe(
        tap(m => patchState(this.metrics$$, {downloadInstallerMetrics: m}))
      ),
      this.getMetrics("download_release_total").pipe(
        tap(m => patchState(this.metrics$$, {downloadReleaseMetrics: m}))
      ),
    ).pipe(
      switchMap(() => NEVER),
    );
  }

  getReleaseDownloadsCount(releaseId: string): Observable<number> {
    return this.downloadReleaseMetrics$.pipe(
      map(metrics => metrics.filter(m => m.fields["release"] == releaseId)[0]),
      map(metric => metric ? Number(metric.value) ?? 0 : 0)
    )
  }
}

const metricToEntity = (metric: Metric): MetricEntity => ({
  name: metric.name ?? "",
  fields: metric.fields ?? {},
  timestamp: metric.timestamp ?? moment(),
  value: metric.value
});

