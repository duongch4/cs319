import React, { Component } from 'react';
import './UserStyles.css'
import EditIcon from "@material-ui/icons/Edit";
import {Link} from "react-router-dom";

class UserCard extends Component {

    render(){
        const {user, canEdit} = this.props;
        return(
            <div className="card-summary">
                <div className="card-summary-title">
                    <Link to={'/users/' + user.userID}>
                        <h2 className="blueHeader">{user.name}</h2>
                    </Link>
                    <p><b>Location:</b> {user.location.city}, {user.location.province}</p>
                    {canEdit && (
                        <Link to={'/edituser/' + user.userID} className="action-link">
                            <EditIcon style={{fontSize: 'small'}}/> Edit
                        </Link>
                    )}
                </div>
                <div className="card-summary-title utilization">
                    <p>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default UserCard
