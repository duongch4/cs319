import * as masterlistsActions from '../masterlistsActions';
import * as types from '../actionTypes';
import axios from 'axios';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);

const adminRole = ["adminUser", "regularUser"];
const regularRole = ["regularUser"];

describe('Add Disciplines', () => {
    const store = mockStore({});
    afterEach(() => {
        store.clearActions();
    })
    it('should successfully add a discipline', async () => {
        axios.post.mockImplementationOnce(() => Promise.resolve({
            data: {
                payload: 1
            }
        }));

        authUtils.getHeaders.mockImplementationOnce(() => Promise.resolve({}));
        const discipline = {
            id: 5,
            name: "Intel",
            skills: "\"Deception,False Identity Creation\""
          }

        const expectedAction = {
              type: types.CREATE_DISCIPLINE,
              disciplines: discipline
          };
        await store.dispatch(masterlistsActions.createDiscpline(discipline, adminRole));

        const actions = store.getActions();
        expect(actions.length).toEqual(1);
        const action = actions[0];
        expect(action).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
    });

    // it('should catch error', async () => {
    //     let error = {
    //         "code": 400,
    //         "status": "Bad Request",
    //         "message": "Invalid Request Parameters"
    //       }
    //     axios.post.mockImplementationOnce(() => Promise.reject({status: 400, data: error}));
    //     authUtils.getHeaders.mockImplementationOnce(() => Promise.resolve({}));
    //     const discipline = {
    //         id: 5,
    //         name: "Intel",
    //         skills: "\"Deception,False Identity Creation\""
    //       }
    //     const expectedAction = {
    //         type: types.ERROR_CREATING,
    //         error: error
    //     };
    //     await expect(store.dispatch(masterlistsActions.createDiscpline(discipline, adminRole)));

    //     const actions = store.getActions();
    //     expect(actions.length).toEqual(1);
    //     const action = actions[0];
    //     expect(action).toEqual(expectedAction);
    // })
})