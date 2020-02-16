import React,{ Component } from 'react';
import './ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";


class Disciplines extends Component {
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
          this.setState({ 
            opening: { ...this.state.opening, commitment: e.target.value} 
          }, () => 
          {
            if(this.props.search){
              this.props.addOpening(this.state.opening)
            }
          }
        );
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
         }
        , () => 
        {
          if(this.props.search){
            this.props.addOpening(this.state.opening)
          }
        }
      );
      }
      else{
        this.setState({
            opening: {
              ...this.state.opening,
            discipline: { ...this.state.opening.discipline, [e.target.id]: e.target.value}
          }
        }, () => 
        {
          if(this.props.search){
            this.props.addOpening(this.state.opening)
          }
        }
      )
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

  render(){
    var disciplines = this.props.disciplines;
    var yearsOfExperience = this.props.masterYearsOfExperience;

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
    })

    if(this.props.search){
      return (
        <div>
            <form onSubmit={this.handleSubmit}>
              <select defaultValue={'DEFAULT'} id="name" onChange={this.handleChange}>
              <option value="DEFAULT" disabled>Discipline</option>
                {discipline_render}
              </select>
    
              <select defaultValue={'DEFAULT'} id="skills" onChange={this.handleChange}>
                <option value="DEFAULT" disabled>Skills</option>
                {skill_render}
              </select>
    
                <label htmlFor= "yearsOfExperience">
                  Years of Experience
                  <select defaultValue={'DEFAULT'} id="yearsOfExperience" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Select a range</option>
                    {range_render}
                  </select>
                </label>
              </form>
        </div>
      );
    }
    return (
      <div>
          <h4 className="darkGreenHeader">Team Requirements</h4>

          <form onSubmit={this.handleSubmit}>
          <select defaultValue={'DEFAULT'} id="name" onChange={this.handleChange}>
          <option value="DEFAULT" disabled>Discipline</option>
            {discipline_render}
          </select>

          <select defaultValue={'DEFAULT'} id="skills" onChange={this.handleChange}>
            <option value="DEFAULT" disabled>Skills</option>
            {skill_render}
          </select>

            <label htmlFor= "yearsOfExperience">
            Years of Experience
            <select defaultValue={'DEFAULT'} id="yearsOfExperience" onChange={this.handleChange}>
              <option value="DEFAULT" disabled>Select a range</option>
              {range_render}
            </select>
            </label>

            <label htmlFor= "commitment">Expected Hourly Commitment Per Month</label>
            <input type = "text" id="commitment" onChange={this.handleChange}/>

            <input type="submit" value="+"/>

            </form>
      </div>
    );
}
};

export default Disciplines;
