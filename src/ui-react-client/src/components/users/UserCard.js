import React, { Component } from 'react';
import './UserStyles.css'
import EditIcon from "@material-ui/icons/Edit";
import {Link} from "react-router-dom";

class UserCard extends Component {
    state ={
        // these are the top of the range
        low: 50,
        medium: 85,
        high: 100,
    }
    render(){
        const {user, canEdit} = this.props;
        let styleName = "";
        if(user.utilization <= this.state.low){
            styleName = "lowUtil"
        } else if(user.utilization <= this.state.medium){
            styleName = "mediumUtil"
        } else if(user.utilization <= this.state.high){
            styleName = "highUtil"
        } else {
            styleName = "overUtil"
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
                    <p className={styleName}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default UserCard
