import {Moment} from 'moment';

export interface MetricEntity {
  name: string;
  fields: { [key: string]: string; };
  timestamp: Moment;
  value: string | undefined;
}
