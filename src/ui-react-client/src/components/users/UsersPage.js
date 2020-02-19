import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import UserList from './UserList';
import { loadUsers } from '../../redux/actions/usersActions';

const UsersPage = ({
  users,
  loadUsers,
}) => {
  useEffect(() => {
    if (users.length === 0) {
      loadUsers()
          .catch(error => {
            alert('Loading users failed' + error);
          });
    }
  }, [users, loadUsers]);
  return (
    <div className="activity-container">
        <h1 className="greenHeader">Users</h1>
        <UserList users={users.userSummaries} />
    </div>
  );
};

UsersPage.propTypes = {
  users: PropTypes.array.isRequired,
  loadUsers: PropTypes.func.isRequired,
};

const mapStateToProps = state => {
  return {
    users: state.users,
  };
};

const mapDispatchToProps = {
  loadUsers
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(UsersPage);
