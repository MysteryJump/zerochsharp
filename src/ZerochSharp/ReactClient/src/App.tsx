import React from 'react';
import './App.css';
import { Provider } from 'react-redux';
import { store } from './store';
import { MainContent } from './components/MainContent';
import { SnackbarProvider } from 'notistack';
import { SnackbarNotifier } from './components/SnackbarNotifier';

const App: React.FC = () => {
  return (
    <Provider store={store}>
      <SnackbarProvider>
        <SnackbarNotifier>
          <MainContent />
        </SnackbarNotifier>
      </SnackbarProvider>
    </Provider>
  );
};

export default App;
