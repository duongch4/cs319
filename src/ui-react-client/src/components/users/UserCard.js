import React, { Component } from 'react';
import './UserStyles.css'

class UserCard extends Component {

    render(){
        const {user} = this.props
        return(
            <div className="projectCard">    
                <h1 className="blueHeader">{user.name}</h1>
                <p>Location: {user.location.city}, {user.location.province}</p>
                <p>Utilization: {user.utilization}</p>
            </div>
        )
    }
}

export default UserCard