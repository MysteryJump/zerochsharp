import * as session from '../reducers/auth.reducer';
import { createFeatureSelector, createSelector } from '@ngrx/store';

export interface AppState {
  sessionState: session.State;
}

export const reducers = {
  auth: session.reducer
};

export const selectAuthState = createFeatureSelector<AppState>('auth');
