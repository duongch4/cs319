import React from 'react';
import './HomePage.css';

const Json = ({ data }) => <pre>{JSON.stringify(data, null, 4)}</pre>;

const HomePage = (props) => {
  if (!props.account) {
    return (
        <div className="banner-img-container">
            <img className="banner-img" src="https://www.ae.ca/images/default-source/banner/edmonton-office---home-banner-full-size.jpg?sfvrsn=1063c780_0" alt="banner" />
        </div>
    );
  }
  else if (!props.profile) {
    return (
      <div className="activity-container">
        <h3>LOADING YOUR PROFILE</h3>
      </div>
    );
  }
  else {
    // TODO: Change this to user details component after database has fake users
    return (
      <div className="activity-container">
        <h1>Home</h1>
        <h3>HERE IS YOUR PROFILE</h3>
        <Json data={props.profile} />
      </div>
    );
  }
};

export default HomePage;
