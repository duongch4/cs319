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
      if (e != null) {
        this.setState({
          disciplines: {
                name: e.value,
                skills: [],
            }
        }, () =>  this.props.addDisciplines(this.state));
      } else {
        this.setState({
          disciplines: {
                name: null,
                skills: [],
            }
        }, () =>  this.props.addDisciplines(this.state));
      }  
    };

    handleChangeSkills = (e) => {
      if (e != null && e.length != 0){
          var skills_array = e.map(function (e) { return e.label; });
            this.setState({
              disciplines: {
                ...this.state.disciplines,
                skills: skills_array
              }
           }, () => this.props.addDisciplines(this.state))
          } else {
            this.setState({
              disciplines: {
                ...this.state.disciplines,
                skills: []
              }
           }, () => this.props.addDisciplines(this.state))
          }
       };

  render(){
    var disciplines = this.props.disciplines;

    var discipline_render = [];
    var discipline_key = [];
    Object.keys(disciplines).forEach((discipline, i) => {
      discipline_key.push("discipline_" + i);
      var discipline_obj = {label: discipline, value: discipline};
      discipline_render.push(discipline_obj);
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
        <div className="form-row">
            <Select placeholder='Disciplines' id="disciplines" className="input-box"
                    onChange={this.handleChange} options={discipline_render} isClearable/>
            <Select id="skills" key={skill_keys} className="input-box"
                    onChange={this.handleChangeSkills} options={skill_format} isMulti isClearable
                    placeholder='Skills' />
        </div>
      )
  }
}

export default DisciplineSearch;
