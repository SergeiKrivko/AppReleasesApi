import {ChangeDetectionStrategy, Component, DestroyRef, inject, input, OnInit} from '@angular/core';
import {PieMetricEntity} from '../../entities/pie-metric-entity';
import {map, Observable, tap} from 'rxjs';
import {tuiSum} from '@taiga-ui/cdk';
import {TuiRingChart} from '@taiga-ui/addon-charts';
import {AsyncPipe} from '@angular/common';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TuiLabel} from '@taiga-ui/core';

@Component({
  standalone: true,
  selector: 'app-pie-chart',
  imports: [
    TuiRingChart,
    AsyncPipe,
    TuiLabel
  ],
  templateUrl: './pie-chart.html',
  styleUrl: './pie-chart.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PieChart implements OnInit {
  data = input.required<Observable<PieMetricEntity[]>>();
  header = input<string>();
  private readonly destroyRef = inject(DestroyRef);

  ngOnInit() {
    this.data().pipe(
      tap(metrics => {
        console.log(metrics);
        this.values = metrics.map(m => m.value);
        this.labels = metrics.map(m => m.key);
      }),
      map(metrics => metrics.map(m => m.value)),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe();
  }

  protected values: number[] = [];
  protected labels: string[] = [];
  protected index: number = NaN;

  protected get sum(): number {
    return (Number.isNaN(this.index) ? tuiSum(...this.values) : this.values[this.index]) ?? 0;
  }

  protected get label(): string {
    return (Number.isNaN(this.index) ? 'Total' : this.labels[this.index]) ?? '';
  }
}
