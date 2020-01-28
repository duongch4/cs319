import React from 'react';
import { shallow } from 'enzyme';
import ProjectList from '../ProjectList';

const renderProjectList = args => {
  const project = [
    {
      id: 1,
      number: '',
      title: '',
      location: '',
      createdAt: '',
      updatedAt: '',
    },
  ];
  return shallow(<ProjectList projects={project} />);
};

it('render ProjectList and check table headers and table data', () => {
  const wrapper = renderProjectList();
  expect(wrapper.find('th').length).toBe(6);
  expect(wrapper.find('td').length).toBe(6);
});
