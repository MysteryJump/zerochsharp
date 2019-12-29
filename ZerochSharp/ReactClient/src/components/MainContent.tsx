import React, { useState } from 'react';
import clsx from 'clsx';
import {
  makeStyles,
  Theme,
  createStyles
} from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';
import Typography from '@material-ui/core/Typography';
import IconButton from '@material-ui/core/IconButton';
import MenuIcon from '@material-ui/icons/Menu';
import DrawerContainer from '../containers/DrawerContainer';
import { DrawerState } from '../states/drawerState';
import { MainContentActions } from '../containers/MainContentContainer';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { useCookies } from 'react-cookie';
import { ConnectedRouter } from 'connected-react-router';
import { history } from '../store';
import ThreadListContainer from '../containers/ThreadListContainer';
import { Button } from '@material-ui/core';
import SettingsIcon from '@material-ui/icons/Settings';
import AccountCircleIcon from '@material-ui/icons/AccountCircle';
import ResponseListContainer from '../containers/ResponseListContainer';
import { MainState } from '../states/mainState';
import LoginContainer from '../containers/LoginContainer';
import { SessionState, Authority } from '../states/sessionState';
import { Admin } from './Admin';
import { Plugin } from '../components/Plugin';
import { PluginDetail } from '../components/PluginDetail';
import { Signup } from './Signup';

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
          <Route exact path={`${props.match.path}/plugin`} component={Plugin} />
          <Route path={`/admin/plugin/:pluginName`} component={PluginDetail} />
        </Switch>
      </>
    );
  }
  return (
    <Switch>
      <Route
        exact
        path={`${props.match.path}/`}
        component={ThreadListContainer}
      />
      <Route
        path={`${props.match.path}/:threadId`}
        component={ResponseListContainer}
      />
    </Switch>
  );
};

interface OwnProps {}

type Props = OwnProps &
  (DrawerState & MainState & SessionState) &
  MainContentActions;

export const MainContent = (props: Props) => {
  const classes = useStyles();
  const [loginDialogOpen, setLoginDialogOpen] = useState(false);
  const [signupDialogOpen, setSignupDialogOpen] = useState(false);
  const [, , removeSessionCookie] = useCookies(['.session.main']);
  let open = props.isOpening;

  const sampleMainContent = () => {
    props.setCurrentName('Home');

    return (
      <>
        <Typography paragraph>
          Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do
          eiusmod tempor incididunt ut labore et dolore magna aliqua. Rhoncus
          dolor purus non enim praesent elementum facilisis leo vel. Risus at
          ultrices mi tempus imperdiet. Semper risus in hendrerit gravida rutrum
          quisque non tellus. Convallis convallis tellus id interdum velit
          laoreet id donec ultrices. Odio morbi quis commodo odio aenean sed
          adipiscing. Amet nisl suscipit adipiscing bibendum est ultricies
          integer quis. Cursus euismod quis viverra nibh cras. Metus vulputate
          eu scelerisque felis imperdiet proin fermentum leo. Mauris commodo
          quis imperdiet massa tincidunt. Cras tincidunt lobortis feugiat
          vivamus at augue. At augue eget arcu dictum varius duis at consectetur
          lorem. Velit sed ullamcorper morbi tincidunt. Lorem donec massa sapien
          faucibus et molestie ac.
        </Typography>
        <Typography paragraph>
          Consequat mauris nunc congue nisi vitae suscipit. Fringilla est
          ullamcorper eget nulla facilisi etiam dignissim diam. Pulvinar
          elementum integer enim neque volutpat ac tincidunt. Ornare suspendisse
          sed nisi lacus sed viverra tellus. Purus sit amet volutpat consequat
          mauris. Elementum eu facilisis sed odio morbi. Euismod lacinia at quis
          risus sed vulputate odio. Morbi tincidunt ornare massa eget egestas
          purus viverra accumsan in. In hendrerit gravida rutrum quisque non
          tellus orci ac. Pellentesque nec nam aliquam sem et tortor. Habitant
          morbi tristique senectus et. Adipiscing elit duis tristique
          sollicitudin nibh sit. Ornare aenean euismod elementum nisi quis
          eleifend. Commodo viverra maecenas accumsan lacus vel facilisis. Nulla
          posuere sollicitudin aliquam ultrices sagittis orci a.
        </Typography>
      </>
    );
  };

  const isLogined =
    props.user === undefined ||
    props.sesssionToken === '' ||
    props.sesssionToken === undefined;
  const isAdmin =
    !isLogined &&
    (props.user !== undefined ? props.user.authority & Authority.Admin : false);
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
              onClick={props.handleDrawerOpen}
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
                props.sessionLogout();
                removeSessionCookie('.session.main');
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
        <DrawerContainer />
        <main
          className={clsx(classes.content, {
            [classes.contentShift]: open
          })}
        >
          <div className={classes.drawerHeader} />
          <Route exact path="/" component={sampleMainContent} />
          <Route path="/:boardKey" component={MainViewArea} />
          <LoginContainer
            open={loginDialogOpen}
            setDialogStatus={setLoginDialogOpen}
          />
          <Signup open={signupDialogOpen} setDialogStatus={setSignupDialogOpen}/>
        </main>
      </div>
    </ConnectedRouter>
  );
};
