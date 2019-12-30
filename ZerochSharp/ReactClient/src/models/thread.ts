export interface Thread {
  title: string;
  threadId: number;
  key?: number; 
  created: string;
  boardKey?: string; 
  author: string;
  modified: string;
  responseCount: number;
  datKey?: number;
  influence: number;
  responses?: Response[];
}
