import React, { useState } from 'react';
import clsx from 'clsx';
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
// import { useCookies } from 'react-cookie';
import { ConnectedRouter } from 'connected-react-router';
import { history, AppState } from '../store';
import { Button } from '@material-ui/core';
import SettingsIcon from '@material-ui/icons/Settings';
import AccountCircleIcon from '@material-ui/icons/AccountCircle';
import { Authority } from '../models/user';
import { Admin } from './Admin';
import { Plugins } from './AdminArea/PluginPages/Plugins';
import { PluginDetail } from './AdminArea/PluginPages/PluginDetail';
import { Signup } from './Signup';
import LeftDrawer from './Drawer';
import { useSelector, useDispatch } from 'react-redux';
import { SampleHomeContent } from './SampleHomeContent';
import { drawerActions } from '../actions/drawerAction';
import { sessionActions } from '../actions/sessionActions';
import { Login } from './Login';
import { ThreadList } from './ThreadListPage/ThreadList';
import { ResponseList } from './ResponseListPage/ResponseList';
import { BoardSetting } from './BoardSetting';

export const drawerWidth = 280;

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: 'flex'
    },
    appBar: {
      transition: theme.transitions.create(['margin', 'width'], {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen
      })
    },
    appBarShift: {
      width: `calc(100% - ${drawerWidth}px)`,
      marginLeft: drawerWidth,
      transition: theme.transitions.create(['margin', 'width'], {
        easing: theme.transitions.easing.easeOut,
        duration: theme.transitions.duration.enteringScreen
      })
    },
    menuButton: {
      marginRight: theme.spacing(2),
      [theme.breakpoints.up('sm')]: {
        display: 'none'
      }
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
    appTitle: {
      [theme.breakpoints.down('sm')]: {
        display: 'none'
      },
      marginLeft: drawerWidth
    },
    noneDisplayTitle: {
      [theme.breakpoints.up('sm')]: {
        display: 'none'
      }
    }
  })
);
interface MainViewAreaProps extends RouteComponentProps<{ boardKey: string }> {}
const MainViewArea = (props: MainViewAreaProps) => {
  const boardKey = props.match.params.boardKey;
  if (boardKey === 'admin') {
    return (
      <>
        <Switch>
          <Route exact path={`${props.match.path}`} component={Admin} />
          <Route
            exact
            path={`${props.match.path}/plugin`}
            component={Plugins}
          />
          <Route path={`/admin/plugin/:pluginName`} component={PluginDetail} />
        </Switch>
      </>
    );
  }
  return (
    <Switch>
      <Route exact path={`${props.match.path}/`} component={ThreadList} />
      <Route
        exact
        path={`${props.match.path}/setting`}
        component={BoardSetting}
      />
      <Route path={`${props.match.path}/:threadId`} component={ResponseList} />
    </Switch>
  );
};

interface OwnProps {}

type Props = OwnProps;

export const MainContent = (props: Props) => {
  const classes = useStyles();
  const [loginDialogOpen, setLoginDialogOpen] = useState(false);
  const [signupDialogOpen, setSignupDialogOpen] = useState(false);

  const dispatch = useDispatch();
  const sessionState = useSelector(
    (appState: AppState) => appState.sessionState
  );
  const drawerState = useSelector((appState: AppState) => appState.drawerState);
  let open = drawerState.isOpening;

  const isLogined =
    sessionState.user === undefined ||
    sessionState.sesssionToken === '' ||
    sessionState.sesssionToken === undefined;
  const isAdmin =
    !isLogined &&
    (sessionState.user !== undefined
      ? sessionState.user.authority & Authority.Admin
      : false);
  const loginStatusStyle = {
    display: isLogined ? 'initial' : 'none'
  };
  const notLoginStatusStyle = {
    display: isLogined ? 'none' : 'initial'
  };
  const isAdminStatusStyle = {
    display: isAdmin ? 'initial' : 'none'
  };

  return (
    <ConnectedRouter history={history}>
      <div className={classes.root}>
        <CssBaseline />
        <AppBar
          position="fixed"
          className={clsx(classes.appBar, {
            [classes.appBarShift]: open
          })}
        >
          <Toolbar>
            <IconButton
              color="inherit"
              aria-label="open drawer"
              onClick={() => dispatch(drawerActions.openDrawer())}
              edge="start"
              className={clsx(classes.menuButton, open && classes.hide)}
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
            <Button
              color="inherit"
              onClick={() => setLoginDialogOpen(true)}
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
              onClick={() => setSignupDialogOpen(true)}
            >
              Signup
            </Button>
            <IconButton color="inherit" style={isAdminStatusStyle}>
              <SettingsIcon />
            </IconButton>
            <IconButton color="inherit">
              <AccountCircleIcon />
            </IconButton>
          </Toolbar>
        </AppBar>
        <LeftDrawer />
        <main
          className={clsx(classes.content, {
            [classes.contentShift]: open
          })}
        >
          <div className={classes.drawerHeader} />
          <Route exact path="/" component={SampleHomeContent} />
          <Route path="/:boardKey" component={MainViewArea} />
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
