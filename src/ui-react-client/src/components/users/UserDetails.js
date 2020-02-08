import React, { Component, useEffect }  from 'react';
import PropTypes from 'prop-types';
import './UserStyles.css';
import { connect } from 'react-redux';

class UserDetails extends Component {
    state = {
        users: this.props.users.filter(user => user.userID == this.props.match.params.user_id)
    }
 
    // XXX TODO: These (below) will eventually be sent in from the database XXX

    componentDidMount = () => {
        
    }

    render(){
        
        if(this.state.users.length === 0) {
            return(
                <div className="UserDetails">
                No User Available
                </div>
            )
        }
        const userDetails = this.state.users[0]
        return (
            <div className="UserDetails">
                <h1 className="blueHeader">{userDetails.name}</h1>
                <p>Location: {userDetails.location.city}, {userDetails.location.province}</p>   
            </div>
        );
    }
};

UserDetails.propTypes = {
    users: PropTypes.array.isRequired,
};

const mapStateToProps = state => {
    return {
      users: state.users,
    };
  };

const mapDispatchToProps = {
  };
  
export default connect(
    mapStateToProps,
    mapDispatchToProps,
  )(UserDetails);
