import React,{ Component } from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";


class TeamRequirements extends Component {
    state = {
      discipline: null,
      skills: null,
      years_of_experience: null,
      expected_hourly_commitment_per_month: null
    }

    handleChange = (e) => {
      this.setState({
        [e.target.id]: e.target.value
      })
    }

    handleChangeDate = (date) => {
      this.setState({
        start_date: date
      })
    }

    handleSubmit = (e) =>{
      e.preventDefault();
    }

  render(){
    let all_disciplines = new Map();
    all_disciplines.set("Discipline1", ["Skill_1", "Skill_2", "Skill_3"]);
    all_disciplines.set("Discipline2", ["Skill_A", "Skill_B", "Skill_C"]);

    var discipline_render = [];
    var all_disciplines_keys = Array.from(all_disciplines.keys());
    all_disciplines_keys.forEach((discipline) => {
        discipline_render.push(<option value={discipline}>{discipline}</option>)
    })

    var skills = [];
    var skill_render = [];
    if (this.state.discipline === null){
      skill_render = <option disabled>Please select a discipline</option>
    }
    else {
      skills = all_disciplines.get(this.state.discipline);
      skills.forEach((skill, i) => {
        var option_value = "skill" + "_" + i;
          skill_render.push(<option value={option_value}>{skill}</option>)
      })
    }
    return (
      <div>
          <h4 className="darkGreenHeader">Team Requirements</h4>

          <form onSubmit={this.handleSubmit}>
          <select id="discipline" onChange={this.handleChange}>
          <option selected disabled>Discipline</option>
            {discipline_render}
          </select>

          <select id="skills" onChange={this.handleChange}>
            <option selected disabled>Skills</option>
            {skill_render}
          </select>

            <label htmlFor= "years_of_experience">
            Years of Experience
            <select id="years_of_experience" onChange={this.handleChange}>
              <option selected disabled>Select a range</option>
              <option value="range_ex_1">range example 1</option>
              <option value="range_ex_2">range example 2</option>
            </select>
            </label>

            <label htmlFor= "expected_hourly_commitment_per_month">Expected Hourly Commitment Per Month</label>
            <input type = "text" id="expected_hourly_commitment_per_month" onChange={this.handleChange}/>

            <input type="submit" value="submit"/>

            </form>
      </div>
    );
}
};

TeamRequirements.propTypes = {
  projectDetails: PropTypes.object.isRequired,
};

export default TeamRequirements;
