import React, { useEffect, useState } from 'react';
import Axios from 'axios';
import { PluginCard } from './PluginCard';

export interface Plugin {
  pluginName: string;
  pluginType: number;
  priority: number;
  pluginPath: string;
  isEnabled: boolean;
  pluginDescription: string;
  author: string;
  officialSite: string;
  activatedBoards: string[];
  globalSettings: any;
}

export enum PluginTypes {
  Response = 1 << 0,
  Thread = 1 << 1
}

export const pluginTypesToString = (pluginType : PluginTypes) => {
  let str = "";
  if ((pluginType & PluginTypes.Response) === PluginTypes.Response) {
    str += "Response, ";
  } 
  if ((pluginType & PluginTypes.Thread) === PluginTypes.Thread) {
    str += "Thread, ";
  }
  return str.substr(0, str.length - 2);
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
