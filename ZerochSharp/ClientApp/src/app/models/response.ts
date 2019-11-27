export class Response {
  id: number;
  author: string;
  created: Date;
  threadId: number;
  body: string;
  mail: string;
  name: string;
  hostAddress: string | undefined;
  isEditMode: boolean | undefined | null;
  editedText: string | undefined;
  isAboned: boolean;
}