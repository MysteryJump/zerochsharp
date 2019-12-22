import { Action } from 'typescript-fsa';

export interface MainContainerActions {
  setCurrentName: (name: string) => Action<{ name: string }>;
}
