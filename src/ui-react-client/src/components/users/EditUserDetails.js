import React, { Component } from 'react';
import './UserStyles.css';
import PropTypes from 'prop-types';

class EditUserDetails extends Component {
    state = {
        userSummary: {
            userID: this.props.userProfile.userID,
            firstName: this.props.userProfile.firstName,
            lastName: this.props.userProfile.lastName,
            location: this.props.userProfile.location,
            utilization: this.props.userProfile.utilization,
            resourceDiscipline: this.props.userProfile.resourceDiscipline,
            isConfirmed: this.props.userProfile.isConfirmed
        },
        city_options: [],
        province_options: []
    };

    componentDidMount() {
        let provinces = Array.from(Object.keys(this.props.locations));
        this.setState({
            ...this.state,
            userSummary: {
                ...this.state.userSummary,
                firstName: this.props.userProfile.firstName,
                lastName: this.props.userProfile.lastName,
                location: this.props.userProfile.location
            },
            province_options: provinces,
        })
    }

    handleChange = (e) => {
        if (e.target.id === "city") {
            this.setState({
                ...this.state,
                userSummary: {
                    ...this.state.userSummary,
                    location: {
                        ...this.state.userSummary.location,
                        locationID: 0,
                        city: e.target.value
                    }
                    }}, () => this.props.addUserDetails(this.state.userSummary));
        } else if (e.target.id === "province") {
            let newCities = this.props.locations[e.target.value];
            this.setState(
                {
                    ...this.state,
                    userSummary: {
                        ...this.state.userSummary,
                        location: {
                            ...this.state.userSummary.location,
                            province: e.target.value,
                            locationID: 0
                        }},
                    city_options: newCities
                }, () => this.props.addUserDetails(this.state.userSummary));
        } else {
            this.setState({
                ...this.state,
                    userSummary: {
                    ...this.state.userSummary,
                        [e.target.id]: e.target.value
                    }
            }, () => this.props.addUserDetails(this.state.userSummary));
        }
    };

    render() {
        var userProfile = this.state.userSummary;
        var city_render = [];
        this.state.city_options.forEach((city, i) => {
            city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
        });

        var province_render = [];
        this.state.province_options.forEach((province, i) => {
            province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
        });

        return (
            <div className="form-section">
                <h2 className="darkGreenHeader">Personal Details</h2>
                <div className="form-row">
                    <label htmlFor= "name"><p className="form-label">Name</p></label>
                    <input className="input-box" type="text" id="firstName" placeholder="First Name" onChange={this.handleChange} defaultValue={userProfile.firstName}/>
                    <input className="input-box" type="text" id="lastName" placeholder="Last Name" onChange={this.handleChange} defaultValue={userProfile.lastName}/>
                </div>
                <label htmlFor= "location" className="form-row">
                    <p className="form-label">Location</p>
                    <select className="input-box" defaultValue={'DEFAULT'} id="province" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{userProfile.location.province}</option>
                        {province_render}
                    </select>
                    <select className="input-box" defaultValue={'DEFAULT'} id="city" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{userProfile.location.city}</option>
                        {city_render}
                    </select>
                </label>
            </div>
        )
    }

}

EditUserDetails.propTypes = {
  userProfile: PropTypes.object
};

export default EditUserDetails;
