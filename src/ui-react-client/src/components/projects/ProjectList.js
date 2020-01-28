import React from 'react';
import PropTypes from 'prop-types';

const ProjectList = ({ projects }) => {
  return (
    <>
      <table>
        <thead>
          <tr>
            <th>Id</th>
            <th>Number</th>
            <th>Title</th>
            <th>Location</th>
            <th>Created At</th>
            <th>Updated At</th>
          </tr>
        </thead>
        <tbody>
          {projects.map(project => (
            <tr key={project.id}>
              <td>{project.id}</td>
              <td>{project.number}</td>
              <td>{project.title}</td>
              <td>{project.locationName}</td>
              <td>{project.createdAt}</td>
              <td>{project.updatedAt}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </>
  );
};

ProjectList.propTypes = {
  projects: PropTypes.array.isRequired,
};

export default ProjectList;
