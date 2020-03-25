import React,{ Component } from 'react';
import './ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';
import 'bootstrap/dist/css/bootstrap.min.css';
import {Button} from "@material-ui/core";



class TeamRequirements extends Component {
    state = {
      opening: {
          positionID: 0,
          discipline: null,
          skills: [],
          yearsOfExp: null,
          commitmentMonthlyHours: null,
      },
        months: []
    };

    componentDidMount() {
        const months = this.createMonthsList(new Date(this.props.startDate), new Date(this.props.endDate));
        this.setState({
            months
        })
    }

    componentDidUpdate(prevProps) {
        if(prevProps.startDate !== this.props.startDate || prevProps.endDate !== this.props.endDate){
            const months = this.createMonthsList(this.props.startDate, this.props.endDate);
            this.setState({
                months
            })
        }
    }
    createMonthsList = (start, end) => {
        if(start && end){
            start = new Date(start)
            const monthNames = ["January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"
                ];
            const months = [];
            let startIndex = start.getMonth();
            let startYear = start.getYear()
            const endIndex = end.getMonth();
            const endYear = end.getYear()
            while((startIndex <= endIndex || startYear < endYear) && !(startYear > endYear)){
                const year = 1900 + startYear
                months.push(monthNames[startIndex] + " " + year)
                startIndex++;
                if(startIndex > 11){
                    startIndex = 0;
                    startYear++;
                }
            }
            return months;
        }
        return [];        
    }

    handleChange = (e) => {
      if (e.target.id === "yearsOfExp") {
          this.setState({
              opening: {
                ...this.state.opening,
              yearsOfExp: e.target.value
             }
            })
      } else {
          this.setState({
              opening: {
                  ...this.state.opening,
                  discipline: e.target.value
              }
          })
      }
    };

    handleChangeSkills = (e) => {
        if (e){
          var skills_array = e.map(function (e) { return e.label; });
            this.setState({
              opening: {
                ...this.state.opening,
                skills: skills_array
              }
           })
        }
       };

    handleSubmit = (e) => {
      e.target.elements.discipline.value = "DEFAULT";
      e.target.elements.yearsOfExp.value = "DEFAULT";
      if (e.target.elements.commitmentMonthlyHours) {
        e.target.elements.commitmentMonthlyHours.value = null;
      }
      e.preventDefault();
      var isUserPage = this.props.isUserPage;
      const opening = this.state.opening
        if(opening.discipline === null) {
            this.setState({
                ...this.state,
                error: "No Discipline Selected - Unable to add Opening"
            })
        } else if (opening.skills.length === 0){
            this.setState({
                ...this.state,
                error: "No Skill Selected - Unable to add Opening"
            })
        } else if (opening.yearsOfExp === null){
            this.setState({
                ...this.state,
                error: "No Years of Experience Selected - Unable to add Opening"
            })
        } else if (opening.commitmentMonthlyHours === null && !isUserPage){
            this.setState({
                ...this.state,
                error: "No monthly time commitments added - Please enter and estimate of how many hours are required each month"
            })
        } else {
            this.props.addOpening(this.state.opening);
            this.setState({
                ...this.state,
                opening: {
                    positionID: 0,
                    discipline: null,
                    skills: [],
                    yearsOfExp: null,
                    commitmentMonthlyHours: null
                },
                error: ""
            })
        }
    };

    changeCommitment = (date, e) => {
        let commitment = this.state.opening.commitmentMonthlyHours !== null ? this.state.opening.commitmentMonthlyHours : {};
        commitment[date] = e.target.value;
        this.setState({
            ...this.state,
            opening: {
                ...this.state.opening,
                  commitmentMonthlyHours: commitment
            }
        });
    }

    createMonthsDiv = (months) => {
        let val = 0;
        const monthArr = []
        months.forEach(month => {
            if(this.state.opening.commitmentMonthlyHours !== null) {
                val = this.state.opening.commitmentMonthlyHours[month];
            }
            monthArr.push(
            <div className="entry" key={monthArr.length} >
                <p className="label">{month}</p>
                <input type="number" name="name" placeholder="hours" value={val} onChange={(e) => this.changeCommitment(month, e)}/>
            </div>
            )
        })
        return monthArr;
    }

  render(){
    const {disciplines} = this.props;
    var isUserPage = this.props.isUserPage;
    let monthDiv = null;
    if(!isUserPage){
        monthDiv = this.createMonthsDiv(this.state.months);
    }
    var yearsOfExperience = this.props.masterYearsOfExperience;

    var discipline_render = [];
    var all_disciplines_keys = disciplines ? Object.keys(disciplines) : [];
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    if (this.state.opening.discipline){
      skills = disciplines[this.state.opening.discipline].skills;
      var skill_format = [];
      var skill_keys = [];
      skills.forEach((skill, i) => {
        var single_skill = {};
        single_skill['label'] = skill;
        single_skill['value'] = skill;
        skill_format.push(single_skill);
        skill_keys.push('skills_' + i);

      })
    }

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });


    var header = [];
    if (isUserPage) {
        header.push(<h2 key={header.length} className="darkGreenHeader">Disciplines & Skills</h2>);
    } else {
        header.push(<h2 key={header.length} className="darkGreenHeader">Team Requirements</h2>);
    }

    return (
      <div className="form-section">
          { header }
          <form onSubmit={this.handleSubmit}>
              <div className="form-section opening">
                  <div className="form-row">
                      <select className="input-box" defaultValue={'DEFAULT'}
                              id="discipline" onChange={this.handleChange}>
                          <option value={'DEFAULT'} disabled>Discipline</option>
                          {discipline_render}
                      </select>
                      <Select id="skills" key={skill_keys} className="input-box" onChange={this.handleChangeSkills} options={skill_format} isMulti
                        placeholder='Skills' />
                  </div>
                  <label className="form-row" htmlFor= "yearsOfExp">
                      <p className="form-label">Years of Experience</p>
                      <select className="input-box" defaultValue={'DEFAULT'}
                              id="yearsOfExp" onChange={this.handleChange}>
                          <option value="DEFAULT" disabled>Select a range</option>
                          {range_render}
                      </select>
                  </label>
                  {!isUserPage && (
                    <div className="flex">{monthDiv}</div>
                  )}
                  <div className="form-row">
                      <Button type="submit" variant="contained"
                              style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                              disableElevation>Add to list</Button>
                  </div>
              </div>
          </form>
          <p className="errorMessage">{this.state.error}</p>
      </div>
    );
}
}

export default TeamRequirements;
