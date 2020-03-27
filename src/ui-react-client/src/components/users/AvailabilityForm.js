import React,{ Component } from 'react';
import {connect} from "react-redux";
import {Button} from "@material-ui/core";
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class AvailabilityForm extends Component {
    state = {
        startDate: new Date(),
        endDate: new Date(),
        reason: "",
        error: []
    };

    handleSubmit = (e) => {
        e.preventDefault();
        let error = [];
        if(this.state.startDate > this.state.endDate) {
            this.setState({
                error: [...error, <p key={error.length} className="errorMessage">Error: Invalid Date Range</p>]
            })
            error = [...error, <p key={error.length} className="errorMessage">Error: Invalid Date Range</p>];
        }
        if(this.state.reason === "") {
            this.setState({
                error: [...error, <p key={error.length} className="errorMessage">Error: No Reason Entered</p>]
            })
            error = [...error, <p key={error.length} className="errorMessage">Error: No Reason Entered</p>];
        }
        if(this.state.reason !== "" && this.state.startDate <= this.state.endDate){
            this.props.addAvailability(this.state);
            this.setState({
                error: []
            })
        }
    };

    handleChange = (e) => {
        this.setState({
            ...this.state,
            reason: e.target.value
        })
    }

    handleChangeStartDate = (date) => {
        this.setState({
            ...this.state,
            startDate: date
        })
    };

    handleChangeEndDate = (date) => {
        this.setState({
            ...this.state,
            endDate: date
        })
    };

    render() {
        return(
        <div>
            <form onSubmit={this.handleSubmit}>
                <label htmlFor="project_duration" className="form-row">
                    <p className="form-label">Leave Duration</p>
                    <DatePicker 
                        className="input-box" 
                        id="startDate" 
                        selected={this.state.startDate}
                        onChange={this.handleChangeStartDate}/>
                    <DatePicker 
                        className="input-box" 
                        id="endDate" 
                        selected={this.state.endDate}
                        onChange={this.handleChangeEndDate}/>
                </label>
                <div className="form-row">
                    <label htmlFor="title"><p className="form-label">Reason</p></label>
                    <input className="input-box" type="text" id="reason" onChange={this.handleChange}
                        placeholder="Reason"/>
                </div>
                {this.state.error}
                <Button type="submit" variant="contained"
                    style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                    disableElevation>Add Unavailability</Button>
            </form>
        </div>)
    }
}

const mapStateToProps = state => {
    return {

    };
};

const mapDispatchToProps = {
};

export default connect(
    mapStateToProps,
    mapDispatchToProps,
)(AvailabilityForm);
