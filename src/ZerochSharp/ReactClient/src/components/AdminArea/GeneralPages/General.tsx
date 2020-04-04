import React, { useCallback, useState, useEffect } from 'react';
import Axios from 'axios';
import { TextField, Button } from '@material-ui/core';

export const General = () => {
  const [siteName, setSiteName] = useState('' as string | undefined);
  const [initialSiteName, setInitialSiteName] = useState(
    '' as string | undefined
  );

  const getPluginCallback = useCallback(() => {
    Axios.get<SiteSetting>('/api/global')
      .then(x => {
        setSiteName(x.data.siteName);
        setInitialSiteName(x.data.siteName);
      })
      .catch(e => console.error(e));
  }, []);

  const isChangedCallback = useCallback(() => {
    if (siteName === initialSiteName) {
      return true;
    }
    return false;
  }, [initialSiteName, siteName]);
  const patchPluginChangeCallback = useCallback(() => {
    if (isChangedCallback()) {
      Axios.patch<SiteSetting>('/api/global', { siteName: siteName })
        .then(x => {})
        .catch(e => console.error(e));
    }
  }, [isChangedCallback, siteName]);
  useEffect(() => {
    getPluginCallback();
  }, [getPluginCallback]);
  return (
    <>
      <h1>General Settings</h1>
      <div>
        <TextField
          value={siteName}
          label="Site Name"
          onChange={x => setSiteName(x.target.value)}
          style={{ margin: '0.5rem' }}
        />
      </div>

      <Button
        onClick={() => patchPluginChangeCallback()}
        disabled={isChangedCallback()}
      >
        Save
      </Button>
    </>
  );
};

interface SiteSetting {
  siteName?: string;
}
