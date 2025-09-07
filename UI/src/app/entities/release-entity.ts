import {Moment} from 'moment';

export interface ReleaseEntity {
  id: string;
  branchId: string;
  platform: string | null;
  version: string;
  releaseNotes: string | undefined;
  createdAt: Moment;
  deletedAt: Moment | null;
}
