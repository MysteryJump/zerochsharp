import React, { useEffect, useState } from 'react';
import Axios from 'axios';
import { PluginCard } from './PluginCard';
import { Plugin } from '../../../models/plugin';

const initialPlugin: Plugin[] = [];

export const Plugins = () => {
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
        <PluginCard {...x} key={x.pluginPath} />
      ))}
    </>
  );
  return (
    <>
      {pluginList}
    </>
  );
};
