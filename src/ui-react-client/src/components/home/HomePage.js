import React from 'react';
import './HomePage.css';
import UserDetails from '../users/UserDetails';
import Loading from '../common/Loading';

const Json = ({ data }) => <pre>{JSON.stringify(data, null, 4)}</pre>;

const HomePage = (props) => {
  if (!props.account) {
    return (
        <div className="banner-img-container">
            <img className="banner-img" src="https://www.ae.ca/images/default-source/banner/edmonton-office---home-banner-full-size.jpg?sfvrsn=1063c780_0"/>
        </div>
    );
  }
  else if (!props.profile) {
    return (
      <div className="activity-container">
        <Loading />
      </div>
    );
  }
  else {
    return (
      <div className="activity-container">
        <h1>Welcome, {props.profile.givenName}</h1>
        <UserDetails id={props.profile.id} />
      </div>
    );
  }
};

export default HomePage;
