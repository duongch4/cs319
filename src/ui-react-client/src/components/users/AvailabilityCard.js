import React from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import {formatDate} from "../../util/dateFormatter";
import DeleteIcon from '@material-ui/icons/Delete'

const AvailabilityCard = ({edit, availability, removeAvailability}) => {
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
        {edit && <DeleteIcon onClick={()=>removeAvailability(availability)}/>}
    </div>
  )
}

AvailabilityCard.propTypes = {
    availability: PropTypes.object,
    removeAvailability: PropTypes.func
}

export default AvailabilityCard
