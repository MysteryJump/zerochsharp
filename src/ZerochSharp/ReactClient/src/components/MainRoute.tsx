import React from 'react';
import { Route, RouteComponentProps, Switch } from 'react-router-dom';
import { BoardPluginSetting } from './BoardPluginSetting';
import { BoardSetting } from './BoardSettingPage/BoardSetting';
import { ThreadList } from './ThreadListPage/ThreadList';
import { AdminUsers } from './AdminArea/AdminUserPages/AdminUsers';
import { PluginDetail } from './AdminArea/PluginPages/PluginDetail';
import { AddPluginPage } from './AdminArea/PluginPages/AddPluginPage';
import { General } from './AdminArea/GeneralPages/General';
import { Plugins } from './AdminArea/PluginPages/Plugins';
import { Admin } from './Admin';
import { Boards } from './AdminArea/BoardsPages/Boards';
import { ArchivedThreadList } from './ThreadListPage/ArchivedThreadList';
import { ResponseList } from './ResponseListPage/ResponseList';
import { MyPage } from './UserArea/MyPage';
import { UserPage } from './UserArea/UserPage';

export const MainRoute = (props: RouteComponentProps<{ boardKey: string }>) => {
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
  } else if (boardKey === 'user') {
    return (
      <>
        <Switch>
          <Route path={`${props.match.path}/:userId`} component={UserPage} />
          <Route exact path={`${props.match.path}/`} component={MyPage} />
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
