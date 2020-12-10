import React, { useState, useEffect } from 'react';
import {
  ExpansionPanel,
  ExpansionPanelActions,
  Button,
  ExpansionPanelDetails,
  ExpansionPanelSummary,
} from '@material-ui/core';
import { TreeTextField, ParentItem } from '../../general/TreeTextField';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Board } from '../../../models/board';
import { useDispatch } from 'react-redux';
import { boardListActions } from '../../../actions/boardListActions';
import Axios from 'axios';

interface Props {
  board?: Board;
}

export const AutoThreadArchivingPanel = (props: Props) => {
  const dispatch = useDispatch();

  const [predicates, setPredicates] = useState<ParentItem | undefined>(
    undefined
  );

  const postPredicateSettings = () => {
    // for Chromium edge
    const predsTmp: string[] = predicates?.children[0].value as string[];
    Axios.post(`/api/boards/${props.board?.boardKey}/setting`, {
      autoArchivingPredicates: predsTmp,
    })
      .then((x) => {
        dispatch(boardListActions.fetchBoardList());
      })
      .catch((e) => console.error(e));
  };

  useEffect(() => {
    if (props.board && props.board.autoArchivingPredicates) {
      const preds: ParentItem = {
        name: 'Predicates',
        key: 'predicates-parent-item-key',
        children: [
          {
            name: 'Predicates',
            value: props.board.autoArchivingPredicates,
            key: 'predicated-key-' + props.board.boardKey,
          },
        ],
      };
      setPredicates(preds);
    }
  }, [props.board]);

  return (
    <ExpansionPanel>
      <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
        Auto Thread Archiving
      </ExpansionPanelSummary>
      <ExpansionPanelDetails>
        {predicates == null ? (
          <></>
        ) : (
          <TreeTextField
            item={predicates}
            onChange={(items) => setPredicates(items)}
          />
        )}
      </ExpansionPanelDetails>
      <ExpansionPanelActions>
        <Button
          onClick={() => {
            postPredicateSettings();
          }}
        >
          Save
        </Button>
      </ExpansionPanelActions>
    </ExpansionPanel>
  );
};
