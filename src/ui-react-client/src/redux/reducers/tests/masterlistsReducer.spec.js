import masterlistsReducer from '../masterlistsReducer';
import * as types from '../../actions/actionTypes';

let initialState = {
        disciplines: {
            "Environmental Design": {
                disciplineID: 1,
                skills: ["Skill 1","Skill 2"]
            },
            "Waste Management": {
                disciplineID: 2,
                skills: ["Skill A","Skill B"]
            },
            "Parks and Recreation": {
                disciplineID: 3,
                skills: ["Skill 1A","Skill 2B"]
            }
        },
        locations: {
            "British Columbia": {
                "Vancouver": 1,
                "Richmond": 2
            },
            "Seskatchewan": {
                "Saskatoon": 3
            },
            "Ontario": {
                "Toronto": 4
            }
        },
        yearsOfExp: [
            "3-5",
            "10+",
            "1-3"
        ]
    }

afterEach(() => {
    initialState = {
        disciplines: {
            "Environmental Design": {
                disciplineID: 1,
                skills: ["Skill 1","Skill 2"]
            },
            "Waste Management": {
                disciplineID: 2,
                skills: ["Skill A","Skill B"]
            },
            "Parks and Recreation": {
                disciplineID: 3,
                skills: ["Skill 1A","Skill 2B"]
            }
        },
        locations: {
            "British Columbia": {
                "Vancouver": 1,
                "Richmond": 2
            },
            "Seskatchewan": {
                "Saskatoon": 3
            },
            "Ontario": {
                "Toronto": 4
            }
        },
        yearsOfExp: [
            "3-5",
            "10+",
            "1-3"
        ]
    }
})

it('should load the initial state as default' , () => {
    let action = {type: 'random_string'};
    let received = masterlistsReducer(initialState, action);

    expect(received).toEqual(initialState);
    expect(received.error).toBeUndefined();
});

it('should should load masterlist payload from action', () => {
    let action = {type: types.LOAD_MASTERLIST, masterlist: {}};
    let received = masterlistsReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual({});
});

it('should add a new discipline to the masterlist in state', () => {
    let discipline = {
        name: 'Secret Keeping',
        id: 42
    };
    let action = {type: types.CREATE_DISCIPLINE, disciplines: discipline};
    let received = masterlistsReducer(initialState, action);
    let receivedDisciplines = received.disciplines;
    let disciplineKeys = Object.keys(receivedDisciplines);

    expect(disciplineKeys.length).toEqual(4);
    expect(disciplineKeys).toContain(discipline.name);
    expect(receivedDisciplines[discipline.name]).toEqual(
        expect.objectContaining({
            disciplineID: 42,
            skills: []
    }));
    expect(received.error).toBeNull();
});

it('should add a new skill to the masterlist in state', () => {
    let skill = {
        disciplineID: 2,
        skillID: 1,
        name: 'new test skill'
    };
    let action = {type: types.CREATE_SKILL, skill: skill};
    let received = masterlistsReducer(initialState, action);
    let receivedDiscipline = received.disciplines['Waste Management'];

    expect(receivedDiscipline.skills.length).toEqual(3);
    expect(receivedDiscipline.skills).toContain(skill.name);
    expect(received.error).toBeNull();
});

it('should add a new province to the masterlist in state', () => {
    let location = {
        id: 0,
        province: 'Pawnee',
        city: null
    };
    let action = {type: types.CREATE_PROVINCE, location: location};
    let received = masterlistsReducer(initialState, action);
    let receivedLocations = received.locations;
    let locationKeys = Object.keys(receivedLocations);

    expect(locationKeys.length).toEqual(4);
    expect(locationKeys).toContain(location.province);
    expect(receivedLocations[location.province]).toEqual({});
    expect(received.error).toBeNull();
});

it('should add a new city to the masterlist in state', () => {
    let location = {
        id: 12,
        province: 'British Columbia',
        city: 'Tofino'
    };
    let action = {type: types.CREATE_CITY, location: location};
    let received = masterlistsReducer(initialState, action);
    let receivedLocation = received.locations[location.province];
    let cityKeys = Object.keys(receivedLocation);

    expect(cityKeys.length).toEqual(3);
    expect(cityKeys).toContain(location.city);
    expect(receivedLocation[location.city]).toEqual(location.id);
    expect(received.error).toBeNull();
});

