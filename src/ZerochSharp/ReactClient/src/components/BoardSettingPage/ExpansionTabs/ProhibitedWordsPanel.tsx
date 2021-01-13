import React, { useCallback, useEffect, useState } from 'react';
import {
  ExpansionPanel,
  ExpansionPanelSummary,
  ExpansionPanelDetails,
  ExpansionPanelActions,
  Button,
  TextField,
} from '@material-ui/core';
import ExpandMoreIcon from '@material-ui/icons/ExpandMore';
import { Board } from '../../../models/board';
import axios from 'axios';

interface Props {
  board?: Board;
}

interface ProhibitedWord {
  isRegex: boolean;
  pattern: string;
}

export const ProhibitedWordsPanel = (props: Props) => {
  const [prohibitedWords, setProhibitedWords] = useState('');
  const getProhibitedWords = useCallback(() => {
    axios
      .get<{ prohibitedWords: ProhibitedWord[] }>(
        `/api/boards/${props.board?.boardKey}/prohibited-words`
      )
      .then((x) => {
        setProhibitedWords(
          x.data.prohibitedWords
            .map((x) => (x.isRegex ? 'regex:' + x.pattern : x.pattern))
            .reduce((prev, current) => prev + current + '\n', '')
        );
      })
      .catch((e) => console.error(e));
  }, [props.board]);
  const putProhibitedWords = useCallback(() => {
    axios
      .put(`/api/boards/${props.board?.boardKey}/prohibited-words`, {
        prohibitedWords,
      })
      .then((x) => {})
      .catch((e) => console.error(e));
  }, [props.board, prohibitedWords]);
  useEffect(getProhibitedWords, [getProhibitedWords]);
  return (
    <ExpansionPanel>
      <ExpansionPanelSummary expandIcon={<ExpandMoreIcon />}>
        Prohibited Words
      </ExpansionPanelSummary>
      <ExpansionPanelDetails>
        <TextField
          fullWidth
          multiline
          rows={17}
          onChange={(e) => setProhibitedWords(e.target.value)}
          value={prohibitedWords}
        />
      </ExpansionPanelDetails>
      <ExpansionPanelActions>
        <Button onClick={putProhibitedWords}>Save</Button>
      </ExpansionPanelActions>
    </ExpansionPanel>
  );
};
