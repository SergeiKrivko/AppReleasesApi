import {Moment} from 'moment';

export interface InstallerEntity {
  id: string;
  releaseId: string;
  fileName: string;
  createdAt: Moment;
  deletedAt: Moment | null;
}
