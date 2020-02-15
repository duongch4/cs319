import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import UserList from './UserList';
import { loadUsers } from '../../redux/actions/usersActions';
import { loadLocations } from '../../redux/actions/locationsActions';

const UsersPage = ({
  users,
  locations,
  loadUsers,
  loadLocations,
}) => {
  useEffect(() => {
    if (users.length === 0) {
      loadUsers()
      // XXX TODO: Uncomment this once the database is added
      // loadUsers().catch(error => {
      //   alert('Loading users failed' + error);
      // });
    }

    if (locations.length === 0) {
      // XXX TODO: Uncomment this once the database is added
      // loadLocations().catch(error => {
      //   alert('Loading locations failed' + error);
      // });
    }
  }, [users, locations, loadUsers, loadLocations]);

  return (
    <div className="activity-container">
        <h1>Users</h1>
        <UserList users={users} />
      </div>
  );
};

UsersPage.propTypes = {
  users: PropTypes.array.isRequired,
  locations: PropTypes.array.isRequired,
  loadUsers: PropTypes.func.isRequired,
  loadLocations: PropTypes.func.isRequired,
};

const mapStateToProps = state => {
  return {
    users: state.users,
    // XXX TODO: Uncomment this once the database is added
      // state.locations.length === 0
      //   ? []
      //   : state.users.map(user => {
      //       return {
      //         ...user,
      //         locationName: state.locations.find(
      //           element => element.id === user.locationId,
      //         ).name,
      //       };
      //     }),
    locations: state.locations,
  };
};

const mapDispatchToProps = {
  loadUsers,
  loadLocations,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(UsersPage);
