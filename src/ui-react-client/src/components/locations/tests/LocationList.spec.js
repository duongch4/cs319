import React from 'react';
import { shallow } from 'enzyme';
import LocationList from '../LocationList';

const renderLocationList = args => {
  const location = [
    {
      id: 1,
      code: '',
      name: '',
    },
  ];
  return shallow(<LocationList locations={location} />);
};

it('render LocationList and check table headers and table data', () => {
  const wrapper = renderLocationList();
  expect(wrapper.find('th').length).toBe(3);
  expect(wrapper.find('td').length).toBe(3);
});
