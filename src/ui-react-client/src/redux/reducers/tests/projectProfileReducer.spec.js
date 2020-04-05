import projectProfileReducer from '../projectProfileReducer';
import * as types from '../../actions/actionTypes';

let initialState = {
    projectSummary: {
        title: "Martensville Athletic Pavillion",
        location: {
            locationID: 1,
            province: "Seskatchewan",
            city: "Saskatoon"
        },
        projectStartDate: "2020-10-31T00:00:00.0000000",
        projectEndDate: "2021-12-31T00:00:00.0000000",
        projectNumber: "2009-VD9D-15"
    },
    projectManager: {
        userID: 100,
        firstName: "Jason",
        lastName: "Bourne"
    },
    usersSummary: [
        {
            firstName: "Nicky",
            lastName: "Parsons",
            userID: 101,
            location: {
                locationID: 2,
                province: "British Columbia",
                city: "Vancouver"
            },
            utilization: 100,
            resourceDiscipline: {
                disciplineID: 123,
                discipline: "Logistical Operations and Mental Health Analysis",
                yearsOfExp: "3-5"
            },
            isConfirmed: true
        },
        {
            firstName: "Pamela",
            lastName: "Landy",
            userID: 102,
            location: {
                locationID: 3,
                province: "Ontario",
                city: "Toronto"
            },
            utilization: 100,
            resourceDiscipline: {
                disciplineID: 103,
                discipline: "Reconnaisance",
                yearsOfExp: "10+"
            },
            isConfirmed: false
        }
    ],
    openings: [
        {
            positionID: 1,
            discipline: "Environmental Design",
            skills: [
                "skill1", "skill2"
            ],
            yearsOfExp: "1-3",
            commitmentMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50
             }
        },
        {
            positionID: 2,
            discipline: "Waste Management",
            skills: [],
            yearsOfExp: "1-3",
            commitmentMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50
             }
        }
    ]
};

afterEach(() => {
    initialState = {
        projectSummary: {
            title: "Martensville Athletic Pavillion",
            location: {
                locationID: 1,
                province: "Seskatchewan",
                city: "Saskatoon"
            },
            projectStartDate: "2020-10-31T00:00:00.0000000",
            projectEndDate: "2021-12-31T00:00:00.0000000",
            projectNumber: "2009-VD9D-15"
        },
        projectManager: {
            userID: 100,
            firstName: "Jason",
            lastName: "Bourne"
        },
        usersSummary: [
            {
                firstName: "Nicky",
                lastName: "Parsons",
                userID: 101,
                location: {
                    locationID: 2,
                    province: "British Columbia",
                    city: "Vancouver"
                },
                utilization: 100,
                resourceDiscipline: {
                    disciplineID: 123,
                    discipline: "Logistical Operations and Mental Health Analysis",
                    yearsOfExp: "3-5"
                },
                isConfirmed: true
            },
            {
                firstName: "Pamela",
                lastName: "Landy",
                userID: 102,
                location: {
                    locationID: 3,
                    province: "Ontario",
                    city: "Toronto"
                },
                utilization: 100,
                resourceDiscipline: {
                    disciplineID: 103,
                    discipline: "Reconnaisance",
                    yearsOfExp: "10+"
                },
                isConfirmed: false
            }
        ],
        openings: [
            {
                positionID: 1,
                discipline: "Environmental Design",
                skills: [
                    "skill1", "skill2"
                ],
                yearsOfExp: "1-3",
                commitmentMonthlyHours: {
                    "2020-01-01": 30,
                    "2020-02-01": 50
                 }
            },
            {
                positionID: 2,
                discipline: "Waste Management",
                skills: [],
                yearsOfExp: "1-3",
                commitmentMonthlyHours: {
                    "2020-01-01": 30,
                    "2020-02-01": 50
                 }
            }
        ]
    };
})

it('should return initial state by default', () => {
    let action = {type: 'random_type'};
    let received = projectProfileReducer(initialState, action);

    expect(received).toEqual(initialState);
});

it('should load single project from action payload', () => {
    let projectProfile = {project: 'sample projectProfile'};
    let action = {type: types.LOAD_SINGLE_PROJECT, projectProfile: projectProfile};
    let received = projectProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual(projectProfile);
});

it('should load the newly created project from action payload', () => {
    let projectProfile = {project: 'sample projectProfile'};
    let action = {type: types.CREATE_PROJECT, projectProfile: projectProfile};
    let received = projectProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual(projectProfile);
});

it('should load updated project from action payload', () => {
    let projectProfile = {project: 'sample projectProfile'};
    let action = {type: types.UPDATE_PROJECT, projectProfile: projectProfile};
    let received = projectProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual(projectProfile);
});

it('should load an empty object when project is deleted', () => {
    let action = {type: types.DELETE_PROJECT};
    let received = projectProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual({});
});

it('should add new opening to the state', () => {
    let user =  {
        firstName: 'Thor',
        lastName: 'Odinson',
        userID: 42,
        location: {
            locationID: 2,
            province: "British Columbia",
            city: "Vancouver"
        },
        utilization: 99,
        resourceDiscipline: {
            disciplineID: 300,
            discipline: "Lighting Bolts",
            yearsOfExp: "3-5"
        },
        isConfirmed: false
    };
    let action = {type: types.UPDATE_ASSIGN_OPENING, openingID: 2,
                userId: user.userID, confirmedUtilization: 100, user: user};
    let received = projectProfileReducer(initialState, action);
    let receivedUsers = received.usersSummary;
    let receivedOpenings = received.openings;

    expect(receivedOpenings).toHaveLength(1);
    expect(receivedOpenings).not.toContain({
        positionID: 2,
        discipline: "Waste Management",
        skills: [],
        yearsOfExp: "1-3",
        commitmentMonthlyHours: {
            "2020-01-01": 30,
            "2020-02-01": 50
         }
    });
    expect(receivedUsers).toHaveLength(3);
    expect(receivedUsers).toContain(user);
});

it('should update user summary to confirmed', () => {
    let user =  {
        firstName: 'Thor',
        lastName: 'Odinson',
        userID: 42,
        location: {
            locationID: 2,
            province: "British Columbia",
            city: "Vancouver"
        },
        utilization: 99,
        resourceDiscipline: {
            disciplineID: 300,
            discipline: "Lighting Bolts",
            yearsOfExp: "3-5"
        },
        isConfirmed: false
    };
    let prepAction = {type: types.UPDATE_ASSIGN_OPENING, openingID: 2,
                userId: user.userID, confirmedUtilization: 100, user: user};
    let prepState = projectProfileReducer(initialState, prepAction);

    let action = {
        type: types.CONFIRM_ASSIGN_OPENING,
        openingId: 2,
        userId: user.userID,
        confirmedUtilization: 100,
        userSummaryDisciplineName: user.resourceDiscipline.discipline
    }
    let received = projectProfileReducer(prepState, action);
    let receivedUser = received.usersSummary.find(u => u.userID === user.userID);

    expect(receivedUser).not.toBeUndefined();
    expect(receivedUser.isConfirmed).toEqual(true);
});

// it('should remove user from opening', () => {
//     // {
//     //     type: types.UNASSIGN_OPENING,
//     //     openingId: openingId,
//     //     userId: userId,
//     //     confirmedUtilization: confirmedUtilization,
//     //     userSummaryDisciplineName: userSummaryDisciplineName
//     // }
// });
