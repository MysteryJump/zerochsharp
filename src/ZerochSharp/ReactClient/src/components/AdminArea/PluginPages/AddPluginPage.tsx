import React, { useState } from 'react';
import {
  makeStyles,
  createStyles,
  Theme,
  Button,
  CircularProgress
} from '@material-ui/core';
import axios, { AxiosRequestConfig } from 'axios';
import { useHistory } from 'react-router-dom';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      display: 'flex',
      flexWrap: 'wrap',
      flexDirection: 'column'
    },
    upload: {
      display: 'flex',
      flexDirection: 'row'
    }
  })
);

export const AddPluginPage = () => {
  const classes = useStyles();
  const [zippedFile, setZippedFile] = useState<File>();
  const [isUploading, setIsUploading] = useState(false);
  const history = useHistory();
  const uploadFile = () => {
    if (!zippedFile) {
      return;
    }

    const formData = new FormData();
    formData.append('file', zippedFile);
    const config: AxiosRequestConfig = {
      headers: {
        'Content-type': 'multipart/form-data'
      }
    };
    setIsUploading(true);
    axios
      .post('/api/plugin/', formData, config)
      .then(x => {
        history.push('/admin/plugin');
      })
      .catch(e => {
        console.error(e);
        setIsUploading(false);
      });
  };

  return (
    <div className={classes.root}>
      <h1>Add Plugin</h1>
      <div>
        <input
          style={{ display: 'none' }}
          type="file"
          accept="application/zip"
          id="contained-button-file-zip"
          name="zipped-file"
          onChange={e => setZippedFile(e.target.files?.[0])}
        />
        <label htmlFor="contained-button-file-zip">
          <Button variant="contained" color="primary" component="span">
            Select File
          </Button>
        </label>
        {zippedFile ? <p>{zippedFile.name}</p> : <p></p>}
        <Button onClick={uploadFile} disabled={isUploading}>
          Upload
        </Button>
        {isUploading ? <CircularProgress /> : <></>}
      </div>
    </div>
  );
};
