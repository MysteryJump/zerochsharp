import React from 'react';
import { useSelector } from 'react-redux';
import { AppState } from '../../store';
import { List, ListItem, Divider, Chip } from '@material-ui/core';
import { HasSystemAuthority, SystemAuthority } from '../../models/user';

export const MyPage = () => {
  const user = useSelector((state: AppState) => state.sessionState);
  // show authorities: Owner, Admin as SystemAdmin, Limited Admin
  let authorityChipTxt = '';
  if (HasSystemAuthority(SystemAuthority.Owner, user.user)) {
    authorityChipTxt = 'Owner';
  } else if (HasSystemAuthority(SystemAuthority.Admin, user.user)) {
    authorityChipTxt = 'Admin';
  } else if (user.user?.systemAuthority !== 0) {
    authorityChipTxt = 'Limited Admin';
  }
  return (
    <div>
      <h1>
        Hello, {user.user?.userId}! {'  '}
        {authorityChipTxt !== '' ? <Chip label={authorityChipTxt} /> : <></>}
      </h1>
      <List>
        <ListItem>E-mail:</ListItem>
        <Divider />
        <ListItem>Password:</ListItem>
        <Divider />
      </List>
    </div>
  );
};
