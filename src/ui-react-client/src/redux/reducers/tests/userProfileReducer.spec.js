import userProfileReducer from '../userProfileReducer';
import * as userProfileActions from '../../actions/userProfileActions';

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
  
  it('should load user when passed LOAD_USERS_SPECIFIC and userId', () => {
    const action = userProfileActions.loadUserProfileData(initialState);
  
    const newState = userProfileReducer(initialState, action);
    const singleUser = newState.find(
        element => element.id == 1,
      );
    expect(singleUser.firstName).toEqual('Joe');
    expect(singleUser.locationId).toEqual(0);
  });
  


 
  
//   export const updateUserProfileData = userID => {
//     return {
//       type: types.UPDATE_USERS_SPECIFIC,
//       userID: userID
//     }
//   };
  
//   export const loadSpecificUser = (userID) => {
//     return dispatch => {
//       dispatch(loadUserProfileData(userID));
//       // XXX TODO: Uncomment this for full-stack integration
//     };
//   };
  
//   export const updateSpecificUser = (user) => {
//     return dispatch => {
//       dispatch(updateUserProfileData(user));
//       // TODO: updating a user profile
//     }
//   };