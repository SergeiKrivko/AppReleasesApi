import {Moment} from 'moment';

export interface BundleEntity {
  id: string;
  releaseId: string;
  fileName: string;
  createdAt: Moment;
  deletedAt: Moment | null;
}
