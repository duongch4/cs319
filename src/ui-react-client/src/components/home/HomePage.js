import React from 'react';

const Json = ({ data }) => <pre>{JSON.stringify(data, null, 4)}</pre>;

const HomePage = (props) => {

  if (!props.account) {
    return (
      <div className="activity-container">
        <h1>Home</h1>
        <h3>YOU ARE NOT LOGGED IN</h3>
      </div>
    );
  }
  else if (!props.profile) {
    return (
      <div className="activity-container">
        <h1>Home</h1>
        <h3>LOADING YOUR PROFILE</h3>
      </div>
    );
  }
  else {
    return (
      <div className="activity-container">
        <h1>Home</h1>
        <h3>HERE IS YOUR PROFILE</h3>
        <Json data={props.profile} />
      </div>
    );
  }

}

export default HomePage;
