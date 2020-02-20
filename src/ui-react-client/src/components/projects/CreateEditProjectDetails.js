import React, {Component} from 'react';
import './ProjectStyles.css';
import PropTypes from 'prop-types';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class CreateEditProjectDetails extends Component {
    state = {
        projectSummary: {
            title: "",
            location: {city: "", province: ""},
            projectStartDate: new Date(),
            projectEndDate: new Date(),
            projectNumber: ""
        },
        city_options: [],
        province_options: [],
        pending: true
    };

    componentDidMount() {
        this.setState({
            ...this.state,
            province_options: Object.keys(this.props.locations),
            pending: false
        });
        let currentProj = this.props.currentProject;
        if (currentProj) {
            let projStart = new Date(currentProj.projectStartDate);
            let projEnd = new Date(currentProj.projectEndDate);
            this.setState({
                ...this.state,
                projectSummary: {
                    ...this.state.projectSummary,
                    title: currentProj.title,
                    location: {
                        ...this.state.projectSummary.location,
                        city: currentProj.location.city,
                        province: currentProj.location.province
                    },
                    projectStartDate: projStart,
                    projectEndDate: projEnd,
                    projectNumber: currentProj.projectNumber
                }
            })
        }
    }

    handleChange = (e) => {
        if (e.target.id === "city") {
            this.setState({
                location: {...this.state.location, city: e.target.value}
            }, () => this.props.addProjDetails(this.state))
        } else if (e.target.id === "province") {
            this.setState({
                location: {...this.state.location, province: e.target.value},
                city_options: this.state.city_options
            }, () => this.props.addProjDetails(this.state))
        } else {
            this.setState({
                [e.target.id]: e.target.value
            }, () => this.props.addProjDetails(this.state))
        }
    };

    handleChangeStartDate = (date) => {
        this.setState({
            startDate: date
        }, () => this.props.addProjDetails(this.state))
    };

    handleChangeEndDate = (date) => {
        this.setState({
            endDate: date
        }, () => this.props.addProjDetails(this.state))
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

        return (
        <div className="form-section">
            <h2 className="darkGreenHeader">Project Details</h2>
            <div className="form-section">
                <div className="form-row">
                    <label htmlFor="name"><p className="form-label">Title</p></label>
                    <input className="input-box" type="text" id="name" onChange={this.handleChange}
                           defaultValue={projSummary.title}/>
                </div>
                <label htmlFor="location" className="form-row">
                    <p className="form-label">Location</p>
                    <select className="input-box" defaultValue={'DEFAULT'} id="province"
                            onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{projSummary.location.province}</option>
                        {province_render}
                    </select>
                    <select className="input-box" defaultValue={'DEFAULT'} id="city" onChange={this.handleChange}>
                        <option value="DEFAULT" disabled>{projSummary.location.city}</option>
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
