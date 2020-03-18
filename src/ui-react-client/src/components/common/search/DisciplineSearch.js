import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import Select from 'react-select';


class DisciplineSearch extends Component {
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
          this.setState({
            disciplines: {
                  ...this.state.disciplines,
                  name: e.target.value,
                  skills: [],
              }
          }, () =>  this.props.addDisciplines(this.state));
    };

    handleChangeSkills = (e) => {
        if (e){
          var skills_array = e.map(function (e) { return e.label; });
            this.setState({
              disciplines: {
                ...this.state.disciplines,
                skills: skills_array
              }
           }, () => this.props.addDisciplines(this.state))
          }
       };

  render(){
    var disciplines = this.props.disciplines;

    var discipline_render = [];
    var all_disciplines_keys = Object.keys(disciplines);
    all_disciplines_keys.forEach((discipline, i) => {
        discipline_render.push(<option key={"discipline_" + i} value={discipline}>{discipline}</option>)
    });

    var skills = [];
    if (this.state.disciplines.name){
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
