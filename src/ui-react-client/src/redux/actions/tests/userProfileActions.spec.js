import * as userProfileActions from '../userProfileActions';
import * as types from '../actionTypes';

describe('loadUserProfileData', () => {
  it('should load users action using userId', () => {
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
      type: types.LOAD_USERS_SPECIFIC,
      userID: users[0].id,
    };

    const action = userProfileActions.loadUserProfileData(0);

    expect(action).toEqual(expectedAction);
  });
});
