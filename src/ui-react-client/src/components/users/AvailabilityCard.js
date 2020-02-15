import React from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';

const AvailabilityCard = ({availability}) => {
  return (
    <div className="projectCard">
        {/* XXX TODO: once we figure out how the date will come in, 
        we can change this date format*/}
        <h1>{availability.start} to {availability.end}</h1>
        <p>{availability.reason}</p>
    </div>
  );
};

AvailabilityCard.propTypes = {
    availability: PropTypes.object
};

export default AvailabilityCard;
