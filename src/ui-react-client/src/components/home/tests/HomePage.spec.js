import React from 'react';
import { shallow } from 'enzyme';
import HomePage from '../HomePage';

const renderHomePage = () => {
  return shallow(<HomePage />);
};
it('render HomePage component, header, parapgraph', () => {
  const wrapper = renderHomePage();
  expect(wrapper.find('h1').length).toBe(1);
  expect(wrapper.find('h1').text()).toBe('Home');
  expect(wrapper.find('p').length).toBe(1);
  expect(wrapper.find('p').text()).toBe('Boilerplate project');
});
