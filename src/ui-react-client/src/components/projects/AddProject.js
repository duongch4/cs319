import React,{ Component } from 'react';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom';
import Openings from './Openings'
import CreateEditProjectDetails from './CreateEditProjectDetails'
import TeamRequirements from './TeamRequirements'

class AddProject extends Component {

  render(){
    return (
      <>
      <h2 className="greenHeader">Create new project</h2>
        <CreateEditProjectDetails/>
        <TeamRequirements/>
        <Openings opening={  {
          openingID: 112,
          discipline: {
            name: "Discipline: Sustainable Design",
            skills: ["Skill 1", "Skill 2", "Skill 3"],
            yearsOfExperience: "3-5 years",
          },
          commitment: 160
        }} index={0}/>
      </>
    );
  }
};

AddProject.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default AddProject;
