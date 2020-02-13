export interface User {
  userId: string;
  systemAuthority: SystemAuthority;
  setAuthorization: string;
  controllableBoards?: string;
}

export enum SystemAuthority {
  Admin = 1 << 0, // System administrator (root user)
  ThreadStop = 1 << 1,
  ThreadArchive = 1 << 2,
  BBSSetting = 1 << 3,
  CapUserSetting = 1 << 4,
  AboneResponse = 1 << 5,
  EditResponse = 1 << 6,
  RemoveResponse = 1 << 7,
  ViewResponseDetail = 1 << 8,
  Owner = 1 << 9, // System administrator (contains root user authority)
  BoardsManagement = 1 << 10
}

export const HasSystemAuthority = (
  authority: SystemAuthority,
  user?: User,
  boardKey?: string
): boolean => {
  if (!user) {
    return false;
  }
  if (
    (user.systemAuthority & SystemAuthority.Owner) ===
    SystemAuthority.Owner
  ) {
    return true;
  }
  return (user.systemAuthority & authority) === authority;
};

const HasBoardsManagementAuthority = (user: User) => {
  if (
    (user.systemAuthority & SystemAuthority.Owner) === SystemAuthority.Owner ||
    (user.systemAuthority & SystemAuthority.Admin) === SystemAuthority.Admin ||
    (user.systemAuthority & SystemAuthority.BoardsManagement) ===
      SystemAuthority.BoardsManagement
  ) {
    return true;
  } else {
    return false;
  }
};

export const HasViewResponseDetailAuthority = (
  boardKey: string,
  user?: User
): boolean => {
  if (user) {
    if (HasBoardsManagementAuthority(user)) {
      return true;
    }
    if (
      ((user.systemAuthority & SystemAuthority.BBSSetting) ===
        SystemAuthority.BBSSetting ||
        (user.systemAuthority & SystemAuthority.ViewResponseDetail) ===
          SystemAuthority.ViewResponseDetail) &&
      user.controllableBoards?.indexOf(boardKey)
    ) {
      return true;
    }
  }
  return false;
};

export const HasAboneResponseAuthority = (boardKey: string, user?: User) => {
  if (user) {
    if (HasBoardsManagementAuthority(user)) {
      return true;
    }
    if (
      ((user.systemAuthority & SystemAuthority.AboneResponse) ===
        SystemAuthority.AboneResponse ||
        (user.systemAuthority & SystemAuthority.RemoveResponse) ===
          SystemAuthority.RemoveResponse) &&
      user.controllableBoards?.indexOf(boardKey)
    ) {
      return true;
    }
  }
};

export const HasBoardSettingAuthority = (
  boardKey: string,
  user?: User
) => {
  if (user) {
    if (HasBoardsManagementAuthority(user)) {
      return true;
    }
    if (
      (user.systemAuthority & SystemAuthority.BBSSetting) ===
        SystemAuthority.BBSSetting &&
      user.controllableBoards?.indexOf(boardKey)
    ) {
      return true;
    }
    return false;
  }
};
