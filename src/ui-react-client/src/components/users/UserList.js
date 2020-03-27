import React from 'react';
import PropTypes from 'prop-types';
import UserCard from './UserCard';
import Loading from '../common/Loading';

const UserList = ({ users }) => {
  const userCards =[];
  users.forEach(user => {
    userCards.push(
        <div className="card" key={userCards.length}>
            <UserCard user={user} key={userCards.length} canEdit={true}/>
        </div>)
    
  });
  if(users.length === 0){
    return(<Loading />)
  }
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
