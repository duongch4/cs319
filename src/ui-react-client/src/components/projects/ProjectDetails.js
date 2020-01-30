import React from 'react';
import PropTypes from 'prop-types';
import './ProjectStyles.css';

const ProjectDetails = (props) => {
  return (
    <div className="ProjectDetails">
         <h1>project.title</h1>
         <p>Location: </p>   
         <p>Total Hours: </p>
         <p>Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent ullamcorper aliquam enim in fermentum. Pellentesque placerat augue sit amet leo pretium, eget volutpat turpis fringilla. Integer imperdiet nec augue sed mollis. Phasellus at lectus porttitor, vestibulum nulla sed, tristique nunc. Suspendisse id metus id nisl ultrices faucibus. Nam egestas sodales odio vitae fermentum. Ut dignissim turpis in leo gravida, vitae rutrum lorem consequat. Praesent nec arcu vitae libero facilisis dignissim. Interdum et malesuada fames ac ante ipsum primis in faucibus. Phasellus aliquam turpis vel rhoncus rutrum. Donec vitae eros mi. Aliquam non arcu euismod, finibus dolor in, eleifend augue. Phasellus lobortis tempus scelerisque. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos.
         </p>
         <h2 className="greenHeader">The Team</h2>
         <h3>PlaceHolder for Profile Brief from Kaye's People pages</h3>
         <h2 className="greenHeader">Openings</h2>
        {/* <h1>{number}. {project.title}</h1>
        <p>Location: {project.locationName}</p>
        <p>Duration: {project.createdAt} - {project.updatedAt}</p> */}
    </div>
  );
};

ProjectDetails.propTypes = {
};

export default ProjectDetails;
