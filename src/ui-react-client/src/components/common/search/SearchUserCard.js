import React, { Component } from 'react';
import '../../users/UserStyles.css'
import {Link} from "react-router-dom";

class SearchUserCard extends Component {

    render(){
        const {user} = this.props;
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
                    <p>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default SearchUserCard
