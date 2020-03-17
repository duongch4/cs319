import React from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import {formatDate} from "../../util/dateFormatter";

const AvailabilityCard = ({availability}) => {
    let fromDate = formatDate(availability.fromDate);
    let toDate = formatDate(availability.toDate);
  return (
    <div className="card-summary">
        <div className="card-summary-title availability">
            <h4>{fromDate}</h4>
            <h4>to</h4>
            <h4>{toDate}</h4>
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
