import {ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, input, OnInit} from '@angular/core';
import {Observable, tap} from 'rxjs';
import {HistogramMetricEntity} from '../../entities/metric-entity';
import {AsIntegerPipe} from '../../pipes/as-integer-pipe';
import {tuiFormatNumber, TuiHint, TuiLabel} from '@taiga-ui/core';
import {TuiAxes, TuiBarChart, TuiRingChart} from '@taiga-ui/addon-charts';
import {TuiContext} from '@taiga-ui/cdk';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';

@Component({
  standalone: true,
  selector: 'app-histogram-chart',
  imports: [
    AsIntegerPipe,
    TuiLabel,
    TuiRingChart,
    TuiAxes,
    TuiBarChart,
    TuiHint
  ],
  templateUrl: './histogram-chart.html',
  styleUrl: './histogram-chart.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HistogramChart implements OnInit {
  data = input.required<Observable<HistogramMetricEntity[]>>();
  header = input<string>();
  private readonly destroyRef = inject(DestroyRef);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  protected value: number[][] = [];
  protected labelsX: string[] = [];
  protected labelsY: string[] = [];
  protected maximum: number = 1;

  ngOnInit() {
    this.data().pipe(
      tap(metrics => {
        if (metrics.length > 0) {
          this.value = metrics.map(m => m.values.map(e => e.value));
          this.maximum = 0;
          for (const row of this.value)
            for (const x of row)
              if (x > this.maximum)
                this.maximum = x;
          this.labelsX = metrics[0].values.map(e => e.key);
          this.labelsY = ["0", this.maximum.toFixed()];
          this.changeDetectorRef.detectChanges();
        }
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  protected readonly hint = ({$implicit}: TuiContext<number>): string =>
    this.value
      .reduce(
        (result, set) => set[$implicit].toFixed(),
        '',
      )
      .trim();
}
