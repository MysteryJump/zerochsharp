import React, { useState } from 'react';
import clsx from 'clsx';
import { makeStyles, Theme, createStyles } from '@material-ui/core/styles';
import CssBaseline from '@material-ui/core/CssBaseline';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { ConnectedRouter } from 'connected-react-router';
import { history, AppState } from '../store';
import { Admin } from './Admin';
import { Plugins } from './AdminArea/PluginPages/Plugins';
import { PluginDetail } from './AdminArea/PluginPages/PluginDetail';
import { Signup } from './Signup';
import LeftDrawer from './Drawer';
import { useSelector } from 'react-redux';
import { SampleHomeContent } from './SampleHomeContent';
import { Login } from './Login';
import { ThreadList } from './ThreadListPage/ThreadList';
import { ResponseList } from './ResponseListPage/ResponseList';
import { BoardSetting } from './BoardSetting';
import { Boards } from './AdminArea/BoardsPages/Boards';
import { BoardPluginSetting } from './BoardPluginSetting';
import { AdminUsers } from './AdminArea/AdminUserPages/AdminUsers';
import { General } from './AdminArea/GeneralPages/General';
import { ArchivedThreadList } from './ThreadListPage/ArchivedThreadList';
import { ApplicationBar } from './ApplicationBar';
import { AddPluginPage } from './AdminArea/PluginPages/AddPluginPage';

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

interface MainViewAreaProps extends RouteComponentProps<{ boardKey: string }> {}
const MainViewArea = (props: MainViewAreaProps) => {
  const boardKey = props.match.params.boardKey;
  if (boardKey === 'admin') {
    return (
      <>
        <Switch>
          <Route exact path={`${props.match.path}`} component={Admin} />
          <Route exact path={`${props.match.path}/boards`} component={Boards} />
          <Route
            exact
            path={`${props.match.path}/plugin`}
            component={Plugins}
          />
          <Route exact path={`/admin/plugin/add`} component={AddPluginPage} />
          <Route path={`/admin/plugin/:pluginPath`} component={PluginDetail} />
          <Route exact path={`/admin/adminusers/`} component={AdminUsers} />
          <Route exact path={`/admin/general/`} component={General} />
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
      <Route
        exact
        path={`${props.match.path}/setting/plugin/:pluginPath`}
        component={BoardPluginSetting}
      />
      <Route
        exact
        path={`${props.match.path}/archive`}
        component={ArchivedThreadList}
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
