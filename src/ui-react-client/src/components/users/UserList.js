import React from 'react';
import PropTypes from 'prop-types';
import UserCard from './UserCard';

const UserList = ({ users }) => {
  const userCards =[]
  users.forEach(user => {
    userCards.push(<UserCard user={user} key={userCards.length}/>)
    
  });
  
  return (
    <>
      {userCards}
    </>
  );
};

UserList.propTypes = {
  users: PropTypes.array.isRequired,
};

export default UserList;
