import React,{ Component } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'

class AddProject extends Component {
  // createProject(project)
  // openings: [{
    //     openingID: 111,
    //     discipline: {
    //       name: "Discipline: Parks and Recreation",
    //       skills: ["Skill 1", "Skill 2", "Skill 3"],
    //       yearsOfExperience: "3-5 years",
    //     },
    //     commitment: 160
    //   }
  state = {
    project: {
      projID: null,
      name: "",
      location: {
        city: "",
        province: ""
      },
      startDate: "",
      endDate: "",
      openings: [],      
    }
  }

  addOpening = (opening) => {
    const openings = [...this.state.project.openings, opening]
    this.setState({
      project:{
        openings
      }
    })
  }

  render(){
    const openings = []
    this.state.project.openings.forEach((opening, index) => {
      openings.push(<Openings opening={opening} index={index}/>)
    });
    return (
      <>
      <h2 className="greenHeader">Create new project</h2>
        <CreateEditProjectDetails addOpening={() => this.addOpening()} />
        <TeamRequirements/>
        {openings}
      </>
    );
  }
};

AddProject.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default AddProject;
