import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';
import {Button} from "@material-ui/core";

const Openings = ({ opening, index, commitment, isAssignable }) => {

    const skills = [];
    if(opening.skills){
        opening.skills.forEach((skill, index) => {
            skills.push(<span key={skills.length}> {skill} </span>);
            if(opening.skills.length - 1 !== index) {
                skills.push(" • ")
            }
        })
    }
  return (
    <div className="card-summary">
        <div className="card-summary-title">
            <h3 className="darkGreenHeader">{index + 1}</h3>
        </div>
        <div className="card-summary-title">
            <h3 className="darkGreenHeader">{opening.discipline}</h3>
            {(skills.length > 0) && (<p><b>Skills:</b> {skills}</p>)}
            <p><b>Experience:</b> {opening.yearsOfExp}</p>
            {(commitment) &&
            (<p><b>Expected Hourly Commitment per Month:</b> {commitment}</p>)}
        </div>
        {isAssignable &&
            (<div className="card-summary-title assign">
                <div>
                    <Button variant="contained"
                            style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                            disableElevation>
                        Assign
                    </Button>
                </div>
            </div>)}
    </div>

      )
};

Openings.propTypes = {
  opening: PropTypes.object.isRequired,
  index: PropTypes.number.isRequired,
  commitment: PropTypes.number,
  isAssignable: PropTypes.bool
};

export default Openings;
