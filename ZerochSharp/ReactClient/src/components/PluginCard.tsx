import React, { useState } from 'react';
import clsx from 'clsx';
import { Plugin } from './Plugin';
import {
  CardActions,
  CardHeader,
  Card,
  CardContent,
  IconButton,
  Collapse,
  Typography,
  Theme,
  makeStyles,
  createStyles,
  Link,
  Tooltip,
  Switch,
  FormControlLabel
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import SettingsIcon from '@material-ui/icons/Settings';
import Axios from 'axios';

interface Props extends Plugin {
  isSelected?: boolean;
}

const useStyle = makeStyles((theme: Theme) =>
  createStyles({
    expand: {
      transform: 'rotate(0deg)',
      marginLeft: 'auto',
      transition: theme.transitions.create('transform', {
        duration: theme.transitions.duration.shortest
      })
    },
    expandOpen: {
      transform: 'rotate(180deg)'
    }
  })
);

export const PluginCard = (prop: Props) => {
  const [expanded, setExpanded] = useState(false);
  const [isEnabled, setIsEnabled] = useState(prop.isEnabled);
  const classes = useStyle();

  const changePluginEnableState = () => {
    Axios.patch<{ isEnable: boolean }>(`/api/plugin/${prop.pluginPath}`, {
      isEnable: !isEnabled
    })
      .then(x => {
        setIsEnabled(!isEnabled);
      })
      .catch(x => console.error(x));
  };

  return (
    <>
      <Card>
        <CardHeader
          title={prop.pluginName}
          subheader={`Path:${prop.pluginPath} Type:${prop.pluginType}`}
          style={{ paddingBottom: 8 }}
        />
        <CardActions>
          <FormControlLabel
            control={<Switch checked={isEnabled} />}
            label={isEnabled ? 'Enabled' : 'Disabled'}
            onChange={() => changePluginEnableState()}
            style={{ marginLeft: '0.2rem' }}
          />
          <Tooltip title="Plugin Setting" style={{ marginLeft: 'auto' }}>
            <IconButton>
              <SettingsIcon aria-label="plugin settings" />
            </IconButton>
          </Tooltip>
          <IconButton
            onClick={() => setExpanded(!expanded)}
            className={clsx(classes.expand, {
              [classes.expandOpen]: expanded
            })}
            aria-expanded={expanded}
            aria-label="show more"
          >
            <ExpandMoreIcon />
          </IconButton>
        </CardActions>
        <Collapse in={expanded} timeout="auto" unmountOnExit>
          <CardContent>
            <Typography>
              <h4>
                Author: {prop.author} {'    '}
                Official:{' '}
                <Link href={prop.officialSite}>{prop.officialSite}</Link>
              </h4>
              {prop.pluginDescription}
            </Typography>
          </CardContent>
        </Collapse>
      </Card>
    </>
  );
};
