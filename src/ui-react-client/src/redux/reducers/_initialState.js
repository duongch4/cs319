export default {
  users: [{
    userID: 123,
    name: "Jason Bourne",
    discipline: {
      name: "Covert Spy",
      skills: ["skill 1", "skill 2"],
      yearsOfExperience: "15+"
    },
    position: "Rogue",
    utilization: 80,
    location: {city: "Vancouver", province: "BC"},
  }, {
    userID: 432,
    name: "Nicky Parsons",
    discipline: {
      name: "Logistical Operations and Mental Health Analysis",
      skills: ["skill 1", "skill 2"],
      yearsOfExperience: "3-5 years"
    },
    position: "Contact",
    utilization: 10,
    location: {city: "Toronto", province: "ON"},
  }],
  projects: [
    {
      projID: 1233,
      name: "Demo",
      location: {city: "Vancouver", province: "BC"},
      startDate: 9876,
      endDate:6789,
      openings: [{
        openingID: 111,
        discipline: {
          name: "Discipline: Parks and Recreation",
          skills: ["Skill 1", "Skill 2", "Skill 3"],
          yearsOfExperience: "3-5 years",
        },
        commitment: 160
      },
      {
        openingID: 112,
        discipline: {
          name: "Discipline: Environmental Planning",
          skills: ["Skill 1", "Skill 2", "Skill 3"],
          yearsOfExperience: "3-5 years",
        },
        commitment: 160
      },
      {
        openingID: 112,
        discipline: {
          name: "Discipline: Sustainable Design",
          skills: ["Skill 1", "Skill 2", "Skill 3"],
          yearsOfExperience: "3-5 years",
        },
        commitment: 160
      }],
      users: [
        {
          userID: 123,
          name: "Jason Bourne",
          discipline: {
            name: "Covert Spy",
            skills: ["skill 1", "skill 2"],
            yearsOfExperience: "15+"
          },
          position: "Rogue",
          utilization: 80,
          location: {city: "Vancouver", province: "BC"},
        }, {
          userID: 432,
          name: "Nicky Parsons",
          discipline: {
            name: "Logistical Operations and Mental Health Analysis",
            skills: ["skill 1", "skill 2"],
            yearsOfExperience: "3-5 years"
          },
          position: "Contact",
          utilization: 10,
          location: {city: "Toronto", province: "ON"},
        }
      ]
    },
    {
      projID: 1234,
      name: "Demo - Item 2",
      location: {city: "Vancouver", province: "BC"},
      startDate: 9876,
      endDate:6789,
      openings: []

  },
  {
    projID: 1235,
    name: "Demo - Item 3",
    location: {city: "Vancouver", province: "BC"},
    startDate: 9876,
    endDate:6789,
    openings: []
}
  ],
  locations: [
    {
      city: "Vancouver",
      province: "British Columbia"
    },{
      city: "Toronto",
      province: "Ontario"
    }
  ],
  disciplines: new Map([
    ["Discipline: Environmental Planning",
    ["Skill 1", "Skill 2", "Skill 3"]],
    ["Discipline: Sustainable Design",
    ["Skill A", "Skill B", "Skill C"]],
    ["Discipline: Parks and Recreation",
    ["Skill 1A", "Skill 2B", "Skill 3C"]],
  ]),
  masterYearsOfExperience: [
    "less than 1 year",
    "1-3 years",
    "3-5 years",
    "5+ years"
  ],
  usersProfile: [{
    userID: 123,
    name: "Jason Bourne",
    discipline: {
      name: "Covert Spy",
      skills: ["skill 1", "skill 2"],
      yearsOfExperience: "15+"
    },
    position: "Rogue",
    utilization: 80,
    location: {city: "Vancouver", province: "BC"},
    currentProjects: [
      {
        projID: 1233,
        name: "Demo",
        location: {city: "Vancouver", province: "BC"},
        startDate: 9876,
        endDate:6789,
        openings: [{
          openingID: 111,
          discipline: {
            name: "Discipline: Parks and Recreation",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        },
        {
          openingID: 112,
          discipline: {
            name: "Discipline: Environmental Planning",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        },
        {
          openingID: 112,
          discipline: {
            name: "Discipline: Sustainable Design",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        }]
      },
      {
        projID: 1234,
        name: "Demo",
        location: {city: "Vancouver", province: "BC"},
        startDate: 9876,
        endDate:6789,
        openings: [{
          openingID: 111,
          discipline: {
            name: "Discipline: Parks and Recreation",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        }]
      }
    ],
    availability: [
      {
        reason: "sick",
        start: '2020-02-14',
        end: '2020-02-14'
      },
      {
        reason: "sick",
        start: '2020-02-15',
        end: '2020-02-15'
      }
    ],
    disciplines: [
      {
        name: "Discipline: Sustainable Design",
        skills: ["Skill 1", "Skill 2", "Skill 3"],
        yearsOfExperience: "3-5 years",
      },
      {
        name: "Discipline: Sustainable Design",
        skills: ["Skill 1", "Skill 2", "Skill 3"],
        yearsOfExperience: "3-5 years",
      }
    ]
  }, {
    userID: 432,
    name: "Nicky Parsons",
    discipline: {
      name: "Logistical Operations and Mental Health Analysis",
      skills: ["skill 1", "skill 2"],
      yearsOfExperience: "3-5 years"
    },
    position: "Contact",
    utilization: 10,
    location: {city: "Toronto", province: "ON"},
    currentProjects: [
      {
        projID: 1233,
        name: "Demo",
        location: {city: "Vancouver", province: "BC"},
        startDate: 9876,
        endDate:6789,
        openings: [{
          openingID: 111,
          discipline: {
            name: "Discipline: Parks and Recreation",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        },
        {
          openingID: 112,
          discipline: {
            name: "Discipline: Environmental Planning",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        },
        {
          openingID: 112,
          discipline: {
            name: "Discipline: Sustainable Design",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        }]
      }
    ],
    availability: [],
    disciplines: []
  }],

};
