import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const Openings = ({ opening, index, commitment }) => {

    const skills = []
    if(opening.skills){
        opening.skills.forEach((skill, index) => {
            skills.push(<span key={skills.length}> {skill} </span>)
            if(opening.skills.length - 1 != index) {
                skills.push("·")
            }
        })
    }
  return (
    <div>
        <h4 className="darkGreenHeader">{index + 1}. {opening.name}</h4>
        <p>Skills: {skills}</p>
        <p>Experience: {opening.yearsOfExperience}</p>
        <p>Expected Hourly Commitment per Month: {commitment}</p>
      </div>

      );
};

Openings.propTypes = {
  opening: PropTypes.object.isRequired,
  index: PropTypes.number.isRequired,
  commitment: PropTypes.number
};

export default Openings;
