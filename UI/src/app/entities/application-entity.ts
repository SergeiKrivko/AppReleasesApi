import {Moment} from 'moment';

export interface ApplicationEntity {
  id: string;
  key: string;
  name: string;
  description: string | undefined;
  createdAt: Moment;
  deletedAt: Moment | null;
}
