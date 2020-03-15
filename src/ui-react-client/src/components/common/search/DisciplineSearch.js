import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';
import Select from 'react-select';


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
    updateDisciplines = (name, skills) => {
        this.state.disciplines.name = name;
        this.state.disciplines.skills = skills;
    }

  render(){
    var disciplines = this.props.disciplines;

    var discipline_render = [];
    var all_disciplines_keys = Array.from(Object.keys(disciplines));
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    if (this.state.disciplines.name){
      console.log(this.props.disciplines[this.state.disciplines.name]["skills"]);
      skills =this.props.disciplines[this.state.disciplines.name]["skills"];
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

        return (
        <div className="form-section">
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
        </div>
        </div>
        );
    }
}

export default DisciplineSearch;
