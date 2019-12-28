import React, { useEffect, useState } from 'react';
import Axios from 'axios';
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

export const Plugin = () => {
  const [plugins, setPlugins] = useState(initialPlugin);

  useEffect(() => {
    Axios.get<Plugin[]>('/api/plugin').then(x => {
      setPlugins(x.data);
    });
  }, []);
  const pluginList = (
    <>
      <h1>Plugin List</h1>
      {plugins.map(x => (
        <PluginCard {...x} />
      ))}
    </>
  );
  return (
    <>
      {pluginList}
    </>
  );
};
