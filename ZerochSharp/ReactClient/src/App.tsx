import React from 'react';
import './App.css';
import { Provider } from 'react-redux';
import { store } from './store';
import { MainContent } from './components/MainContent';

const App: React.FC = () => {
  return (
    <Provider store={store}>
      <MainContent />
    </Provider>
  );
};

export default App;
