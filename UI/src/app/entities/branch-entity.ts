import {Duration, Moment} from 'moment';

export interface BranchEntity {
  id: string;
  name: string;
  createdAt: Moment | null;
  deletedAt: Moment | null;
  duration: Duration | null;
}
