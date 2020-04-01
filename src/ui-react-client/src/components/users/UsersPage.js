import React, {useContext, useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import UserList from './UserList';
import { loadUsers } from '../../redux/actions/usersActions';
import {CLIENT_DEV_ENV} from '../../config/config';
import {UserContext, getUserRoles} from "../common/userContext/UserContext";

const UsersPage = (props) => {
  const userRoles = getUserRoles(useContext(UserContext));
  useEffect(() => {
    if (props.users.length === 0) {
        if (CLIENT_DEV_ENV) {
            props.loadUsers(["adminUser"])
        } else {
            props.loadUsers(userRoles)
                .catch(error => {
                    alert('Loading users failed' + error);
                });
        }
    }}, [props.users, props.loadUsers, userRoles]);
  return (
    <div className="activity-container">
        <h1 className="greenHeader">Users</h1>
        <UserList users={props.users} />
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
