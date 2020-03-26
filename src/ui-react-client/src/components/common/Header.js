import React from 'react';
import { NavLink } from 'react-router-dom';
import UserHeaderMenu from './UserHeaderMenu';
import './Header.css';

const Header = () => {
  const activeStyle = { color: '#ffffff' };
  const style={ color: '#ffffff', fontSize: '20px'};
  return (
    <div className="sidenav">
      <p className="ae-logo">AE</p>
      <p className="ae-subtitle">Associated Engineering</p>
      <svg width={80} height={80}>
        <circle className="circle" r={40} cx={40} cy={40}></circle>
      </svg>
      <UserHeaderMenu />
      <nav>
        <div>
        <NavLink to="/" style={style} activeStyle={activeStyle} exact>
          Home
        </NavLink>
        </div>
        <div>
        <NavLink to="/search" style={style} activeStyle={activeStyle} exact>
          Search
        </NavLink>
        </div>
        <div>
        <NavLink to="/users" style={style} activeStyle={activeStyle} exact>
          Users
        </NavLink>
        </div>
        <div>
        <NavLink to="/projects" style={style} activeStyle={activeStyle} exact>
          Projects
        </NavLink>
        </div>
        <div>
        <NavLink to="/admin" style={style} activeStyle={activeStyle} exact>
          Admin
        </NavLink>
        </div>
      </nav>
    </div>
  );
};

export default Header;
