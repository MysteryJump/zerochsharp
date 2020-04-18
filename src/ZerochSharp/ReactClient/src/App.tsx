import React from 'react';
import './App.css';
import { Provider } from 'react-redux';
import { store } from './store';
import { MainContent } from './components/MainContent';
import { SnackbarProvider } from 'notistack';

const App: React.FC = () => {
  return (
    <Provider store={store}>
      <SnackbarProvider>
        <MainContent />
      </SnackbarProvider>
    </Provider>
  );
};

export default App;
