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
    }

    handleChange = (e) => {
      if (e.target.id == "city"){
          this.setState({ location: { ...this.state.location, city: e.target.value}
        }, () => this.props.addProjDetails(this.state))
      }
      else if (e.target.id == "province"){
        this.setState({ location: { ...this.state.location, province: e.target.value}
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

    handleSubmit = (e) =>{
      e.preventDefault();
      this.setState({
            name: null,
            location: {city: null, province: null},
            startDate: new Date(),
            endDate: new Date(),
      })
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
    var cities = [];
    var provinces = [];
    var locations_map = this.props.locations;

    locations_map.forEach(element =>{
      cities.push(element["city"])
      provinces.push(element["province"])
    })

    var city_render = [];
    cities.forEach((city, i) => {
        city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
    })

    var province_render = [];
    provinces.forEach((province, i) => {
        province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
    })

    return (
      <div className="form-section">
          <h2 className="darkGreenHeader">Project Details</h2>
            <div className="form-section">
                <div className="form-row">
                    <label htmlFor= "name">Name</label>
                    <input type = "text" id="name" onChange={this.handleChange} value= {this.state.name}/>
                </div>
                <div className="form-row">
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
                </div>
                <label htmlFor= "project_duration">
                Project Duration
                <DatePicker id="startDate" selected={this.state.startDate} onChange={this.handleChangeStartDate} />
                <DatePicker id="endDate" selected={this.state.endDate} onChange={this.handleChangeEndDate} />
                </label>
                <input type="submit" value="submit"/>
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
