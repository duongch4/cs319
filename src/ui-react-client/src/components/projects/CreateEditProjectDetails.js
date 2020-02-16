import React,{ Component } from 'react';
import './ProjectStyles.css';
import PropTypes from 'prop-types';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";
import {TextField, FormControl, Select, InputLabel, MenuItem} from "@material-ui/core";

class CreateEditProjectDetails extends Component {
    state = {
      projID: 1,
      name: null,
      location: {city: null, province: null},
      startDate: new Date(),
      endDate: new Date(),
    }

    handleChange = (e) => {
      if (e.target.id == "city"){
          this.setState({ location: { ...this.state.location, city: e.target.value}
        })
      }
      else if (e.target.id == "province"){
        this.setState({ location: { ...this.state.location, province: e.target.value}
        })
      }
      else {
        this.setState({
          [e.target.id]: e.target.value
        })
      }
    }

    handleChangeStartDate = (date) => {
      this.setState({
        startDate: date
      })
    }

    handleChangeEndDate = (date) => {
      this.setState({
        endDate: date
      })
    }

    handleSubmit = (e) =>{
      e.preventDefault();
      this.props.addProjDetails(this.state)
      this.setState({
            projID: this.state.projID + 1,
            name: null,
            location: {city: null, province: null},
            startDate: new Date(),
            endDate: new Date(),
      })
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
    cities.forEach((city) => {
        city_render.push(<option value={city}>{city}</option>)
    })

    var province_render = [];
    provinces.forEach((province) => {
        province_render.push(<option value={province}>{province}</option>)
    })

    var province_render2 = [];
    provinces.forEach((province) => {
        province_render2.push(<MenuItem value={province}>{province}</MenuItem>)
    })

    //<TextField label="Title" fullWidth inputLabelProps={{shrink: true}}/>
    return (
      <div className="form-section">
          <h2 className="darkGreenHeader">Project Details</h2>
          <form onSubmit={this.handleSubmit}>
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
          </form>
      </div>
    );
}
};

CreateEditProjectDetails.propTypes = {
  addOpening: PropTypes.object.isRequired,
};

export default CreateEditProjectDetails;
