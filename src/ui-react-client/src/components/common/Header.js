import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';

import { isIE } from "../../config/authUtils";
import { RequestButton } from "./RequestButton";

class Header extends React.Component {
  render() {
    return (
      <div className="sidenav">
        <p className="ae-logo">AE</p>
        <p className="ae-subtitle">Associated Engineering</p>
        {
          !this.props.appProps.account ? <RequestButton text="Log In" onClick={this.props.appProps.onSignIn} useRedirect={isIE()} /> : (
            <div className="navigation">
                {!this.props.appProps.graphProfile ? <div/> : (
                    <div className="user-info">
                        <p>{this.props.appProps.graphProfile.displayName}</p>
                    </div>
                )}
              <RequestButton text="Log Out" onClick={this.props.appProps.onSignOut} />
                <nav className="navlink">
                <div><Link to="/">Home</Link></div>
                <div><Link to="/search">Search</Link></div>
                <div><Link to="/users">Users</Link></div>
                <div><Link to="/projects">Projects</Link></div>
                <div><Link to="/admin">Admin</Link></div>
              </nav>
            </div>
          )
        }
        {this.props.appProps.error && (<p className="error">Error: {this.props.appProps.error}</p>)}
      </div>
    );
  }
}

export default Header;
