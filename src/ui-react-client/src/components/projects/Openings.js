import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const Openings = ({ opening, index }) => {

    // title:"Discipline: Parks and Recreation",
    //     skills: ["skill1", "skill2", "skill3"],
    //     experience: "3-5 years",
    //     commitment: 160
    var skills = []
    opening.skills.forEach((skill, index) => {
        skills.push(<span> {skill} </span>)
        if(opening.skills.length - 1 != index) {
            skills.push("·")
        }
    })
  return (
    <div>    
        <h4 className="darkGreenHeader">{index + 1}. {opening.title}</h4>
        <p>Skills: {skills}</p>
        <p>Experience: {opening.experience}</p>
        <p>Expected Hourly Commitment per Month: {opening.commitment}</p>
    </div>
  );
};

Openings.propTypes = {
  opening: PropTypes.object.isRequired,
};

export default Openings;
