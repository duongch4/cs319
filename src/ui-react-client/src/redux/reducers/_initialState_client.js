export default {
    users: [
        {
            firstName: "Jason",
            lastName: "Bourne",
            userID: 100,
            location: {
                locationID: 2,
                province: "British Columbia",
                city: "Vancouver"
            },
            utilization: 100,
            resourceDiscipline: {
                disciplineID: 456,
                discipline: "Intel",
                yearsOfExp: "3-5"
            },
            isConfirmed: true
        },
        {
            firstName: "Nicky",
            lastName: "Parsons",
            userID: 101,
            location: {
                locationID: 2,
                province: "British Columbia",
                city: "Vancouver"
            },
            utilization: 75,
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
                disciplineID: 789,
                discipline: "Reconnaisance",
                yearsOfExp: "10+"
            },
            isConfirmed: false
        }],
    projects: [
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
    },
        {
        title: "University of British Columbia Science Building",
        location: {
            locationID: 2,
            province: "British Columbia",
            city: "Vancouver"
        },
        projectStartDate: "2020-10-31T00:00:00.0000000",
        projectEndDate: "2021-12-31T00:00:00.0000000",
        projectNumber: "2009-VD9D-16"
    }],
    projectProfiles: [
        {
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
                        locationID: 4,
                        province: "Ontario",
                        city: "Toronto"
                    },
                    utilization: 100,
                    resourceDiscipline: {
                        disciplineID: 101,
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
        },
        {
            projectSummary: {
                title: "University of British Columbia Science Building",
                location: {
                    locationID: 2,
                    province: "British Columbia",
                    city: "Vancouver"
                },
                projectStartDate: "2020-10-31T00:00:00.0000000",
                projectEndDate: "2021-12-31T00:00:00.0000000",
                projectNumber: "2009-VD9D-16"
            },
            projectManager: {
                "userID": 100,
                "firstName": "Jason",
                "lastName": "Bourne"
            },
            usersSummary: [],
            openings: []
        }
    ],
    userProfiles: [
        {
            userSummary: {
                userID: 100,
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
                    discipline: "Intel",
                    yearsOfExp: "10+",
                    skills: [
                        "Deception"
                    ]
                },
                {
                    disciplineID: 106,
                    discipline: "Language",
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
                        "2020-02-01": 50
                     }
                }
            ]
        }, {
            userSummary: {
                firstName: "Nicky",
                lastName: "Parsons",
                userID: 101,
                location: {
                    locationID: 2,
                    province: "British Columbia",
                    city: "Vancouver"
                },
                utilization: 75,
                resourceDiscipline: {
                    disciplineID: 123,
                    discipline: "Logistical Operations and Mental Health Analysis",
                    yearsOfExp: "3-5"
                },
                isConfirmed: true
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
                    reason: "Maternity Leave"
                }
            ],
            disciplines: [
                {
                    disciplineID: 107,
                    name: "Intel",
                    yearsOfExp: "10+",
                    skills: [
                        "Deception"
                    ]
                }],
            positions: [
                {
                    positionID: 1,
                    positionName: "Analyst",
                    projectTitle: "Martensville Athletic Pavillion",
                    disciplineName: "Logistical Operations and Mental Health Analysis",
                    projectedMonthlyHours: {
                        "2020-01-01)": 30,
                        "2020-02-01": 50,
                        "2020-03-01": 50
                     }
                    
                }
            ]
        }, {
            userSummary: {
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
                    disciplineID: 102,
                    discipline: "Reconnaisance",
                    yearsOfExp: "10+"
                },
                isConfirmed: false
            },
            currentProjects: [],
            availability: [],
            disciplines: [],
            positions: []
        }
    ],
    userProfile: {
        userSummary: {
            userID: 100,
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
    },
    projectProfile: {
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
    },
    masterlist: {
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
};
