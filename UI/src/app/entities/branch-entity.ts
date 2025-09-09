import {Duration, Moment} from 'moment';

export interface BranchEntity {
  id: string;
  applicationId: string;
  name: string;
  createdAt: Moment | null;
  deletedAt: Moment | null;
  releaseLifetime: Duration | null;
  latestReleaseLifetime: Duration | null;
  useDefaultReleaseLifetime: boolean;
}
