import * as locationsActions from '../locationsActions';
import * as types from '../actionTypes';

describe('loadLocations', () => {
  it('should load locations action', () => {
    const locations = [
      {
        id: 0,
        code: '',
        name: '',
      },
    ];

    const expectedAction = {
      type: types.LOAD_LOCATIONS_ALL,
      locations: locations,
    };

    const action = locationsActions.loadLocationsAllData(locations);

    expect(action).toEqual(expectedAction);
  });
});
