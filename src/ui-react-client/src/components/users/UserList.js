import React from 'react';
import PropTypes from 'prop-types';
import UserCard from './UserCard';
import { Link } from 'react-router-dom';

const UserList = ({ users }) => {
  const userCards =[]
  users.forEach(user => {
    userCards.push(
        <div className="card">
          <Link to={'/users/' + user.userID} key={userCards.length}>
            <UserCard user={user} key={userCards.length} canEdit={true}/>
          </Link>
        </div>)
    
  });
  
  return (
      <div>
      {userCards}
      </div>
  );
};

UserList.propTypes = {
  users: PropTypes.array.isRequired,
};

export default UserList;
