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
    utilization: 0,
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
    utilization: 0,
    location: "Europe",
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
      }]
    },
    {
      projID: 1234,
      name: "Demo - Item 2",
      location: {city: "Vancouver", province: "BC"},
      startDate: 9876,
      endDate:6789,
  },
  {
    projID: 1235,
    name: "Demo - Item 3",
    location: {city: "Vancouver", province: "BC"},
    startDate: 9876,
    endDate:6789,
}

  ],
  locations: [
    {
      city: "Vancouver", 
      province: "BC"
    }
  ],
};
