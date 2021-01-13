import React, { useCallback, useState } from 'react';
import axios from 'axios';
import { AdminUser, GetSystemAuthorityString } from '../../../models/user';
import { Button, List, ListItem, ListItemSecondaryAction, ListItemText } from '@material-ui/core';
import { useEffect } from 'react';
import { useHistory } from 'react-router-dom';

export const AdminUsers = () => {
  const [users, setUsers] = useState<AdminUser[]>([]);
  const history = useHistory();
  const getAdminUser = useCallback(async () => {
    try {
      const response = await axios.get<AdminUser[]>(`/api/users?isAdmin=true`);
      setUsers(
        response.data.map((x, i) => {
          return { ...x, id: i + 1 };
        })
      );
    } catch (e) {
      console.log(e);
    }
  }, []);

  useEffect(() => {
    getAdminUser();
  }, [getAdminUser]);
  return (
    <div>
      <h1>Hello, Admin user page!</h1>
      <List>
        {users.map((x) => (
          <ListItem>
            <ListItemText
              primary={x.userId}
              secondary={GetSystemAuthorityString(x.systemAuthority)}
            />
            <ListItemSecondaryAction>
              <Button onClick={() => {
                history.push(`/user/${x.userId}/`);
              }}>
                Detail
              </Button>
            </ListItemSecondaryAction>
          </ListItem>
        ))}
      </List>
    </div>
  );
};
