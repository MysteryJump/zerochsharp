import React from 'react';
import {
  Typography,
  Box,
  Theme,
  makeStyles,
  AppBar,
  Tab
} from '@material-ui/core';

interface Props {
  children?: React.ReactNode;
  index: any;
  value: any;
}

const BoardSettingTabs = (props: Props) => {
  const { children, value, index, ...other } = props;
  return (
    <Typography
      component="div"
      role="tabpanel"
      hidden={value !== index}
      id={`wrapped-admin-tabpanel-${index}`}
      aria-labelledby={`wrapped-admin-tab-${index}`}
      {...other}
    >
      {value === index && <Box p={3}>{children}</Box>}
    </Typography>
  );
};

const a11yProps = (index: any) => {
  return { id: `wrapped-admin-tab-${index}` };
};

const useStyle = makeStyles((theme: Theme) => ({
  root: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.paper
  }
}));

export const BoardSetting = (props: Props) => {
  const classes = useStyle();
  return (
    <>
      <div>
        <p>Hello, Board Setting Pages</p>
        <div className={classes.root}>
          <AppBar position="static" aria-label="wrapped label tabs admin">
            <Tab value="one" label="Plugins" />
          </AppBar>
        </div>
      </div>
    </>
  );
};
