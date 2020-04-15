import React from 'react';
import {
  AppBar,
  IconButton,
  Typography,
  Toolbar,
  InputBase,
  Button,
  Theme,
  makeStyles,
  createStyles,
  fade,
} from '@material-ui/core';
import SearchIcon from '@material-ui/icons/Search';
import MenuIcon from '@material-ui/icons/Menu';
import AccountCircleIcon from '@material-ui/icons/AccountCircle';
import { drawerWidth } from './MainContent';
import clsx from 'clsx';
import { useDispatch, useSelector } from 'react-redux';
import { sessionActions } from '../actions/sessionActions';
import { drawerActions } from '../actions/drawerAction';
import { AppState } from '../store';
import { routerActions } from 'connected-react-router';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    appBar: {
      transition: theme.transitions.create(['margin', 'width'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
      }),
    },
    appBarShift: {
      width: `calc(100% - ${drawerWidth}px)`,
      marginLeft: drawerWidth,
      transition: theme.transitions.create(['margin', 'width'], {
        easing: theme.transitions.easing.easeOut,
        duration: theme.transitions.duration.enteringScreen,
      }),
    },
    menuButton: {
      marginRight: theme.spacing(2),
      [theme.breakpoints.up('sm')]: {
        display: 'none',
      },
    },
    hide: {
      display: 'none',
    },
    appTitle: {
      [theme.breakpoints.down('sm')]: {
        display: 'none',
      },
      marginLeft: drawerWidth,
    },
    searchIcon: {
      width: theme.spacing(7),
      height: '100%',
      position: 'absolute',
      pointerEvents: 'none',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
    },
    inputRoot: {
      color: 'inherit',
    },
    inputInput: {
      padding: theme.spacing(1, 1, 1, 7),
      transition: theme.transitions.create('width'),
      width: '100%',
      [theme.breakpoints.up('sm')]: {
        width: 120,
        '&:focus': {
          width: 200,
        },
      },
    },
    search: {
      position: 'relative',
      borderRadius: theme.shape.borderRadius,
      backgroundColor: fade(theme.palette.common.white, 0.15),
      '&:hover': {
        backgroundColor: fade(theme.palette.common.white, 0.25),
      },
      marginLeft: 0,
      width: '100%',
      [theme.breakpoints.up('sm')]: {
        marginLeft: theme.spacing(1),
        width: 'auto',
      },
      marginRight: '1rem',
    },
  })
);

interface Props {
  open: boolean;
  setLoginDialogOpen: (value: boolean) => void;
  setSignupDialogOpen: (value: boolean) => void;
}

export const ApplicationBar = (props: Props) => {
  const classes = useStyles();
  const dispatch = useDispatch();

  const sessionState = useSelector(
    (appState: AppState) => appState.sessionState
  );

  const isLogined =
    sessionState.user === undefined ||
    sessionState.sesssionToken === '' ||
    sessionState.sesssionToken === undefined;
  const loginStatusStyle = {
    display: isLogined ? 'initial' : 'none',
  };
  const notLoginStatusStyle = {
    display: isLogined ? 'none' : 'initial',
  };

  return (
    <AppBar
      position="fixed"
      className={clsx(classes.appBar, {
        [classes.appBarShift]: props.open,
      })}
    >
      <Toolbar>
        <IconButton
          color="inherit"
          aria-label="open drawer"
          onClick={() => dispatch(drawerActions.openDrawer())}
          edge="start"
          className={clsx(classes.menuButton, props.open && classes.hide)}
        >
          <MenuIcon />
        </IconButton>
        <Typography
          variant="h6"
          noWrap
          style={{ flexGrow: 1 }}
          className={classes.appTitle}
        >
          Zeroch Sharp Client
        </Typography>
        <div className={classes.search}>
          <div className={classes.searchIcon}>
            <SearchIcon />
          </div>
          <InputBase
            placeholder="Search..."
            inputProps={{ 'aria-label': 'search' }}
            classes={{
              root: classes.inputRoot,
              input: classes.inputInput,
            }}
          />
        </div>
        <Button
          color="inherit"
          onClick={() => props.setLoginDialogOpen(true)}
          style={loginStatusStyle}
        >
          Login
        </Button>
        <Button
          color="inherit"
          style={notLoginStatusStyle}
          onClick={() => {
            dispatch(sessionActions.logoutSession());
          }}
        >
          Logout
        </Button>
        <Button
          color="inherit"
          style={loginStatusStyle}
          onClick={() => props.setSignupDialogOpen(true)}
        >
          Signup
        </Button>
        <IconButton
          color="inherit"
          onClick={() => dispatch(routerActions.push('/user'))}
          style={notLoginStatusStyle}
        >
          <AccountCircleIcon />
        </IconButton>
      </Toolbar>
    </AppBar>
  );
};
