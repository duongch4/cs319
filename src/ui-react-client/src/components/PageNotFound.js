import React from 'react';

const PageNotFound = () => {
    return (
        <div className="activity-container">
            <div style={{textAlign: "center", display: 'flex', flexDirection: 'column', alignContent: 'center', alignItems: 'center'}}>
                <h1> HTTP ERROR 404 </h1>
                <p>Page not found.</p>
            </div>
        </div>
    );
};

export default PageNotFound;
