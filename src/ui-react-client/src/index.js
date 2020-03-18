import 'react-app-polyfill/ie11';
import 'react-app-polyfill/stable';

import React from 'react';
import { render } from 'react-dom';
import { BrowserRouter as Router } from 'react-router-dom';
import './index.css';
import './components/common/common.css';
import App from './components/App';
import configureStore from './redux/configureStore';
import { Provider } from 'react-redux';

const store = configureStore();

render(
  <Provider store={store}>
    <Router>
      <App />
    </Router>
  </Provider>,
  document.getElementById('root'),
);
