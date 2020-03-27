import React, { Component } from 'react';
import './UserStyles.css'
import EditIcon from "@material-ui/icons/Edit";
import {Link} from "react-router-dom";
import {
    LOW_UTILIZATION, 
    MEDIUM_UTILIZATION, 
    HIGH_UTILIZATION,
    LOW_UTILIZATION_COLOUR,
    MEDIUM_UTILIZATION_COLOUR,
    HIGH_UTILIZATION_COLOUR,
    OVER_UTILIZATION_COLOUR
} from '../../config/config';

class UserCard extends Component {

    render(){
        const {user, canEdit} = this.props;
        let colour = ""
        if(user.utilization <= LOW_UTILIZATION){
            colour = LOW_UTILIZATION_COLOUR
        } else if(user.utilization <= MEDIUM_UTILIZATION){
            colour = MEDIUM_UTILIZATION_COLOUR
        } else if(user.utilization <= HIGH_UTILIZATION){
            colour = HIGH_UTILIZATION_COLOUR
        } else {
            colour = OVER_UTILIZATION_COLOUR
        }
        return(
            <div className="card-summary">
                <div className="card-summary-title">
                    <Link to={'/users/' + user.userID}>
                        <h2 className="blueHeader">{user.firstName + " " + user.lastName}</h2>
                    </Link>
                    <p><b>Location:</b> {user.location.city}, {user.location.province}</p>
                    {canEdit && (
                        <Link to={'/edituser/' + user.userID} className="action-link">
                            <EditIcon style={{fontSize: 'small'}}/> Edit
                        </Link>
                    )}
                </div>
                <div className="card-summary-title utilization">
                    <p style={{color: colour}}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default UserCard
