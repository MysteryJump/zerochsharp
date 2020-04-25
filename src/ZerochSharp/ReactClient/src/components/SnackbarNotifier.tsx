import React, { useEffect, useState } from 'react';
import { useSnackbar } from 'notistack';
import { useDispatch, useSelector } from 'react-redux';
import { AppState } from '../store';
import { SnackbarItem } from '../states/snackbarState';
import { snackbarActions } from '../actions/snackbarActions';
import { Button } from '@material-ui/core';

export const SnackbarNotifier = (props: { children?: JSX.Element }) => {
  const dispatch = useDispatch();
  const snackbarState = useSelector(
    (appState: AppState) => appState.snackbarState
  );
  const snackbar = useSnackbar();
  const [displayed, setDisplayed] = useState<SnackbarItem[]>([]);

  useEffect(() => {
    snackbarState.items.forEach((item) => {
      if (item.isDismissed) {
        snackbar.closeSnackbar(item.key);
        setDisplayed(displayed.filter((x) => x.key !== item.key));
        dispatch(snackbarActions.removeSnackbar({ key: item.key }));
      } else {
        if (displayed.findIndex((x) => x.key === item.key) < 0) {
          setDisplayed([...displayed, item]);
          snackbar.enqueueSnackbar(item.text, {
            ...item.options,
            key: item.key,
          });
        }
      }
    });
  }, [dispatch, displayed, snackbarState, snackbar]);
  return <>{props.children}</>;
};

export const SnackbarItemActionButton = (props: { key: string }) => {
  const dispatch = useDispatch();

  return (
    <Button
      onClick={() =>
        dispatch(snackbarActions.dismissSnackbar({ key: props.key }))
      }
    >
      Dismiss
    </Button>
  );
};
