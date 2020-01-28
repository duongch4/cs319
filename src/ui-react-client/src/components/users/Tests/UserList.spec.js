import React from 'react';
import { shallow } from 'enzyme';
import UserList from '../UserList';

const renderUserList = args => {
  const user = [
    {
      id: 0,
      firstName: '',
      lastName: '',
      username: '',
      locationId: 0,
    },
  ];
  return shallow(<UserList users={user} />);
};

it('render UserList check table headers and table data', () => {
  const wrapper = renderUserList();
  expect(wrapper.find('th').length).toBe(5);
  expect(wrapper.find('td').length).toBe(5);
});
