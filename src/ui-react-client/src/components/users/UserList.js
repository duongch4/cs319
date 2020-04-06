import React from 'react';
import PropTypes from 'prop-types';
import Loading from '../common/Loading';
import SearchUserCard from '../common/search/SearchUserCard'

const UserList = ({ users, canEdit, isAssignable, projectNumber, openingId, createAssignOpenings}) => {
  const userCards =[];
  users.forEach(user => {
    userCards.push(
      <SearchUserCard user={user}
        key={userCards.length}
        isUserPage={true}
        canEdit={canEdit}
        isAssignable={isAssignable}
        projectNumber={projectNumber}
        openingId={openingId}
        createAssignOpenings={(openingId, userId, utilization, user, userRoles) => 
        createAssignOpenings(openingId, userId, utilization, user, userRoles)}/>
        )});
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
