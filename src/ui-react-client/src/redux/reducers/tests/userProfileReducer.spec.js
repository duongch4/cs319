import userProfileReducer from '../userProfileReducer';
import * as types from '../../actions/actionTypes';

let initialState = {
    userSummary: {
        userID: '100',
        firstName: "Jason",
        lastName: "Bourne",
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
        isConfirmed: false
    },
    currentProjects: [
        {
            title: "Martensville Athletic Pavillion",
            location: {
                locationID: 1,
                province: "Seskatchewan",
                city: "Saskatoon"
            },
            projectStartDate: "2020-10-31T00:00:00.0000000",
            projectEndDate: "2021-12-31T00:00:00.0000000",
            projectNumber: "2009-VD9D-15"
        }
    ],
    availability: [
        {
            fromDate: "2020-10-31T00:00:00",
            toDate: "2020-11-11T00:00:00",
            reason: "Paternal Leave"
        }
    ],
    disciplines: [
        {
            disciplineID: 105,
            name: "Intel",
            yearsOfExp: "10+",
            skills: [
                "Deception"
            ]
        },
        {
            disciplineID: 106,
            name: "Language",
            yearsOfExp: "10+",
            skills: [
                "Russian"
            ]
        }
    ],
    positions: [
        {
            positionID: 1,
            positionName: "Spy",
            projectTitle: "Martensville Athletic Pavillion",
            disciplineName: "Intel",
            projectedMonthlyHours: {
                "2020-01-01": 30,
                "2020-02-01": 50,
                "2020-03-01": 50
             }
        }
    ]
}

afterEach(() => {
    initialState = {
        userSummary: {
            userID: '100',
            firstName: "Jason",
            lastName: "Bourne",
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
            isConfirmed: false
        },
        currentProjects: [
            {
                title: "Martensville Athletic Pavillion",
                location: {
                    locationID: 1,
                    province: "Seskatchewan",
                    city: "Saskatoon"
                },
                projectStartDate: "2020-10-31T00:00:00.0000000",
                projectEndDate: "2021-12-31T00:00:00.0000000",
                projectNumber: "2009-VD9D-15"
            }
        ],
        availability: [
            {
                fromDate: "2020-10-31T00:00:00",
                toDate: "2020-11-11T00:00:00",
                reason: "Paternal Leave"
            }
        ],
        disciplines: [
            {
                disciplineID: 105,
                name: "Intel",
                yearsOfExp: "10+",
                skills: [
                    "Deception"
                ]
            },
            {
                disciplineID: 106,
                name: "Language",
                yearsOfExp: "10+",
                skills: [
                    "Russian"
                ]
            }
        ],
        positions: [
            {
                positionID: 1,
                positionName: "Spy",
                projectTitle: "Martensville Athletic Pavillion",
                disciplineName: "Intel",
                projectedMonthlyHours: {
                    "2020-01-01": 30,
                    "2020-02-01": 50,
                    "2020-03-01": 50
                 }
            }
        ]
    }
})

it('should load the initial state as default' , () => {
    let action = {type: 'random_string'};
    let received = userProfileReducer(initialState, action);

    expect(received).toEqual(initialState);
});

it('should load profile from gives action payload', () => {
    let profile = {};
    let action = {type: types.LOAD_USERS_SPECIFIC, userProfile: profile};
    let received = userProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual(profile);
})

it('should load profile from given update action payload', () => {
    let profile = {};
    let action = {type: types.UPDATE_USERS_SPECIFIC, userProfile: profile};
    let received = userProfileReducer(initialState, action);

    expect(received).not.toEqual(initialState);
    expect(received).toEqual(profile);
})