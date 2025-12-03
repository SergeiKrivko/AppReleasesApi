import {Duration} from 'moment';

export interface UsingInstallerBuilderEntity {
  id: string,
  builderKey: string,
  name: string | null,
  settings: object,
  installerLifetime: Duration,
}
