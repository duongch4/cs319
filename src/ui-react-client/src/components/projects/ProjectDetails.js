import React from 'react';
import PropTypes from 'prop-types';
import Openings from './Openings'
import './ProjectStyles.css';

const ProjectDetails = (props) => {
    // XXX this will be helpful when we pull in the database: console.log(props.match.params.project_id)
    
    // XXX These (below) will eventually be sent in from the database XXX
    var openings = [{
        title:"Discipline: Parks and Recreation",
        skills: ["Skill 1", "Skill 2", "Skill 3"],
        experience: "3-5 years",
        commitment: 160
    },
    {
        title:"Discipline: Environmental Planning",
        skills: ["Skill 1", "Skill 2", "Skill 3"],
        experience: "3-5 years",
        commitment: 160
    },
    {
        title:"Discipline: Sustainable Design",
        skills: ["Skill 1", "Skill 2", "Skill 3"],
        experience: "3-5 years",
        commitment: 160
    }]

    var openingsRender = []
    openings.forEach((opening, index) => {
        openingsRender.push(<Openings opening={opening} index={index} />)
        if(openings.length - 1 != index){
            openingsRender.push(<hr></hr>)
        }
    })

    return (
        <div className="ProjectDetails">
            <h1 className="blueHeader">project.title</h1>
            <p>Location: </p>   
            <p>Total Hours: </p>
            <p>Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ullamcorper aliquam enim in fermentum. Pellentesque placerat augue sit amet leo pretium, eget volutpat turpis fringilla. Integer imperdiet nec augue sed mollis. Phasellus at lectus porttitor, vestibulum nulla sed, tristique nunc.
            </p>
            <h2 className="greenHeader">The Team</h2>
            <h3>PlaceHolder for Profile Brief from Kaye's People pages</h3>
            <h2 className="greenHeader">Openings</h2>
            {openingsRender}
        </div>
    );
};

ProjectDetails.propTypes = {
};

export default ProjectDetails;
