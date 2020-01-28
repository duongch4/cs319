import usersReducer from '../usersReducer';
import * as actions from '../../actions/usersActions';

const initialState = [
  {
    id: 1,
    firstName: 'Joe',
    lastName: '',
    username: '',
    locationId: 0,
  },
  {
    id: 2,
    firstName: 'Jane',
    lastName: '',
    username: '',
    locationId: 0,
  },
];

it('should load all locations when passed LOAD_LOCATIONS_ALL', () => {
  const action = actions.loadUsersAllData(initialState);

  const newState = usersReducer(initialState, action);

  expect(newState.length).toEqual(2);
  expect(newState[0].firstName).toEqual('Joe');
  expect(newState[1].firstName).toEqual('Jane');
});
