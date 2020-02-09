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

    handleChangeDate = (date) => {
      this.setState({
        start_date: date
      })
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

    var discipline_render = [];
    var all_disciplines_keys = Array.from(disciplines.keys());
    all_disciplines_keys.forEach((discipline) => {
        discipline_render.push(<option value={discipline}>{discipline}</option>)
    })

    var skills = [];
    var skill_render = [];
    if (this.state.opening.discipline.name === null){
      skill_render = <option disabled>Please select a discipline</option>
    }
    else {
      skills = disciplines.get(this.state.opening.discipline.name);
      skills.forEach((skill) => {
          skill_render.push(<option value={skill}>{skill}</option>)
      })
    }

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience) => {
        range_render.push(<option value={yearsOfExperience}>{yearsOfExperience}</option>)
    })

    return (
      <div>
          <h4 className="darkGreenHeader">Team Requirements</h4>

          <form onSubmit={this.handleSubmit}>
          <select id="name" onChange={this.handleChange}>
          <option selected disabled>Discipline</option>
            {discipline_render}
          </select>

          <select id="skills" onChange={this.handleChange}>
            <option selected disabled>Skills</option>
            {skill_render}
          </select>

            <label htmlFor= "yearsOfExperience">
            Years of Experience
            <select id="yearsOfExperience" onChange={this.handleChange}>
              <option selected disabled>Select a range</option>
              {range_render}
            </select>
            </label>

            <label htmlFor= "commitment">Expected Hourly Commitment Per Month</label>
            <input type = "text" id="commitment" onChange={this.handleChange}/>

            <input type="submit" value="submit"/>

            </form>
      </div>
    );
}
};

export default TeamRequirements;
