export interface ApplicationEntity {
  key: string;
  name: string;
  description: string | undefined;
  createdAt: Date;
  deletedAt: Date;
}
