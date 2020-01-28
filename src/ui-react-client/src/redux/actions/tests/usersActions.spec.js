import * as usersActions from '../usersActions';
import * as types from '../actionTypes';

describe('loadUsers', () => {
  it('should load users action', () => {
    const users = [
      {
        id: 0,
        firstName: '',
        lastName: '',
        username: '',
        locationId: 0,
      },
    ];

    const expectedAction = {
      type: types.LOAD_USERS_ALL,
      users: users,
    };

    const action = usersActions.loadUsersAllData(users);

    expect(action).toEqual(expectedAction);
  });
});
