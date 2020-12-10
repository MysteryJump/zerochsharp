import React, { useState } from 'react';
import clsx from 'clsx';
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import { Route } from 'react-router-dom';
import { ConnectedRouter } from 'connected-react-router';
import { history, AppState } from '../store';
import { Signup } from './Signup';
import LeftDrawer from './Drawer';
import { useSelector } from 'react-redux';
import { SampleHomeContent } from './SampleHomeContent';
import { Login } from './Login';
import { ApplicationBar } from './ApplicationBar';
import { MainRoute } from './MainRoute';

export const drawerWidth = 280;

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: 'flex'
    },
    hide: {
      display: 'none'
    },
    drawerHeader: {
      display: 'flex',
      alignItems: 'center',
      padding: theme.spacing(0, 1),
      ...theme.mixins.toolbar,
      justifyContent: 'flex-end'
    },
    content: {
      flexGrow: 1,
      padding: theme.spacing(3),
      transition: theme.transitions.create('margin', {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen
      })
    },
    contentShift: {
      transition: theme.transitions.create('margin', {
        easing: theme.transitions.easing.easeOut,
        duration: theme.transitions.duration.enteringScreen
      }),
      marginLeft: 0
    },
    noneDisplayTitle: {
      [theme.breakpoints.up('sm')]: {
        display: 'none'
      }
    }
  })
);

interface Props {}

export const MainContent = (props: Props) => {
  const classes = useStyles();
  const [loginDialogOpen, setLoginDialogOpen] = useState(false);
  const [signupDialogOpen, setSignupDialogOpen] = useState(false);

  const drawerState = useSelector((appState: AppState) => appState.drawerState);
  let open = drawerState.isOpening;

  return (
    <ConnectedRouter history={history}>
      <div className={classes.root}>
        <CssBaseline />
        <ApplicationBar
          open={open ?? false}
          setLoginDialogOpen={setLoginDialogOpen}
          setSignupDialogOpen={setSignupDialogOpen}
        />
        <LeftDrawer />
        <main
          className={clsx(classes.content, {
            [classes.contentShift]: open
          })}
        >
          <div className={classes.drawerHeader} />
          <Route exact path="/" component={SampleHomeContent} />
          <Route path="/:boardKey" component={MainRoute} />
          <Login open={loginDialogOpen} setDialogStatus={setLoginDialogOpen} />
          <Signup
            open={signupDialogOpen}
            setDialogStatus={setSignupDialogOpen}
          />
        </main>
      </div>
    </ConnectedRouter>
  );
};
