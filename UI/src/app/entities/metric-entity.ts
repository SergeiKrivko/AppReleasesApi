import {Moment} from 'moment';

export interface MetricEntity {
  name: string;
  fields: { [key: string]: string; };
  timestamp: Moment;
  value: string | undefined;
}

export interface PieMetricEntity {
  key: string,
  value: number,
}

export interface HistogramMetricEntity {
  key: string,
  values: PieMetricEntity[],
}
