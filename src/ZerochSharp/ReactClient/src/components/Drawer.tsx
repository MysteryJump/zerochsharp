import React from 'react';
import {
  makeStyles,
  useTheme,
  Theme,
  createStyles
} from '@material-ui/core/styles';
import Drawer from '@material-ui/core/Drawer';
import List from '@material-ui/core/List';
import Divider from '@material-ui/core/Divider';
import IconButton from '@material-ui/core/IconButton';
import ChevronLeftIcon from '@material-ui/icons/ChevronLeft';
import ChevronRightIcon from '@material-ui/icons/ChevronRight';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import InboxIcon from '@material-ui/icons/MoveToInbox';
import HomeIcon from '@material-ui/icons/Home';
import DeleteIcon from '@material-ui/icons/Delete';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom';
import { Hidden, ListItemSecondaryAction, Tooltip } from '@material-ui/core';
import {
  TabType,
  LeftDrawerTabItem,
  ThreadTabItem,
  BoardTabItem,
  HomeTabItem
} from '../states/tabState';
import { drawerWidth } from './MainContent';
import { HasSystemAuthority, SystemAuthority } from '../models/user';
import { useSelector, useDispatch } from 'react-redux';
import { AppState } from '../store';
import { tabActions } from '../actions/tabActions';
import { drawerActions } from '../actions/drawerAction';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: 'flex'
    },
    menuButton: {
      marginRight: theme.spacing(2)
    },
    hide: {
      display: 'none'
    },
    drawer: {
      [theme.breakpoints.up('sm')]: {
        width: drawerWidth,
        flexShrink: 0
      }
    },
    drawerPaper: {
      width: drawerWidth
    },
    drawerHeader: {
      display: 'flex',
      alignItems: 'center',
      padding: theme.spacing(0, 1),
      ...theme.mixins.toolbar,
      justifyContent: 'flex-end'
    },
    drawerTopIcon: {
      [theme.breakpoints.up('sm')]: {
        display: 'none'
      }
    }
  })
);

interface OwnProps {}

type Props = OwnProps;

