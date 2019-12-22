import React from 'react';
import './App.css';
import { Provider } from 'react-redux';
import {store} from './store'
import MainContentContainer from './containers/MainContentContainer';

const App: React.FC = () => {
  return (
    <Provider store={store}>
      <MainContentContainer />
    </Provider>
  );
}

export default App;
