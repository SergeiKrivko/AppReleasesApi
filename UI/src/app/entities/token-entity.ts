import {Moment} from 'moment';

export interface TokenEntity {
  id: string;
  name: string;
  createdAt: Moment;
  expiresAt: Moment;
  revokedAt: Moment | null;
  mask: string;
}
