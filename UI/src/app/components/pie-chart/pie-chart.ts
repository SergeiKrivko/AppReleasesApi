import {ChangeDetectionStrategy, ChangeDetectorRef, Component, DestroyRef, inject, input, OnInit} from '@angular/core';
import {PieMetricEntity} from '../../entities/pie-metric-entity';
import {Observable, tap} from 'rxjs';
import {tuiSum} from '@taiga-ui/cdk';
import {TuiRingChart} from '@taiga-ui/addon-charts';
import {AsyncPipe} from '@angular/common';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {TuiLabel} from '@taiga-ui/core';
import {AsIntegerPipe} from '../../pipes/as-integer-pipe';

@Component({
  standalone: true,
  selector: 'app-pie-chart',
  imports: [
    TuiRingChart,
    AsyncPipe,
    TuiLabel,
    AsIntegerPipe
  ],
  templateUrl: './pie-chart.html',
  styleUrl: './pie-chart.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class PieChart implements OnInit {
  data = input.required<Observable<PieMetricEntity[]>>();
  header = input<string>();
  private readonly destroyRef = inject(DestroyRef);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  ngOnInit() {
    this.data().pipe(
      tap(metrics => {
        this.values = metrics.map(m => m.value);
        this.labels = metrics.map(m => m.key);
        this.changeDetectorRef.detectChanges();
      }),
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
