import React, { Component } from 'react';
import '../../users/UserStyles.css'
import {Link} from "react-router-dom";
import { UserContext, getUserRoles } from "../userContext/UserContext";
import {Button} from "@material-ui/core";
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

  onSubmit = (openingId, userId, utilization, user, userRoles) => {
     this.props.createAssignOpenings(openingId, userId, utilization, user, userRoles);
   };


    render(){
        const {user} = this.props;

        var disc_string = "";

        user.resourceDiscipline.forEach((disc, index) => {
            if (index === 0) {
                disc_string = disc.discipline + " (" + disc.yearsOfExp + ")";
            } else {
                disc_string = disc_string + ", " + disc.discipline + " (" + disc.yearsOfExp + ")";
            }
        });

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

        const userRoles = getUserRoles(this.context);
        return(
            <div className="card-summary">
                <div className="card-summary-title">
                    <Link to={'/users/' + user.userID}>
                        <h2 className="blueHeader">{user.firstName + " " + user.lastName}</h2>
                    </Link>
                    <p><b>Location:</b> {user.location.city}, {user.location.province}</p>
                    <p><b>Disciplines:</b> {disc_string}</p>
                    {this.props.isAssignable &&
                        (  <Button variant="contained"
                                    style={{backgroundColor: "#87c34b", color: "#ffffff", size: "small" }}
                                    disableElevation onClick={() => this.onSubmit(this.props.openingId, user.userID, user.utilization, user, userRoles)}
                                    component={Link} to={"/projects/" + this.props.projectNumber}>
                                Assign
                            </Button>)}
                </div>
                <div className="card-summary-title utilization">
                    <p style={{color: colour}}>{user.utilization}%</p>
                </div>
            </div>
        )
    }
}

SearchUserCard.contextType = UserContext;

export default SearchUserCard
