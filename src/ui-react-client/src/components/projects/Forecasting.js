import React, {Component} from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import {connect} from 'react-redux';
import { loadUsers } from '../../redux/actions/usersActions';
import {createAssignOpenings} from '../../redux/actions/forecastingActions';
import {Button} from "@material-ui/core";
import Fab from '@material-ui/core/Fab';
import AddIcon from '@material-ui/icons/Add';
import { Link } from 'react-router-dom';
// import {createProject} from '../../redux/actions/projectProfileActions.js';


class Forecasting extends Component {
  state = {
    buttonClicked: 'no',
    projectSummary: {}
  };

  onSubmit = () => {
        console.log("TEst");
        // return this.props.createProject();
        return this.props.createAssignOpening(this.state.discipline);
    // this.setState({
    //     buttonClicked: 'yes'
    // })
  };

  render() {
    return (
      <div className="activity-container">
          <div className="title-bar">
          <h2 className="blueHeader">Test</h2>
          </div>
          <div className="title-bar">
            <h1 className="greenHeader">Assign: 'Name of Role/Discipline'</h1>
          </div>

          <div className="section-container">
              <Button variant="contained"
                      style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small"}}
                      disableElevation
                      onClick={() => this.onSubmit()}>Assign</Button>
          </div>
      </div>
    )

  }
}

Forecasting.propTypes = {
  opening: PropTypes.object,
  index: PropTypes.number,
  commitment: PropTypes.number,
  isAssignable: PropTypes.bool
};

const mapStateToProps = state => {
  return {
    disciplines: state.disciplines,
    locations: state.locations,
    masterYearsOfExperience: state.masterYearsOfExperience,
  };
};

const mapDispatchToProps = {
  createAssignOpenings,
  // createProject
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(Forecasting);
