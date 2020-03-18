import React, {Component} from 'react';
import './ProjectStyles.css';
import PropTypes from 'prop-types';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class CreateEditProjectDetails extends Component {
    state = {
        projectSummary: {
            title: null,
            location: {
                locationID: 0,
                city: 'DEFAULT',
                province: 'DEFAULT'},
            projectStartDate: new Date(),
            projectEndDate: new Date(),
            projectNumber: ""
        },
        city_options: [],
        province_options: [],
        pending: true
    };

    componentDidMount() {
        let provinces = Object.keys(this.props.locations);
        let currentProj = this.props.currentProject;
        if (currentProj) {
            let cities = Object.keys(this.props.locations[currentProj.location.province]);
            let projStart = new Date(currentProj.projectStartDate);
            let projEnd = new Date(currentProj.projectEndDate);
            this.setState({
                ...this.state,
                projectSummary: {
                    ...this.state.projectSummary,
                    title: currentProj.title,
                    location: {
                        ...this.state.projectSummary.location,
                        locationID: currentProj.location.locationID,
                        city: currentProj.location.city,
                        province: currentProj.location.province
                    },
                    projectStartDate: projStart,
                    projectEndDate: projEnd,
                    projectNumber: currentProj.projectNumber
                },
                province_options: provinces,
                city_options: cities,
                pending: false
            })
        } else {
            this.setState({
                ...this.state,
                province_options: provinces,
                pending: false
            });
        }
    }

    handleChange = (e) => {
        if (e.target.id === "city") {
            let locationID = this.props.locations[this.state.projectSummary.location.province][e.target.value];
            this.setState({
                ...this.state,
                projectSummary: {
                    ...this.state.projectSummary,
                    location: {
                        ...this.state.projectSummary.location,
                        city: e.target.value,
                        locationID: locationID
                    }
                }
            }, () => this.props.addProjDetails(this.state.projectSummary));
        } else if (e.target.id === "province") {
            let newCities = Object.keys(this.props.locations[e.target.value]);
            let locationID = this.props.locations[e.target.value][newCities[0]];
            this.setState({
                ...this.state,
                projectSummary: {
                    ...this.state.projectSummary,
                    location: {
                        ...this.state.projectSummary.location,
                        province: e.target.value,
                        city: newCities[0],
                        locationID: locationID
                    },
                },
                city_options: newCities
            }, () => this.props.addProjDetails(this.state.projectSummary));
        } else {
            this.setState({
                ...this.state,
                projectSummary: {
                    ...this.state.projectSummary,
                    [e.target.id]: e.target.value
                }
            }, () => this.props.addProjDetails(this.state.projectSummary));
        }
    };

    handleChangeStartDate = (date) => {
        this.setState({
            ...this.state,
            projectSummary: {
                ...this.state.projectSummary,
                projectStartDate: date
            }
        }, () => this.props.addProjDetails(this.state.projectSummary))
    };

    handleChangeEndDate = (date) => {
        this.setState({
            ...this.state,
            projectSummary: {
                ...this.state.projectSummary,
                projectEndDate: date
            }
        }, () => this.props.addProjDetails(this.state.projectSummary))
    };

    render() {
        let projSummary = this.state.projectSummary;
        var city_render = [];
        this.state.city_options.forEach((city, i) => {
            city_render.push(<option key={"cities_" + i} value={city}>{city}</option>)
        });

        var province_render = [];
        this.state.province_options.forEach((province, i) => {
            province_render.push(<option key={"provinces_" + i} value={province}>{province}</option>)
        });

        // load default values
        let def_projectNumber, def_projectTitle;
        projSummary.projectNumber ? def_projectNumber = projSummary.projectNumber : def_projectNumber = null;
        projSummary.title ? def_projectTitle = projSummary.title : def_projectTitle = null;

        return (
        <div className="form-section">
            <h2 className="darkGreenHeader">Project Details</h2>
            <div className="form-section">
                <div className="form-row">
                    <label htmlFor="projectNumber"><p className="form-label">ProjectNumber</p></label>
                    <input className="input-box" type="text" id="projectNumber" placeholder="Project Number" onChange={this.handleChange}
                           defaultValue={def_projectNumber}/>
                </div>
                <div className="form-row">
                    <label htmlFor="title"><p className="form-label">Title</p></label>
                    <input className="input-box" type="text" id="title" onChange={this.handleChange}
                           defaultValue={def_projectTitle} placeholder="Project Title"/>
                </div>
                <label htmlFor="location" className="form-row">
                    <p className="form-label">Location</p>
                    <select className="input-box" value={projSummary.location.province} id="province"
                            onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>Province</option>
                        {province_render}
                    </select>
                    <select className="input-box"  value={projSummary.location.city} id="city" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>City</option>
                        {city_render}
                    </select>
                </label>
                <label htmlFor="project_duration" className="form-row">
                    <p className="form-label">Project Duration</p>
                    <DatePicker className="input-box" id="startDate" selected={projSummary.projectStartDate}
                                onChange={this.handleChangeStartDate}/>
                    <DatePicker className="input-box" id="endDate" selected={projSummary.projectEndDate}
                                onChange={this.handleChangeEndDate}/>
                </label>
            </div>
        </div>)
    }
}

CreateEditProjectDetails.propTypes = {
    addOpening: PropTypes.object,
    currentProject: PropTypes.object
};

export default CreateEditProjectDetails;
