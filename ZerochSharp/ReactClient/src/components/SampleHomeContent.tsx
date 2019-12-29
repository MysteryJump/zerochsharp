import React, { useEffect } from 'react';
import {
  Typography,
  Divider,
  makeStyles,
  createStyles,
  Theme,
  Link
} from '@material-ui/core';
import { useDispatch } from 'react-redux';
import { mainActions } from '../actions/mainActions';

const useStyle = makeStyles((theme: Theme) =>
  createStyles({
    dividerStyle: {
      marginLeft: theme.spacing(-1.5),
      marginRight: theme.spacing(-1.5),
      marginBottom: theme.spacing(1.5)
    }
  })
);

export const SampleHomeContent = () => {
  const dispatch = useDispatch();
  const classes = useStyle();
  useEffect(() => {
    dispatch(mainActions.replaceCurrentName({ name: 'Home' }));
  }, [dispatch]);
  return (
    <>
      <Typography variant="h2">Welcome to Zeroch Sharp Project!</Typography>
      <Typography variant="h5" gutterBottom>
        The purpose of the "Zeroch Sharp" project is simple, it will be the
        successor of 0ch+ project.
      </Typography>
      <Divider className={classes.dividerStyle} />
      <Typography variant="h4" gutterBottom>
        Concept
      </Typography>
      <Typography paragraph>
        <Link href="https://zerochplus.osdn.jp/">
          Zeroch Plus (ぜろちゃんねるプラス)
        </Link>{' '}
        is used by many users who want to run own BBS easily. However Zeroch plus is
        already the end of life (maybe). So we will develop the successor of
        Zeroch Plus to replace the BBS system of{' '}
        <Link href="https://7gon.net">Septagon</Link>. (if possible)
      </Typography>
    </>
  );
};
