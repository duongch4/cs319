import React, { Component } from 'react';
import '../../users/UserStyles.css'
import {Link} from "react-router-dom";

class SearchUserCard extends Component {
    state ={
        // these are the top of the range
        low: 50,
        medium: 85,
        high: 100,
    }

    render(){
        const {user} = this.props;
        var disc_string = "";
        
        user.resourceDiscipline.forEach((disc, index) => {
            if (index == 0) {
                disc_string = disc.discipline + " (" + disc.yearsOfExp + ")";
            } else {
                disc_string = disc_string + ", " + disc.discipline + " (" + disc.yearsOfExp + ")";
            }
        });

        let styleName = ""
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
                    <p><b>Disciplines:</b> {disc_string}</p>
                </div>
                <div className="card-summary-title utilization">
                    <p className={styleName}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default SearchUserCard
