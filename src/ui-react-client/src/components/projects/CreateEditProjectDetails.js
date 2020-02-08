import React,{ Component } from 'react';
import './ProjectStyles.css';
import DatePicker from "react-datepicker";
import "react-datepicker/dist/react-datepicker.css";

class CreateEditProjectDetails extends Component {
    state = {
      title: null,
      city: null,
      province: null,
      start_date: new Date(),
      end_date: new Date()
    }

    handleChange = (e) => {
      this.setState({
        [e.target.id]: e.target.value
      })
    }

    handleChangeStartDate = (date) => {
      this.setState({
        start_date: date
      })
    }

    handleChangeEndDate = (date) => {
      this.setState({
        end_date: date
      })
    }

    handleSubmit = (e) =>{
      e.preventDefault();
      // this.props.addOpening(this.state)
    }
  render(){
    return (
      <div>
          <h4 className="darkGreenHeader">Project Details</h4>

          <form onSubmit={this.handleSubmit}>
            <label htmlFor= "title">Title</label>
            <input type = "text" id="title" onChange={this.handleChange}/>

            <label htmlFor= "location">
            Location
            <select id="city" onChange={this.handleChange}>
              <option selected disabled>City</option>
              <option value="city_id_1">C1</option>
              <option value="city_id_2">C2</option>
            </select>

            <select id="province" onChange={this.handleChange}>
              <option selected disabled>Province</option>
              <option value="province_id_1">P1</option>
              <option value="province_id_2">P2</option>
            </select>

            </label>

            <label htmlFor= "project_duration">
            Project Duration
            <DatePicker id="start_date" selected={this.state.start_date} onChange={this.handleChangeStartDate} />
            <DatePicker id="end_date" selected={this.state.end_date} onChange={this.handleChangeEndDate} />
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
