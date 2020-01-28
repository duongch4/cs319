import React from 'react';
import { shallow } from 'enzyme';
import App from './App';

it('app renders without crashing', () => {
  shallow(<App />);
});
