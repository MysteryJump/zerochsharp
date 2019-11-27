export class Session {
  createdAt: Date;
  sessionToken: string;
  expired: Date;
}

export class UserSession extends Session {
  userName: string;
  authority: number;
}
