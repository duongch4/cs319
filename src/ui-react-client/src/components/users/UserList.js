import React from 'react';
import PropTypes from 'prop-types';

const UserList = ({ users }) => {
  return (
    <>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Username</th>
            <th>Location</th>
          </tr>
        </thead>
        <tbody>
          {users.map(user => (
            <tr key={user.id}>
              <td>{user.id}</td>
              <td>{user.firstName}</td>
              <td>{user.lastName}</td>
              <td>{user.username}</td>
              <td>{user.locationName}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

UserList.propTypes = {
  users: PropTypes.array.isRequired,
};

export default UserList;
