import React, { useEffect, useState } from 'react';
import Axios from 'axios';
import { Card, CardActions, CardContent, CardHeader, IconButton } from '@material-ui/core';
import { PluginCard } from './PluginCard';


export interface Plugin {
  pluginName: string;
  pluginType: string;
  priority: number;
  pluginPath: string;
  isEnabled: boolean;
  pluginDescription: string;
  author: string;
  officialSite: string;
  activatedBoards: string[];
  globalSettings: any;
}

const initialPlugin: Plugin[] = [];

interface Props {}

export const Plugin = (props: Props) => {
  const [plugins, setPlugins] = useState(initialPlugin);
  
  useEffect(() => {
    Axios.get<Plugin[]>('/api/plugin').then(x => {
      setPlugins(x.data);
    });
  }, []);
  return (
    <>
      <h1>Hello, Plugin page!</h1>
      {plugins.map((x, index) => <PluginCard {...x} />)}
    </>
  );
};
