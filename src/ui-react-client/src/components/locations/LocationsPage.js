import React, { useEffect } from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import LocationList from './LocationList';
import { loadLocations } from '../../redux/actions/locationsActions';

const LocationsPage = ({ locations, loadLocations }) => {
  useEffect(() => {
    if (locations.length === 0) {
      loadLocations().catch(error => {
        alert('Loading locations failed' + error);
      });
    }
  }, [locations, loadLocations]);

  return (
    <div className="activity-container">
      <h1>Locations</h1>
      <LocationList locations={locations} />
    </div>
  );
};

LocationsPage.propTypes = {
  locations: PropTypes.array.isRequired,
  loadLocations: PropTypes.func.isRequired,
};

const mapStateToProps = state => {
  return {
    locations: state.locations,
  };
};

const mapDispatchToProps = {
  loadLocations,
};

export default connect(
  mapStateToProps,
  mapDispatchToProps,
)(LocationsPage);
