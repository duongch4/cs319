import React, { Component } from 'react';
import './UserStyles.css';
import PropTypes from 'prop-types';

class EditUserDetails extends Component {
    state = {
        name: this.props.userProfile.name,
        location: {
            city: this.props.userProfile.location.city,
            province: this.props.userProfile.location.province
        }
    };

    handleChange = (e) => {
        if (e.target.id == "city") {
            this.setState(
                { location: {...this.state.location, city: e.target.value}},
                () => this.props.addUserDetails(this.state));
        } else if (e.target.id == "province") {
            this.setState(
                { location: { ...this.state.location, province: e.target.value}},
                () => this.props.addUserDetails(this.state));
        } else {
            this.setState(
                { [e.target.id]: e.target.value },
                () => this.props.addUserDetails(this.state));
        }
    };

    render() {
        var cities = [];
        var provinces = [];
        var locations_map = this.props.locations;

        locations_map.forEach(element =>{
            cities.push(element["city"]);
            provinces.push(element["province"]);
        });

        var city_render = [];
        cities.forEach((city, i) => {
            city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
        });

        var province_render = [];
        provinces.forEach((province, i) => {
            province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
        });

        var userProfile = this.state;
        if (this.props.userProfile) {
            userProfile.name = this.props.userProfile.name;
            userProfile.location = this.props.userProfile.location;
        }

        return (
            <div className="form-section">
                <h2 className="darkGreenHeader">Personal Details</h2>
                <div className="form-row">
                    <label htmlFor= "name">Name</label>
                    <input type = "text" id="name" onChange={this.handleChange} value= {userProfile.name}/>
                </div>
                <div className="form-row">
                    <label htmlFor= "location">
                        Location
                        <select defaultValue={'DEFAULT'} id="province" onChange={this.handleChange}>
                            <option value="DEFAULT" disabled>{userProfile.location.province}</option>
                            {province_render}
                        </select>
                        <select defaultValue={'DEFAULT'} id="city" onChange={this.handleChange}>
                            <option value="DEFAULT" disabled>{userProfile.location.city}</option>
                            {city_render}
                        </select>
                    </label>
                </div>
            </div>
        )
    }

}

EditUserDetails.propTypes = {
  userProfile: PropTypes.object
};

export default EditUserDetails;
