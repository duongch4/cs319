import React, { Component } from 'react';

class UserCard extends Component {
    
    render(){
        const {user} = this.props
        return(
            <div>
                {user.id}
                {user.firstName}
                {user.lastName}
                {user.username}
                {user.locationName}
            </div>
        )
    }
}

export default UserCard
