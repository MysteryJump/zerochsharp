import React from 'react';
import { Tooltip, Fab, makeStyles, createStyles, Theme } from '@material-ui/core';
import DeleteIcon from '@material-ui/icons/Delete';

interface Props {
  isAdmin?: boolean;
  checkedLength: number;
  setRemoveResponseDialogOpenCallback: (
    value: React.SetStateAction<boolean>
  ) => void;
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    removeIconMargin: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(10)
    }
  })
);

export const RemoveResponseFab = (props: Props) => {
  const classes = useStyles();
  return (
    <div
      style={{
        display: props.checkedLength > 0 && props.isAdmin ? 'initial' : 'none'
      }}
    >
      <Tooltip
        title="Remove Responses"
        placement="top"
        TransitionProps={{ timeout: 0 }}
      >
        <Fab
          size="medium"
          color="primary"
          aria-label="remote response"
          className={classes.removeIconMargin}
          onClick={() => props.setRemoveResponseDialogOpenCallback(true)}
        >
          <DeleteIcon />
        </Fab>
      </Tooltip>
    </div>
  );
};
