import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';


class DisciplineSearch extends Component {
    constructor(props){
        super(props);
      }

    state = {
      key: this.props.keyName,
      disciplines: 
        {
        name: null,
        skills: [],
        yearsOfExp: null,
        },
      skills_arr: [],
    };

    handleChange = (e) => {
        if (e.target.id === "skills") {
        var skills_arr = [e.target.value];
          this.setState({
            disciplines: {
              ...this.state.disciplines,
              skills: skills_arr
            }
         }, this.updateSkill(e.target.value));
      } else if (e.target.id === "yearsOfExp") {
          this.setState({
            disciplines: {
                ...this.state.disciplines,
              yearsOfExp: e.target.value
             }
            }, this.updateYears(e.target.value));
      } else {
           var skills_arr = [...this.state.disciplines.skills, e.target.value];
          this.setState({
            disciplines: {
                  ...this.state.disciplines,
                  name: e.target.value,
                  skills: [],
              }
          },this.updateDisciplines(e.target.value, []));
      }
    this.props.addDisciplines(this.state);
    };

    updateSkill = (val) => {
        this.state.disciplines.skills = [val];
    }
    updateYears = (val) => {
        this.state.disciplines.yearsOfExp = val;
    }

    updateDisciplines = (name, skills) => {
        this.state.disciplines.name = name;
        this.state.disciplines.skills = skills;
    }

  render(){
    var disciplines = this.props.disciplines;
    var yearsOfExperience = this.props.masterYearsOfExperience;

    var discipline_render = [];
    var all_disciplines_keys = Array.from(Object.keys(disciplines));
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    var skill_render = [];
    if (this.state.disciplines.name === null){
      skill_render = <option disabled>Please select a discipline</option>
    } else {
        skills = disciplines[this.state.disciplines.name];
        skills.forEach((skill, i) => {
          skill_render.push(<option key={"skills_" + i} value={skill}>{skill}</option>)
      })
    }

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });

        return (
        <div className="form-section">
                <div className="form-section opening">
            <div className="form-row">
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="discipline" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Discipline</option>
                    {discipline_render}
                </select>


                {(this.state.disciplines.skills.length == 0) && 
                <select className="input-box" defaultValue={'DEFAULT'} value="DEFAULT"
                id="skills" onChange={this.handleChange}>
            <option value="DEFAULT" disabled>Skills</option>
            {skill_render}
             </select>}
                {(this.state.disciplines.skills.length  !== 0) && 
               <select className="input-box" defaultValue={'DEFAULT'} value={this.state.disciplines.skills[0]}
               id="skills" onChange={this.handleChange}>
                <option value="DEFAULT" disabled>Skills</option>
            {skill_render}
            </select>}

                
            </div>
            <label className="form-row" htmlFor= "yearsOfExp">
                <p className="form-label">Years of Experience</p>
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="yearsOfExp" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Select a range</option>
                    {range_render}
                </select>
            </label>
        </div>
        </div>
        );
    }
}

export default DisciplineSearch;
