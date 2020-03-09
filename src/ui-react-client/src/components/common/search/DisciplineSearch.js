import React,{ Component } from 'react';
import '../../projects/ProjectStyles.css';
import "react-datepicker/dist/react-datepicker.css";
import { ReactDOM } from 'react-dom';


class DisciplineSearch extends Component {
    constructor(props){
        // this.addDisciplines = this.addDisciplines.bind(this);
        super();
      }

    state = {
      disciplines: [
        {
        name: null,
        skills: [],
        yearsOfExp: null,
        },
    ],
      disciplinesAvailable: {
          count: 1,
      },
      skills_arr: [],
    };

    handleChange = (e) => {
       if (e.target.id === "skills") {
        var skills_arr = [...this.state.disciplines[0].skills, e.target.value];
          this.setState({
            disciplines: [{
              ...this.state.disciplines[0],
              skills: skills_arr
            }]
         });
      } else if (e.target.id === "yearsOfExp") {
          this.setState({
            disciplines: [{
                ...this.state.disciplines[0],
              yearsOfExp: e.target.value
             }]
            })
      } else {
           var skills_arr = [...this.state.disciplines[0].skills, e.target.value];
          this.setState({
            disciplines: [{
                  ...this.state.disciplines[0],
                  name: e.target.value
              }]
          })
      }
    };

    closeDiscipline(id) {
        // const disciplinesNew = [...this.state.disciplines];
        // console.log(disciplinesNew);
        // this.setState({
        //     ...this.state,
        //     disciplinesAvailable: {
        //         count: (this.state.disciplinesAvailable.count) - 1
        //     }
        // });
        // console.log(this.state);
    };

    addDiscipline(id) {
        // const obj = {name: null,
        //     skills: [],
        //     yearsOfExp: null};

        // const disciplinesNew = [...this.state.disciplines, obj];
        // this.setState({
        //         ...this.state,
        //         disciplines: disciplinesNew,
        //         disciplinesAvailable: {
        //             count: (this.state.disciplinesAvailable.count) + 1
        //         }
        // })
        this.props.addDisciplines(this.state.disciplines[0]);     
        // console.log(this.state);
    };

    searchNotNull(myArray){
        for (var i=0; i < myArray.length; i++) {
            if (myArray[i].name != null) {
                return myArray[i];
            }
        }
    }

    handleSubmit = (e) =>{
      e.preventDefault();
    //   this.setState({
    //       ...this.state.disciplines,
    //       disciplinesAvailable: {
    //           count: (this.state.disciplinesAvailable.count) + 1
    //       }
    //   })
    //   console.log(this.state);
    };

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
    if (this.state.disciplines[0].name === null){
      skill_render = <option disabled>Please select a discipline</option>
    } else {
        skills = disciplines[this.state.disciplines[0].name];
        skills.forEach((skill, i) => {
          skill_render.push(<option key={"skills_" + i} value={skill}>{skill}</option>)
      })
    }

    var range_render = [];
    yearsOfExperience.forEach((yearsOfExperience, i) => {
        range_render.push(<option key={"yearsOfExperience_" + i} value={yearsOfExperience}>{yearsOfExperience}</option>)
    });

    var inputType = null; 
    var disciplineHTML = [];
    for (var i = 1; i <= this.state.disciplinesAvailable.count; i++) {
        var id = "disciplines_" + i;
        if (i <= 1){
            inputType = (<input className="add" type="submit" value="+" onClick={()=> this.addDiscipline("disciplines_" + i)}/>);
        } else {
            inputType = (<input className="add" type="submit" value="-" onClick={()=> this.closeDiscipline("disciplines_" + i)}/>);
        }
        console.log("i: " + i);

        disciplineHTML.push(<div className="form-row" id={id}>
        {inputType}
        <div className="form-section opening">
            <div className="form-row">
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="discipline" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Discipline</option>
                    {discipline_render}
                </select>
                <select className="input-box" defaultValue={'DEFAULT'}
                        id="skills" onChange={this.handleChange}>
                    <option value="DEFAULT" disabled>Skills</option>
                    {skill_render}
                </select>
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
        </div>);
    }
    

        return (
        <div className="form-section">
            <h2 key={1} className="darkGreenHeader">Disciplines & Skills</h2>
                <form onSubmit={this.handleSubmit}>
                    {disciplineHTML}
                </form>
        </div>
        );
    }
}

export default DisciplineSearch;
