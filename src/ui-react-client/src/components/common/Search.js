import React,{ Component } from 'react';
import { loadDisciplines } from '../../redux/actions/disciplinesActions.js';
import { createProject } from '../../redux/actions/projectsActions.js';
import { loadLocations} from '../../redux/actions/locationsActions.js';
import { loadExperiences } from '../../redux/actions/experienceActions.js';
// import {Disciplines} from '../projects/Disciplines'
import { connect } from 'react-redux';

class AddProject extends Component {
  state = {
    location: {},
    city_options: [],
    province_options: [],
    disciplines: [],
    skills: [],
    years_experience: [],
    /*
    filterType
      location: "",
      discipline: "", - what is there are multiple ones to search?
      experience: "",
      utilization: "",
      searchParam: "",
      startDate: "",
      endDate: "" ; 
    filterValue = String ; 
    */
  }

  // componentDidMount(){
  //   this.props.loadDisciplines();
  //   // this.props.disciplines holds the master disciplines Map now
  //   this.props.loadLocations();
  //   // this.props.locations hold the master locations now
  //   this.props.loadExperiences();
  //   // this.props.masterYearsOfExperience holds the master list of experiences

  //   var locations = {}

  //   this.props.locations.forEach(element =>{
  //     if(locations[element.province]){
  //       locations[element.province] = [...locations[element.province], element.city]
  //     } else {
  //       locations[element.province] = [element.city]
  //     }
  //   })

  //   this.setState({
  //     location: locations,
  //     province_options: Object.keys(locations)
  //   })
  // }

  // addOpening = (opening) => {
  //   console.log(opening.discipline)
  //   if(opening.discipline.name){
  //     const disciplines = [...this.state.disciplines, opening.discipline.name]
  //       this.setState({
  //         disciplines
  //       })
  //   }
  //   if(opening.discipline.skills.length > 0){
  //     const skills = [...this.state.skills, opening.discipline.skills]
  //       this.setState({
  //         skills
  //       })
  //   }
  //   if(opening.discipline.yearsOfExperience){
  //     const years_experience = [...this.state.years_experience, opening.discipline.yearsOfExperience]
  //       this.setState({
  //         years_experience
  //       })
  //   }
  // }

  // handleChange = (e) => {
  //   if (e.target.id == "city"){
  //     this.setState({ 
  //       city: e.target.value
  //     })
  //   }
  //   else if (e.target.id == "province"){
  //     var city_options = Object.values(this.state.location[e.target.value])
  //     this.setState({ 
  //       province: e.target.value,
  //       city_options: city_options
  //     })
  //   }
  //   else {
  //     this.setState({
  //       [e.target.id]: e.target.value
  //     })
  //   }
  // }

  onSubmit = () => {
    // TODO: Send through Redux - waiting for format from backend
  }

  render(){
    console.log(this.state)
    var city_render = [];
    this.state.city_options.forEach((city, i) => {
        city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
    })

    var province_render = [];
    this.state.province_options.forEach((province, i) => {
        province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
    })
    return (
      <div>
        <label htmlFor= "location">
            Location
            <select defaultValue={'DEFAULT'} id="province" onChange={this.handleChange}>
              <option value="DEFAULT" disabled>{this.state.location.province}</option>
              {province_render}
            </select>
            <select defaultValue={'DEFAULT'} id="city" onChange={this.handleChange}>
              <option value="DEFAULT" disabled>{this.state.location.city}</option>
              {city_render}
            </select>

            </label>
            <loadDisciplines 
              disciplines={this.props.loadDisciplines}
              masterYearsOfExperience={this.props.masterYearsOfExperience}
              addOpening={(opening) => this.addOpening(opening)}
              search={true}/>
        <button onClick={() => this.onSubmit()}>Search</button>
      </div>
    );
  }
};

const mapStateToProps = state => {
  return {
    disciplines: state.disciplines,
    locations: state.locations,
    masterYearsOfExperience: state.masterYearsOfExperience,
  };
};

const mapDispatchToProps = {
  loadDisciplines,
  loadLocations,
  createProject,
  loadExperiences,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(AddProject);
