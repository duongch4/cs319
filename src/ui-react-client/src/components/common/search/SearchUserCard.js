import React, { Component } from 'react';
import '../../users/UserStyles.css'
import {Link} from "react-router-dom";

class SearchUserCard extends Component {

    render(){
        const {user} = this.props;
        var year_string = "";
        
        // converts years of experience to string
        user.yearsOfExp.forEach((year, index) => {
            if (index == 0) {
                year_string = year
            } else {
                year_string = year_string + ", " + year;
            }
        });
        
        return(
            <div className="card-summary">
                <div className="card-summary-title">
                    <Link to={'/users/' + user.userID}>
                        <h2 className="blueHeader">{user.firstName + " " + user.lastName}</h2>
                    </Link>
                    <p><b>Location:</b> {user.location.city}, {user.location.province}</p>
                    <p><b>Disciplines:</b> {user.discipline}</p>
                    <p><b>Years of Experience:</b> {year_string}</p>
                </div>
                <div className="card-summary-title utilization">
                    <p>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

export default SearchUserCard
