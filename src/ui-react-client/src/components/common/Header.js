import React from 'react';
import { Link } from 'react-router-dom';
import './Header.css';

import PropTypes from "prop-types";
import AuthProvider from "../../config/AuthProvider";
import { isIE } from "../../config/authUtils";
import { RequestButton } from "./RequestButton";

class Header extends React.Component {
  static propTypes = {
    account: PropTypes.object,
    emailMessages: PropTypes.object,
    error: PropTypes.string,
    graphProfile: PropTypes.object,
    onSignIn: PropTypes.func.isRequired,
    onSignOut: PropTypes.func.isRequired,
    onRequestEmailToken: PropTypes.func.isRequired
  };

  render() {
    return (
      <div className="sidenav">
        <p className="ae-logo">AE</p>
        <p className="ae-subtitle">Associated Engineering</p>
        <svg width={80} height={80}>
          <circle r={40} cx={40} cy={40}></circle>
        </svg>
        {
          !this.props.account ? <RequestButton text="Log In" onClick={this.props.onSignIn} useRedirect={isIE()} /> : (
            <div>
              <RequestButton text="Log Out" onClick={this.props.onSignOut} />
              <RequestButton text="Request Email Token" onClick={this.props.onRequestEmailToken} />
              <nav className="navlink">
                <div><Link to="/">Home</Link></div>
                <div><Link to="/users">Users</Link></div>
                <div><Link to="/projects">Projects</Link></div>
                <div><Link to="/admin">Admin</Link></div>
                <div><Link to={
                  {
                    pathname: "/current_user",
                    state: {
                      account: this.props.account,
                      profile: this.props.graphProfile,
                      emailMessages: this.props.emailMessages
                    }
                  }
                }>Current User</Link></div>
              </nav>
            </div>
          )
        }
        {this.props.error && (<p className="error">Error: {this.props.error}</p>)}
      </div>
    );
  }
}

export default AuthProvider(Header);