it('should remove a discipline from the masterlist in state', () => {
    let discipline = {
        name: 'Secret Keeping',
        id: 42
    };
    let prepAction = {type: types.CREATE_DISCIPLINE, disciplines: discipline};
    let prepState = masterlistsReducer(initialState, prepAction);

    expect(prepState.disciplines).toEqual(expect.objectContaining({
        [discipline.name]: { disciplineID: 42,
                             skills: []}
    }))

    let action = {type: types.DELETE_DISCIPLINE, id: discipline.id};
    let received = masterlistsReducer(prepState, action);

    expect(received.disciplines).not.toEqual(expect.objectContaining({
        [discipline.name]: { disciplineID: 42,
                             skills: []}
    }));
    expect(received.error).toBeNull();
});

it('should remove a skill from the masterlist in state', () => {
    let skill = {
        disciplineID: 2,
        skillID: 1,
        name: 'new test skill'
    };
    let prepAction = {type: types.CREATE_SKILL, skill: skill};
    let prepState = masterlistsReducer(initialState, prepAction);

    expect(prepState.disciplines['Waste Management'].skills).toContain(skill.name);
    expect(prepState.disciplines['Waste Management'].skills.length).toEqual(3);

    let action = {type: types.DELETE_SKILL, disciplineID: skill.disciplineID, skillName: skill.name};
    let received = masterlistsReducer(prepState, action);
    let receivedDiscipline = received.disciplines['Waste Management'];

    expect(receivedDiscipline.skills.length).toEqual(2);
    expect(receivedDiscipline.skills).not.toContain(skill.name);
    expect(received.error).toBeNull();
});

it('should remove a province from the masterlist in state', () => {
    let location = {
        id: 0,
        province: 'Pawnee',
        city: null
    };
    let prepAction = {type: types.CREATE_PROVINCE, location: location};
    let prepState = masterlistsReducer(initialState, prepAction);

    expect(prepState.locations).toEqual(expect.objectContaining({
        [location.province]: {}
    }));

    let action = {type: types.DELETE_PROVINCE, provinceName: location.province};
    let received = masterlistsReducer(prepState, action);

    expect(received.locations).not.toEqual(expect.objectContaining({
        [location.province]: {}
    }));
    expect(received.error).toBeNull();
});

it('should remove a city from the masterlist in state', () => {
    let location = {
        id: 12,
        province: 'British Columbia',
        city: 'Tofino'
    };
    let prepAction = {type: types.CREATE_CITY, location: location};
    let prepState = masterlistsReducer(initialState, prepAction);

    expect(prepState.locations).toEqual(expect.objectContaining({
        "British Columbia": expect.objectContaining({
            [location.city]: location.id
        })
    }));

    let action = {type: types.DELETE_CITY, name: location.city, id: location.id};
    let received = masterlistsReducer(prepState, action);
    expect(received.locations).toEqual(expect.objectContaining({
        "British Columbia": expect.not.objectContaining({
            [location.city]: location.id
        })
    }));
    expect(received.error).toBeNull();
});

it('should append deletion error to the state', () => {
    let error = 'deleting error';
    let action = {type: types.ERROR_DELETING, error: error};
    let received = masterlistsReducer(initialState, action);

    expect(received).toEqual(expect.objectContaining({error: error}))
});

it('should append creation error to the state', () => {
    let error = 'creating error';
    let action = {type: types.ERROR_CREATING, error: error};
    let received = masterlistsReducer(initialState, action);

    expect(received).toEqual(expect.objectContaining({error: error}))
});

it('should remove error from the state',() => {
    let error = 'deleting error';
    let prepAction = {type: types.ERROR_DELETING, error: error};
    let prepState = masterlistsReducer(initialState, prepAction);

    expect(prepState).toEqual(expect.objectContaining({error: error}));

    let action = {type: types.CLEAR_ERROR};
    let received = masterlistsReducer(prepState, action);

    expect(received).toEqual(expect.not.objectContaining({error: error}));
});

