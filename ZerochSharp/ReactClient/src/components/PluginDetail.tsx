import React from 'react';
import { Plugin } from './Plugin';

interface Props extends Plugin {}

export const PluginDetail = (props: Props) => {
  return (
    <>
      <p>Hello, PluginDetail page!</p>
    </>
  );
};
