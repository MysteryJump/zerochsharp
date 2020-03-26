import { Thread } from "../../models/thread";

export interface BoardState {
  boardKey: string;
  boardName: string;
  children: Thread[];
  // this property is maybe available in archived thread list
  childrenCount?: number;
}
