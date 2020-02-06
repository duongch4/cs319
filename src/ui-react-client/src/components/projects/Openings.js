import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const Openings = ({ opening, index }) => {

    const skills = []
    const discipline = opening.discipline;
    if(discipline.skills){
        discipline.skills.forEach((skill, index) => {
            skills.push(<span key={skills.length}> {skill} </span>)
            if(discipline.skills.length - 1 != index) {
                skills.push("·")
            }
        })
    }
  return (
    <div>
        <h4 className="darkGreenHeader">{index + 1}. {discipline.name}</h4>
        <p>Skills: {skills}</p>
        <p>Experience: {discipline.yearsOfExperience}</p>
        <p>Expected Hourly Commitment per Month: {opening.commitment}</p>
      </div>

      );
};

Openings.propTypes = {
  opening: PropTypes.object.isRequired,
};

export default Openings;
