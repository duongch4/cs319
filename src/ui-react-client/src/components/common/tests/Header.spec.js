import React from 'react';
import { shallow } from 'enzyme';
import Header from '../Header';

it('test for x number of navLinks', () => {
  const header = shallow(<Header />);
  expect(header.find('NavLink').length).toEqual(4);
});
