import React,{ Component } from 'react';
import './ProjectStyles.css';
import PropTypes from 'prop-types';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

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

    return (
      <div>
          <h4 className="darkGreenHeader">Project Details</h4>

          <form onSubmit={this.handleSubmit}>
            <label htmlFor= "name">Name</label>
            <input type = "text" id="name" onChange={this.handleChange}/>

            <label htmlFor= "location">
            Location
            <select id="province" onChange={this.handleChange}>
              <option selected disabled>Province</option>
              {province_render}
            </select>
            <select id="city" onChange={this.handleChange}>
              <option selected disabled>City</option>
              {city_render}
            </select>

            </label>

            <label htmlFor= "project_duration">
            Project Duration
            <DatePicker id="startDate" selected={this.state.startDate} onChange={this.handleChangeStartDate} />
            <DatePicker id="endDate" selected={this.state.endDate} onChange={this.handleChangeEndDate} />
            </label>

            <input type="submit" value="submit"/>

            </form>
      </div>
    );
}
};

CreateEditProjectDetails.propTypes = {
  addOpening: PropTypes.object.isRequired,
};

export default CreateEditProjectDetails;
