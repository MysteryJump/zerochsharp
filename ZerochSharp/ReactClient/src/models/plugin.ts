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
