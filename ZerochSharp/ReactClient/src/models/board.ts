export interface Board {
  id: number;
  boardKey: string;
  boardName: string;
  key: number;
  boardDefaultName: string;
  boardSubTitle: string;
  autoArchivingPredicates?: string[];
}
