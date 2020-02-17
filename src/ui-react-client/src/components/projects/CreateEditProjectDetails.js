import React,{ Component } from 'react';
import './ProjectStyles.css';
import PropTypes from 'prop-types';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class CreateEditProjectDetails extends Component {
    state = {
      name: "",
      location: {city: "City", province: "Province"},
      startDate: new Date(),
      endDate: new Date(),
      city_options: [],
      province_options: [],
      location_options: {},
    }

    componentDidMount(){
    var locations = {}

    this.props.locations.forEach(element =>{
      if(locations[element.province]){
        locations[element.province] = [...locations[element.province], element.city]
      } else {
        locations[element.province] = [element.city]
      }
    })

    this.setState({
      location: locations,
      province_options: Object.keys(locations),
      location_options: this.props.locations
    })
  }

    handleChange = (e) => {
      if (e.target.id == "city"){
          this.setState({ location: { ...this.state.location, city: e.target.value}
        }, () => this.props.addProjDetails(this.state))
      }
      else if (e.target.id == "province"){
        var all_locations = {};
        var city_options = [];

        if (this.state.location_options){
          this.props.locations.forEach(element =>{
            if(all_locations[element.province]){
              all_locations[element.province] = [...all_locations[element.province], element.city]
            } else {
              all_locations[element.province] = [element.city]
            }
          })
          var city_options = Object.values(all_locations[e.target.value])
        }
        else{
          var city_options = Object.values(this.state.location[e.target.value])
        }
        this.setState({ location: { ...this.state.location, province: e.target.value},
          city_options: city_options
        }, () => this.props.addProjDetails(this.state))
      }
      else {
        this.setState({
          [e.target.id]: e.target.value
        }, () => this.props.addProjDetails(this.state))
      }
    }

    handleChangeStartDate = (date) => {
      this.setState({
        startDate: date
      }, () => this.props.addProjDetails(this.state))
    }

    handleChangeEndDate = (date) => {
      this.setState({
        endDate: date
      }, () => this.props.addProjDetails(this.state))
    }

    componentWillReceiveProps(existing_project){
      if (existing_project.currentProject){
        var project = existing_project.currentProject.project;
        this.setState({
          projID: project.projID,
          name: project.name,
          location: project.location,
          startDate: project.startDate,
          endDate: project.endDate,
        })
      }
    }

  render(){
    var city_render = [];
    this.state.city_options.forEach((city, i) => {
        city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
    })

    var province_render = [];
    this.state.province_options.forEach((province, i) => {
        province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
    })

    return (
      <div className="form-section">
          <h2 className="darkGreenHeader">Project Details</h2>
            <div className="form-section">
                <div className="form-row">
                    <label htmlFor= "name"><p className="form-label">Name</p></label>
                    <input className="input-box" type = "text" id="name" onChange={this.handleChange} value= {this.state.name}/>
                </div>
                <label htmlFor= "location" className="form-row">
                    <p className="form-label">Location</p>
                    <select className="input-box" defaultValue={'DEFAULT'} id="province" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{this.state.location.province}</option>
                        {province_render}
                    </select>
                    <select className="input-box" defaultValue={'DEFAULT'} id="city" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{this.state.location.city}</option>
                        {city_render}
                    </select>
                </label>
                <label htmlFor= "project_duration" className="form-row">
                    <p className="form-label">Project Duration</p>
                <DatePicker className="input-box" id="startDate" selected={this.state.startDate} onChange={this.handleChangeStartDate} />
                <DatePicker className="input-box" id="endDate" selected={this.state.endDate} onChange={this.handleChangeEndDate} />
                </label>
            </div>
      </div>
    )
}
}

CreateEditProjectDetails.propTypes = {
  addOpening: PropTypes.object,
  currentProject: PropTypes.object
}

export default CreateEditProjectDetails;
