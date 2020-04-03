import * as masterlistsActions from '../masterlistsActions';
import * as types from '../actionTypes';
import axios from 'axios';
import * as authUtils from '../../../config/authUtils';
import configureStore from 'redux-mock-store';
import thunk from 'redux-thunk';
import { SVC_ROOT, CLIENT_DEV_ENV } from '../../../config/config';

jest.mock('axios');
jest.mock('../../../config/authUtils');
const middlewares = [thunk];
const mockStore = configureStore(middlewares);
const store = mockStore({});

const adminRole = ["adminUser", "regularUser"];
const regularRole = ["regularUser"];
const baseURL = `${SVC_ROOT}api/`;

// TODO for all the test groups
// Invalid authentication error from getHeaders
// internal server error
// bad request error
// not found error

describe('Create Disciplines', () => {
    afterEach(() => {
        store.clearActions();
        axios.post.mockClear();
        authUtils.getHeaders.mockClear();
    })

    it('should successfully add a discipline', async () => {
        axios.post.mockResolvedValueOnce({data: {code: 201,
            status: "Created",
            payload: 1,
            message: "Successfully created discipline 1",
            extra: {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        
        const discipline = {
            id: 5,
            name: "Intel",
            skills: "\"Deception,False Identity Creation\""
          }

        const expectedAction = [{
              type: types.CREATE_DISCIPLINE,
              disciplines: discipline
          }];
        await store.dispatch(masterlistsActions.createDiscpline(discipline, adminRole));

        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}admin/disciplines`, discipline, {headers:{ Authorization: `Bearer 100` }});
    });
})

describe('Delete Disciplines', () => {
    afterEach(() => {
        store.clearActions();
        axios.delete.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully delete discipline', async () => {
        axios.delete.mockResolvedValueOnce({data: {"code": 200,
        "status": "OK",
        "payload": 1,
        "message": "Successfully deleted discipline 1",
        "extra": {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let id = 1;
        const expectedAction = [{
            type: types.DELETE_DISCIPLINE,
            id: id
        }];
      await store.dispatch(masterlistsActions.deleteDiscipline(id, adminRole));

      expect(store.getActions()).toEqual(expectedAction);
      expect(axios.delete).toHaveBeenCalledTimes(1);
      expect(axios.delete).toHaveBeenCalledWith(`${baseURL}admin/disciplines/${id}`, {headers:{ Authorization: `Bearer 100` }});
    })
})

describe('Create Skills', () => {
    afterEach(() => {
        store.clearActions();
        axios.post.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully add a skill', async () => {
        axios.post.mockResolvedValueOnce({data: {code: 201,
            status: "Created",
            payload: 1,
            message: "Successfully created skill 1 associated with discipline 100",
            extra: {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let skill = {
            disciplineID: 100,
            skillID: 1,
            Name: 'new test skill'
        };
        let expectedAction = [{
            type: types.CREATE_SKILL,
            skill: skill
        }];

        await store.dispatch(masterlistsActions.createSkill(skill, adminRole));

        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}admin/disciplines/100/skills`, skill, {headers:{ Authorization: `Bearer 100` }});
    })
})

describe('Delete Skills', () => {
    afterEach(() => {
        store.clearActions();
        axios.delete.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully delete discipline', async () => {
        axios.delete.mockResolvedValueOnce({data: {"code": 200,
        "status": "OK",
        "payload": 1,
        "message": "Successfully deleted skill 1",
        "extra": {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let skill = {
            disciplineID: 100,
            skillID: 1,
            name: 'some skill name'
        };
        let expectedAction = [{
            type: types.DELETE_SKILL,
            disciplineID: skill.disciplineID,
            skillName: skill.name
        }];
      await store.dispatch(masterlistsActions.deleteSkill(skill.disciplineID, skill.name, adminRole));

      expect(store.getActions()).toEqual(expectedAction);
      expect(axios.delete).toHaveBeenCalledTimes(1);
      expect(axios.delete).toHaveBeenCalledWith(`${baseURL}admin/disciplines/${skill.disciplineID}/skills/${skill.name}`, {headers:{ Authorization: `Bearer 100` }});
    })
})

describe('Create Provinces', () => {
    afterEach(() => {
        store.clearActions();
        axios.post.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully add a province', async () => {
        axios.post.mockResolvedValueOnce({data: {code: 201,
            status: "Created",
            payload: "new province",
            message: "Successfully created province new province",
            extra: {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let location = {
            locationID: 0,
            province: 'new province',
            city: null
        }
        let expectedAction = [{
            type: types.CREATE_PROVINCE,
            location: location
        }];

        await store.dispatch(masterlistsActions.createProvince(location, adminRole));

        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}admin/provinces`, location, {headers:{ Authorization: `Bearer 100` }});
    
    })
})

describe('Delete Provinces', () => {
    afterEach(() => {
        store.clearActions();
        axios.delete.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully delete province', async () => {
        axios.delete.mockResolvedValueOnce({data: {code: 200,
            status: "OK",
            payload: "test_province",
            message: "Successfully deleted province test province",
            extra: {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        const expectedAction = [{
            type: types.DELETE_PROVINCE,
            provinceName: 'test_province',
        }];
      await store.dispatch(masterlistsActions.deleteProvince('test_province', adminRole));

      expect(store.getActions()).toEqual(expectedAction);
      expect(axios.delete).toHaveBeenCalledTimes(1);
      expect(axios.delete).toHaveBeenCalledWith(`${baseURL}admin/provinces/test_province`, {headers:{ Authorization: `Bearer 100` }});
    })
})

describe('Create City', () => {
    afterEach(() => {
        store.clearActions();
        axios.post.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully add a city', async () => {
        axios.post.mockResolvedValueOnce({data: {code: 201,
            status: "Created",
            payload: 1,
            message: "Successfully created location 12",
            extra: {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let location = {
            locationID: 12,
            province: 'province',
            city: 'new city'
        }
        let expectedAction = [{
            type: types.CREATE_CITY,
            location: location
        }];

        await store.dispatch(masterlistsActions.createCity(location, adminRole));

        expect(store.getActions()).toEqual(expectedAction);
        expect(axios.post).toHaveBeenCalledTimes(1);
        expect(axios.post).toHaveBeenCalledWith(`${baseURL}admin/locations`, location, {headers:{ Authorization: `Bearer 100` }});
    
    })
})

describe('Delete Disciplines', () => {
    afterEach(() => {
        store.clearActions();
        axios.delete.mockClear();
        authUtils.getHeaders.mockClear();
    });

    it('should successfully delete skill', async () => {
        axios.delete.mockResolvedValueOnce({data: {"code": 200,
        "status": "OK",
        "payload": 12,
        "message": "Successfully deleted location 12",
        "extra": {}}});
        authUtils.getHeaders.mockResolvedValueOnce({Authorization: `Bearer 100`});
        let location = {
            locationID: 12,
            province: 'province',
            city: 'new city'
        }
        const expectedAction = [{
            type: types.DELETE_CITY,
            name: location.city,
            id: location.locationID
        }];
      await store.dispatch(masterlistsActions.deleteCity(location.city, location.locationID, adminRole));

      expect(store.getActions()).toEqual(expectedAction);
      expect(axios.delete).toHaveBeenCalledTimes(1);
      expect(axios.delete).toHaveBeenCalledWith(`${baseURL}admin/locations/${location.locationID}`, {headers:{ Authorization: `Bearer 100` }});
    })
})