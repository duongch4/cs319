import locationsReducer from '../locationsReducer';
import * as actions from '../../actions/locationsActions';

const initialState = [
  {
    id: 1,
    code: 'abc',
    name: '',
  },
  {
    id: 2,
    code: 'xyz',
    name: '',
  },
];

it('should load all locations when passed LOAD_LOCATIONS_ALL', () => {
  const action = actions.loadLocationsAllData(initialState);

  const newState = locationsReducer(initialState, action);

  expect(newState.length).toEqual(2);
  expect(newState[0].code).toEqual('abc');
  expect(newState[1].code).toEqual('xyz');
});
