import React, { Component } from 'react';
import '../../users/UserStyles.css'
import {Link} from "react-router-dom";
import {
    LOW_UTILIZATION, 
    MEDIUM_UTILIZATION, 
    HIGH_UTILIZATION,
    LOW_UTILIZATION_COLOUR,
    MEDIUM_UTILIZATION_COLOUR,
    HIGH_UTILIZATION_COLOUR,
    OVER_UTILIZATION_COLOUR
} from '../../../config/config';

class SearchUserCard extends Component {
    render(){
        const {user} = this.props;
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
                    <p><b>Disciplines:</b> {user.resourceDiscipline.discipline}</p>
                </div>
                <div className="card-summary-title utilization">
                    <p style={{color: colour}}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default SearchUserCard
