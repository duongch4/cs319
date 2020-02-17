import React,{ Component } from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";


class TeamRequirements extends Component {
    state = {
      opening: {
        openingID: 1,
        discipline: {
          name: null,
          skills: [],
          yearsOfExperience: null,
        },
        commitment: null
      }
    }

    handleChange = (e) => {
      if (e.target.id == "commitment"){
          this.setState({ opening: { ...this.state.opening, commitment: e.target.value} });
      }
      else if (e.target.id == "skills"){
        var skills_arr = [...this.state.opening.discipline.skills, e.target.value];
          this.setState({
            opening: {
              ...this.state.opening,
            discipline: {
              ...this.state.opening.discipline,
              skills: skills_arr
            }
           }
         });
      }
    else{
      this.setState({
          opening: {
            ...this.state.opening,
          discipline: { ...this.state.opening.discipline, [e.target.id]: e.target.value}
         }
        })
    }
    }

    handleSubmit = (e) =>{
      e.preventDefault();
      this.props.addOpening(this.state.opening)
      this.setState({
          opening: {
            openingID: this.state.opening.openingID + 1,
            discipline: {
              name: null,
              skills: [],
              yearsOfExperience: null,
            },
            commitment: null
          }
      })
    }

    componentDidMount(){
// db call to get the list of disciplines and skills

}

  render(){
    var disciplines = this.props.disciplines;
    var yearsOfExperience = this.props.masterYearsOfExperience;
    var isUserPage = this.props.isUserPage;

    var discipline_render = [];
    var all_disciplines_keys = Array.from(disciplines.keys());
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    })

    var skills = [];
    var skill_render = [];
    if (this.state.opening.discipline.name === null){
      skill_render = <option disabled>Please select a discipline</option>
    }
    else {
      skills = disciplines.get(this.state.opening.discipline.name);
      skills.forEach((skill, i) => {
          skill_render.push(<option key={"skills_" + i} value={skill}>{skill}</option>)
      })
    }

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });

    var header = [];
    if (isUserPage) {
        header.push(<h2 className="darkGreenHeader">Disciplines & Skills</h2>);
    } else {
        header.push(<h2 className="darkGreenHeader">Team Requirements</h2>);
    }

    return (
      <div className="form-section">
          { header }
          <form onSubmit={this.handleSubmit}>
              <div className="form-row">
                  <input className="add" type="submit" value="+"/>
                  <div className="form-section opening">
                      <div className="form-row">
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="name" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Discipline</option>
                              {discipline_render}
                          </select>
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="skills" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Skills</option>
                              {skill_render}
                          </select>
                      </div>
                      <label className="form-row" htmlFor= "yearsOfExperience">
                          <p className="form-label">Years of Experience</p>
                          <select className="input-box" defaultValue={'DEFAULT'}
                                  id="yearsOfExperience" onChange={this.handleChange}>
                              <option value="DEFAULT" disabled>Select a range</option>
                              {range_render}
                          </select>
                      </label>
                      {!isUserPage && (
                          <div className="form-row">
                              <label htmlFor= "commitment">
                                  <p className="form-label">Expected Hourly Commitment Per Month</p>
                              </label>
                              <input className="input-box" type = "text"
                                     id="commitment" onChange={this.handleChange}/>
                          </div>
                      )}
                  </div>
              </div>
          </form>
      </div>
    );
}
};

export default TeamRequirements;
