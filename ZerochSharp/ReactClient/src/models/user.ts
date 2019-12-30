export interface User {
  userId: string;
  authority: Authority;
  setAuthorization: string;
}

export enum Authority {
  Normal = 1,
  Admin = 1 << 1,
  Restricted = 1 << 2
}
