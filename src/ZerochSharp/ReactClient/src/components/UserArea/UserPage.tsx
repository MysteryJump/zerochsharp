import React, { useCallback, useState, useEffect } from 'react';
import { useHistory, RouteComponentProps } from 'react-router-dom';
import { useSelector } from 'react-redux';
import { AppState } from '../../store';
import { Chip } from '@material-ui/core';
import { SystemAuthority, HasSystemAuthority, User } from '../../models/user';
import axios from 'axios';

export const UserPage = (props: RouteComponentProps<{ userId?: string }>) => {
  const myUser = useSelector((state: AppState) => state.sessionState);
  const history = useHistory();
  const [user, setUser] = useState<User | undefined>();
  if (
    myUser.user?.userId === props.match.params.userId &&
    myUser.user?.userId !== undefined
  ) {
    history.push('/user');
  }

  const getUserInfo = useCallback(() => {
    axios
      .get<User>(`/api/users/${props.match.params.userId}`)
      .then((x) => {
        setUser(x.data);
      })
      .catch((e) => console.error(e));
  }, [props.match.params.userId]);

  useEffect(() => getUserInfo(), [getUserInfo]);

  let authorityChipTxt = '';
  if (HasSystemAuthority(SystemAuthority.Owner, user)) {
    authorityChipTxt = 'Owner';
  } else if (HasSystemAuthority(SystemAuthority.Admin, user)) {
    authorityChipTxt = 'Admin';
  } else if ((HasSystemAuthority((1 << 11) - 1), user)) {
    authorityChipTxt = 'Limited Admin';
  }
  return (
    <div>
      <h1>
        {user?.userId} {'  '}
        {authorityChipTxt !== '' ? <Chip label={authorityChipTxt} /> : <></>}
      </h1>
    </div>
  );
};
