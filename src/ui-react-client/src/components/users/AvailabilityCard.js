import React from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';

const AvailabilityCard = ({availability}) => {
  return (
    <div className="card-summary">
        {/* XXX TODO: once we figure out how the date will come in, 
        we can change this date format*/}
        <div className="card-summary-title availability">
            <h4>{availability.start}</h4>
            <h4>to</h4>
            <h4>{availability.end}</h4>
        </div>
        <div className="card-summary-title">
            <p>{availability.reason}</p>
        </div>
    </div>
  )
}

AvailabilityCard.propTypes = {
    availability: PropTypes.object
}

export default AvailabilityCard
