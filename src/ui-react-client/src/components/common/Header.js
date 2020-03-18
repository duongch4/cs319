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
        <svg width={80} height={80}>
          <circle r={40} cx={40} cy={40}></circle>
        </svg>
        {
          !this.props.appProps.account ? <RequestButton text="Log In" onClick={this.props.appProps.onSignIn} useRedirect={isIE()} /> : (
            <div>
              <RequestButton text="Log Out" onClick={this.props.appProps.onSignOut} />
              <RequestButton text="Request Email Token" onClick={this.props.appProps.onRequestEmailToken} />
              <nav className="navlink">
                <div><Link to="/">Home</Link></div>
                <div><Link to="/users">Users</Link></div>
                <div><Link to="/projects">Projects</Link></div>
                <div><Link to="/admin">Admin</Link></div>
                <div><Link to={
                  {
                    pathname: "/current_user",
                    state: {
                      account: this.props.appProps.account,
                      profile: this.props.appProps.graphProfile,
                      emailMessages: this.props.appProps.emailMessages
                    }
                  }
                }>Current User</Link></div>
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