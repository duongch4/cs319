export default {
  userSummaries: [
      {
          firstName: "Jason",
          lastName: "Bourne",
          userID: 100,
          location: {
              province: "British Columbia",
              city: "Vancouver"
          },
          utilization: 100,
          resourceDiscipline: {
              discipline: "Intel",
              yearsOfExp: "3-5"
          },
          isConfirmed: true
      }, {
          firstName: "Nicky",
          lastName: "Parsons",
          userID: 101,
          location: {
              province: "British Columbia",
              city: "Vancouver"
          },
          utilization: 75,
          resourceDiscipline: {
              discipline: "Logistical Operations and Mental Health Analysis",
              yearsOfExp: "3-5"
          },
          isConfirmed: true
      }, {
          firstName: "Pamela",
          lastName: "Landy",
          userID: 102,
          location: {
              province: "Ontario",
              city: "Toronto"
          },
          utilization: 100,
          resourceDiscipline: {
              discipline: "Reconnaisance",
              yearsOfExp: "10+"
          },
          isConfirmed: false
      }],
  projectSummaries: [{
      title: "Martensville Athletic Pavillion",
      location: {
          province: "Seskatchewan",
          city: "Saskatoon"
      },
      projectStartDate: "2020-10-31T00:00:00.0000000",
      projectEndDate: "2021-12-31T00:00:00.0000000",
      projectNumber: "2009-VD9D-15"
  }, {
      title: "University of British Columbia Science Building",
      location: {
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
                      province: "British Columbia",
                      city: "Vancouver"
                  },
                  utilization: 100,
                  resourceDiscipline: {
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
                      province: "Ontario",
                      city: "Toronto"
                  },
                  utilization: 100,
                  resourceDiscipline: {
                      discipline: "Reconnaisance",
                      yearsOfExp: "10+"
                  },
                  isConfirmed: false
              }
          ],
          openings: [
              {
                  discipline: "Environmental Design",
                  skills: [
                      "skill1", "skill2"
                  ],
                  yearsOfExp: "1-3",
                  commitmentMonthlyHours: 160
              },
              {
                  discipline: "Waste Management",
                  skills: [],
                  yearsOfExp: "1-3",
                  commitmentMonthlyHours: 160
              }
          ]
      }, {
          projectSummary: {
              title: "University of British Columbia Science Building",
              location: {
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
                  province: "British Columbia",
                  city: "Vancouver"
              },
              utilization: 100,
              resourceDiscipline: {
                discipline: "Logistical Operations and Mental Health Analysis",
                yearsOfExp: "3-5"
              },
              isConfirmed: false
          },
          currentProjects: [
              {
                  title: "Martensville Athletic Pavillion",
                  location: {
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
                  name: "Intel",
                  yearsOfExperience: "10+",
                  skills: [
                      "Deception"
                  ]
              },
              {
                  name: "Language",
                  yearsOfExperience: "10+",
                  skills: [
                      "Russian"
                  ]
              }
          ]
      }, {
      userSummary: {
        firstName: "Nicky",
        lastName: "Parsons",
        userID: 101,
        location: {
          province: "British Columbia",
          city: "Vancouver"
        },
        utilization: 75,
        resourceDiscipline: {
          discipline: "Logistical Operations and Mental Health Analysis",
          yearsOfExp: "3-5"
        },
        isConfirmed: true
      },
      currentProjects: [
        {
          title: "Martensville Athletic Pavillion",
          location: {
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
          name: "Intel",
          yearsOfExperience: "10+",
          skills: [
            "Deception"
          ]
        }]
    }, {
      userSummary: {
        firstName: "Pamela",
        lastName: "Landy",
        userID: 102,
        location: {
          province: "Ontario",
          city: "Toronto"
        },
        utilization: 100,
        resourceDiscipline: {
          discipline: "Reconnaisance",
          yearsOfExp: "10+"
        },
        isConfirmed: false
      },
      currentProjects: [],
      availability: [],
      disciplines: []
    }
  ],
  masterlist: {
      disciplines: {
          "Environmental Design": [
              "Skill 1",
              "Skill 2"
          ],
          "Waste Management": [
              "Skill A",
              "Skill B"
          ],
          "Parks and Recreation" : [
              "Skill A1",
              "Skill B2"
          ]
      },
      locations: {
          "British Columbia": [
              "Vancouver",
              "Richmond"
          ],
          "Seskatchewan": [
              "Saskatoon"
          ],
          "Ontario": [
              "Toronto"
          ]
      },
      yearsOfExp: [
          "3-5",
          "10+",
          "1-3"
      ]
  }
};
