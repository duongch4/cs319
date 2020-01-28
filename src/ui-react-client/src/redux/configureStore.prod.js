import { createStore, applyMiddleware } from 'redux';
import rootReducer from './reducers/_rootReducer';
import thunk from 'redux-thunk';

export const configureStore = initialState => {
  return createStore(
    rootReducer,
    initialState,
    applyMiddleware(thunk),
  );
};

export default configureStore;
