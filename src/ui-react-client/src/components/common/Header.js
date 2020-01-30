import React from 'react';
import { NavLink } from 'react-router-dom';
import UserHeaderMenu from './UserHeaderMenu';
import './Header.css';

const Header = () => {
  const activeStyle = { color: '#F15B2A' };

  return (
    <div className="sidenav">
      <UserHeaderMenu />
      <nav>
        <NavLink to="/" activeStyle={activeStyle} exact>
          Home
        </NavLink>
        {' | '}
        <NavLink to="/users" activeStyle={activeStyle}>
          Users
        </NavLink>
        {' | '}
        <NavLink to="/projects" activeStyle={activeStyle}>
          Projects
        </NavLink>
        {' | '}
        <NavLink to="/locations" activeStyle={activeStyle}>
          Locations
        </NavLink>
      </nav>
    </div>
  );
};

export default Header;
