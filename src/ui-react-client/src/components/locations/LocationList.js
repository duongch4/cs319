import React from 'react';
import PropTypes from 'prop-types';

const LocationList = ({ locations }) => {
  return (
    <div>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Code</th>
            <th>Name</th>
          </tr>
        </thead>
        <tbody>
          {locations.map(location => (
            <tr key={location.id}>
              <td>{location.id}</td>
              <td>{location.code}</td>
              <td>{location.name}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

LocationList.propTypes = {
  locations: PropTypes.array.isRequired,
};

export default LocationList;
