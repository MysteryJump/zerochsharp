import React, { useState, useEffect } from 'react';
import {
  TextField,
  makeStyles,
  Theme,
  createStyles,
  Grid,
  IconButton,
  useTheme,
  Tooltip,
  Select,
  MenuItem
} from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import DeleteIcon from '@material-ui/icons/Delete';

const useStyles = makeStyles((theme: Theme) =>
  createStyles({
    root: {
      textAlign: 'center'
      // marginTop: '3.5rem'
    },
    arrayItems: {
      display: 'flex',
      flexDirection: 'row'
    },
    arrayItemsInner: {
      display: 'flex',
      flexDirection: 'column',
      paddingLeft: '1.5rem',
      textAlign: 'left'
    },
    keyValuePairItem: {
      display: 'flex',
      flexDirection: 'row',
      margin: '0.3rem',
      textAlign: 'left',
      paddingLeft: '1.5rem'
    }
  })
);

// OOP的にはItemを継承するべきでは？
// 直和型最高では？
export interface ParentItem {
  children: (KeyValueChildItem | ArrayChildItem)[];
  name: string;
  key: string;
}

export interface KeyValueChildItem {
  name: string;
  value: string | boolean | number;
  key: string;
}

export interface ArrayChildItem {
  name: string;
  value: string[];
  key: string;
}

const isKeyValueChildItem = (
  item: KeyValueChildItem | ArrayChildItem
): item is KeyValueChildItem => {
  return typeof item.value !== 'object';
};

interface Props {
  item: ParentItem;
  onChange?: (tree: ParentItem) => void;
}

export const TreeTextField: React.FC<Props> = (props: Props) => {
  const theme = useTheme();
  const classes = useStyles();

  const [tree, setTree] = useState(props.item);
  useEffect(() => {
    if (props.onChange) {
      props.onChange(tree);
    }
  }, [tree, props]);
  const generateArrayChildItem = (item: ArrayChildItem, index: number) => {
    return (
      <Grid container className={classes.arrayItems}>
        <div className={classes.arrayItemsInner}>
          <p>{item.name}</p>
          <div style={{ paddingLeft: '1.3rem' }}>
            {item.value.map((x, aIndex) => {
              return (
                <div
                  key={item.key + aIndex}
                  style={{ display: 'flex', flexDirection: 'row' }}
                >
                  <TextField
                    variant="outlined"
                    margin="dense"
                    value={x}
                    onChange={e => {
                      const reTree = Object.assign({}, tree);
                      (reTree.children[index] as ArrayChildItem).value[aIndex] =
                        e.target.value;
                      setTree(reTree);
                    }}
                  />
                  <Tooltip title="Remove this item">
                    <IconButton
                      onClick={() => {
                        const reTree = Object.assign({}, tree);
                        (reTree.children[index] as ArrayChildItem).value.splice(
                          aIndex,
                          1
                        );
                        setTree(reTree);
                      }}
                    >
                      <DeleteIcon />
                    </IconButton>
                  </Tooltip>
                </div>
              );
            })}
          </div>
          <Tooltip title="Add array item">
            <IconButton
              style={{
                width: theme.spacing(6),
                height: theme.spacing(6),
                marginLeft: '1.5rem'
              }}
              onClick={() => {
                const reTree = Object.assign({}, tree);
                (reTree.children[index] as ArrayChildItem).value.push('');
                setTree(reTree);
              }}
            >
              <AddIcon />
            </IconButton>
          </Tooltip>
        </div>
      </Grid>
    );
  };

  const generateKeyValuePairItem = (item: KeyValueChildItem, index: number) => {
    const stringValue = () => {
      return (
        <TextField
          value={item.value}
          margin="dense"
          variant="outlined"
          style={{ marginLeft: '0.5rem' }}
          onChange={e => {
            const reTree = Object.assign({}, tree);
            reTree.children[index].value = e.target.value;
            setTree(reTree);
          }}
        />
      );
    };
    const booleanValue = () => {
      return (
        <Select
          onChange={e => {
            const reTree = Object.assign({}, tree);
            const value = e.target.value as string;

            reTree.children[index].value = value === 'true' ? true : false;
            setTree(reTree);
          }}
          value={item.value.toString()}
          margin="dense"
          style={{ marginLeft: '0.5rem' }}
        >
          <MenuItem value={'true'}>True</MenuItem>
          <MenuItem value={'false'}>False</MenuItem>
        </Select>
      );
    };
    const numberValue = () => {
      return (
        <TextField
          value={item.value}
          margin="dense"
          variant="outlined"
          type="number"
          style={{ marginLeft: '0.5rem' }}
          onChange={e => {
            const reTree = Object.assign({}, tree);
            reTree.children[index].value = parseInt(e.target.value);
            setTree(reTree);
          }}
        />
      );
    };
    const generate = () => {
      switch (typeof item.value) {
        case 'string':
          return stringValue();
        case 'number':
          return numberValue();
        case 'boolean':
          return booleanValue();
        default:
          return <></>;
      }
    };

    return (
      <>
        <Grid container>
          <div className={classes.keyValuePairItem}>
            <p>{item.name}</p>
            {generate()}
          </div>
        </Grid>
      </>
    );
  };

  return (
    <>
      <div className={classes.root}>
        <form>
          {tree.children.map((x, index) => {
            if (!isKeyValueChildItem(x)) {
              return generateArrayChildItem(x, index);
            } else {
              return generateKeyValuePairItem(x, index);
            }
          })}
        </form>
      </div>
    </>
  );
};
