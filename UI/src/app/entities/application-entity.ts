import {Duration, Moment} from 'moment';

export interface ApplicationEntity {
  id: string;
  key: string;
  name: string;
  description: string | undefined;
  mainBranch: string;
  defaultDuration: Duration | null;
  createdAt: Moment;
  deletedAt: Moment | null;
}
