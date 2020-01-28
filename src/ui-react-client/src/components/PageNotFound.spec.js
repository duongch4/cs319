import React from 'react';
import { shallow } from 'enzyme';
import PageNotFound from './PageNotFound';

const renderPageNotFound = () => {
  return shallow(<PageNotFound />);
};

it('render pageNotFound component and header', () => {
  const wrapper = renderPageNotFound();
  expect(wrapper.find('h1').length).toBe(1);
  expect(wrapper.find('h1').text()).toBe(' 404, page not found. ');
});
