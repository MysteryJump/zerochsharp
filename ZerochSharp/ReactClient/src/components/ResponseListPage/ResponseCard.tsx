import React, { useState, useEffect, useCallback } from 'react';
import { Response } from '../../models/response';
import {
  Card,
  CardContent,
  Theme,
  makeStyles,
  createStyles,
  Checkbox,
  Typography
} from '@material-ui/core';
import { User, HasViewResponseDetailAuthority } from '../../models/user';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    responseCard: {
      margin: '0.2rem'
    },
    checkBox: {
      marginLeft: '-0.65rem'
    },
    cardContent: {
      display: 'flex',
      flexDirection: 'row',
      justifyContent: 'flex-start',
      alignItems: 'stretch'
    }
  })
);

interface Props {
  index: number;
  response: Response;
  boardDefaultName: string;
  checked: boolean;
  checkedAction: (val: boolean) => void;
  display: boolean;
  user?: User;
  boardKey?: string;
}

export const ResponseCard = (props: Props) => {
  const classes = useStyles();
  const [checked, setChecked] = useState(props.checked);
  const action = props.checkedAction;
  const actionCallback = useCallback(x => action(x), [action]);

  const handleChangeChecked = (value: boolean) => {
    setChecked(value);
  };
  useEffect(() => {
    actionCallback(checked);
  }, [checked, actionCallback]);
  // const canShowResponseDetail = HasViewResponseDetailAuthority(props.response.);
  // const canAboneResponse 
  return (
    <Card className={classes.responseCard}>
      <CardContent>
        <div className={classes.cardContent}>
          <Checkbox
            className={classes.checkBox}
            value={checked}
            onChange={e => handleChangeChecked(e.target.checked)}
            style={{ display: props.display ? 'initial' : 'none' }}
          />
          <div style={{ flexGrow: 1 }}>
            <Typography>
              {props.index + 1}:{' '}
              <a href={props.response.mail}>
                {(() => {
                  if (
                    props.response.name === '' ||
                    props.response.name == null
                  ) {
                    return props.boardDefaultName;
                  } else {
                    return props.response.name;
                  }
                })()}
              </a>{' '}
              {new Date(Date.parse(props.response.created)).toLocaleString()}{' '}
              ID: {props.response.author}
            </Typography>
            <Typography>{props.response.body}</Typography>
          </div>
        </div>
      </CardContent>
      {/* <Divider />
      <CardActions>
        <Button>Edit This Response</Button>
      </CardActions> */}
    </Card>
  );
};
