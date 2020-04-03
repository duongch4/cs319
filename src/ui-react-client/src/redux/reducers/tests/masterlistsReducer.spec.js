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
});

// it('should handle invalid action return initial state', () => {
//     let action = null;
//     let received = masterlistsReducer(initialState, action);

//     expect(received).toEqual(initialState);
// });

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
    let receivedKeys = Object.keys(received.disciplines);

    expect(receivedKeys.length).toEqual(4);
    expect(receivedKeys).toEqual(expect.arrayContaining([discipline.name]));
    expect(received.disciplines[discipline.name]).not.toBeUndefined();
    expect(received.disciplines[discipline.name].disciplineID).not.toBeUndefined();
    expect(received.disciplines[discipline.name].disciplineID).toEqual(discipline.id);
    expect(received.disciplines[discipline.name].skills).not.toBeUndefined();
    expect(received.disciplines[discipline.name].skills).toEqual([]);
});

it('should add a new skill to the masterlist in state', () => {
});

it('should add a new province to the masterlist in state', () => {});

it('should add a new city to the masterlist in state', () => {});

it('should remove a discipline from the masterlist in state', () => {});

it('should remove a skill from the masterlist in state', () => {});

it('should remove a province from the masterlist in state', () => {});

it('should remove a city from the masterlist in state', () => {});

it('should append deletion error to the state', () => {});

it('should append creation error to the state', () => {});

it('should remove error from the state',() => {});