export default function LeftDrawer(props: Props) {
  const classes = useStyles();
  const theme = useTheme();
  const drawerState = useSelector((appState: AppState) => appState.drawerState);
  const tabListState = useSelector((appState: AppState) => appState.tabState);
  const mainState = useSelector((appState: AppState) => appState.mainState);
  const sessionState = useSelector(
    (appState: AppState) => appState.sessionState
  );
  const router = useSelector((appState: AppState) => appState.router);
  const boardListState = useSelector(
    (appState: AppState) => appState.boardListState
  );
  const dispatch = useDispatch();

  const getTabLink = (tab: LeftDrawerTabItem) => {
    if (tab.tabType === TabType.Home) {
      return '/';
    } else {
      return tab.key;
    }
  };
  const showTabItems = (tabs: LeftDrawerTabItem[]) => {
    if (tabs.length === 0) {
      return (
        <ListItem key="nothing">
          <ListItemText primary="Nothing here." />
        </ListItem>
      );
    }
    return tabs.map((tab, index) => (
      <ListItem
        button
        key={tab.key}
        component={props => <Link to={getTabLink(tab)} {...props} />}
      >
        <ListItemIcon>
          {tab.tabType === TabType.Home ? <HomeIcon /> : <InboxIcon />}
        </ListItemIcon>
        <ListItemText primary={tab.name} secondary={tab.tabType} />
        <ListItemSecondaryAction>
          <Tooltip title="Remove">
            <IconButton
              edge="end"
              aria-label="delete"
              onClick={() => {
                dispatch(tabActions.removeTabItem({ item: tab }));
              }}
            >
              <DeleteIcon />
            </IconButton>
          </Tooltip>
        </ListItemSecondaryAction>
      </ListItem>
    ));
  };

  const handleAddTabClicked = () => {
    const path = router.location.pathname;
    const paths = path.split('/').filter(x => x !== '');
    let tabItem: LeftDrawerTabItem;
    if (paths.length === 2) {
      tabItem = {
        tabType: TabType.Thread,
        key: path,
        name: mainState.currentName,
        boardKey: paths[0],
        threadKey: paths[1],
        boardName: paths[0],
        id: tabListState.tabs.length + 1
      } as ThreadTabItem;
    } else if (paths.length === 1) {
      tabItem = {
        tabType: TabType.Board,
        key: path,
        name: mainState.currentName,
        boardKey: paths[0],
        id: tabListState.tabs.length + 1
      } as BoardTabItem;
    } else if (paths.length === 0) {
      tabItem = HomeTabItem;
    } else {
      return;
    }
    if (tabListState.tabs.findIndex(x => x.key === tabItem.key) < 0) {
      dispatch(tabActions.addTabItem({ item: tabItem }));
    }
  };

  const isAdmin = HasSystemAuthority(SystemAuthority.Admin, sessionState.user);
  const drawerAdminItems = isAdmin ? (
    <>
      <List>
        <ListItem>
          <ListItemText secondary="Admin" />
        </ListItem>
        <ListItem
          button
          component={props => <Link to={'/admin/general'} {...props}></Link>}
        >
          <ListItemText primary="General" />
        </ListItem>
        <ListItem
          button
          component={props => <Link to={'/admin/plugin'} {...props}></Link>}
        >
          <ListItemText primary="Plugins" />
        </ListItem>
        <ListItem button component={
          props => <Link to={'/admin/adminusers'} {...props}></Link>
        }>
          <ListItemText primary="Admin Users" />
        </ListItem>
        <ListItem
          button
          component={props => <Link to={'/admin/boards'} {...props}></Link>}
        >
          <ListItemText primary="Boards" />
        </ListItem>
      </List>
    </>
  ) : (
    <></>
  );
  const drawerInside = (
    <>
      <div className={classes.drawerHeader}>
        <IconButton
          onClick={() => dispatch(drawerActions.closeDrawer())}
          className={classes.drawerTopIcon}
        >
          {theme.direction === 'ltr' ? (
            <ChevronLeftIcon />
          ) : (
            <ChevronRightIcon />
          )}
        </IconButton>
      </div>
      <Divider />
      <List>
        <ListItem>
          <ListItemText secondary="Boards" />
        </ListItem>
        {boardListState.boards.map(board => (
          <ListItem
            button
            component={props => <Link to={'/' + board.boardKey} {...props} />}
          >
            <ListItemText primary={board.boardName} />
          </ListItem>
        ))}
      </List>
      <Divider />

      <List>
        <ListItem>
          <ListItemText secondary="Tabs" />
          <ListItemSecondaryAction>
            <Tooltip title="Add current content to tabs">
              <IconButton onClick={handleAddTabClicked}>
                <AddIcon />
              </IconButton>
            </Tooltip>
          </ListItemSecondaryAction>
        </ListItem>
        {showTabItems(tabListState.tabs)}
      </List>
      <Divider />
      <ListItem>
        <ListItemText secondary="Others" />
      </ListItem>
      <ListItem button key=""></ListItem>
      {sessionState.logined ? drawerAdminItems : <></>}
    </>
  );

  return (
    <>
      <nav className={classes.drawer}>
        <Hidden smUp implementation="css">
          <Drawer
            variant="temporary"
            anchor={theme.direction === 'rtl' ? 'right' : 'left'}
            open={drawerState.isOpening}
            onClose={() => dispatch(drawerActions.closeDrawer())}
            classes={{
              paper: classes.drawerPaper
            }}
            ModalProps={{
              keepMounted: true // Better open performance on mobile.
            }}
          >
            {drawerInside}
          </Drawer>
        </Hidden>
        <Hidden xsDown implementation="css">
          <Drawer
            classes={{
              paper: classes.drawerPaper
            }}
            variant="permanent"
            open
          >
            {drawerInside}
          </Drawer>
        </Hidden>
      </nav>
    </>
  );
}
