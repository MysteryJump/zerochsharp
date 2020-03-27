import React from 'react';
import {
  Tooltip,
  Fab,
  makeStyles,
  createStyles,
  Theme
} from '@material-ui/core';
import EditIcon from '@material-ui/icons/Edit';

interface Props {
  isCreating: boolean;
  archived: boolean;
  stopped: boolean;
  setIsCreatingCallback: (value: React.SetStateAction<boolean>) => void;
}

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    createIconMargin: {
      position: 'fixed',
      bottom: theme.spacing(2),
      right: theme.spacing(2)
    }
  })
);

export const CreateResponseFab = (props: Props) => {
  const classes = useStyles();
  return (
    <div
      style={{
        display:
          props.isCreating || props.archived || props.stopped
            ? 'none'
            : 'initial'
      }}
    >
      <Tooltip title="Create" placement="top" TransitionProps={{ timeout: 0 }}>
        <Fab
          size="medium"
          color="primary"
          aria-label="create"
          className={classes.createIconMargin}
          onClick={() => props.setIsCreatingCallback(true)}
        >
          <EditIcon />
        </Fab>
      </Tooltip>
    </div>
  );
};
