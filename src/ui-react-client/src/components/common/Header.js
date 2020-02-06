import React from 'react';
import { NavLink } from 'react-router-dom';
import UserHeaderMenu from './UserHeaderMenu';
import './Header.css';

const Header = () => {
  const activeStyle = { color: '#ffffff' };
  const style={ color: '#ffffff', fontSize: '20px'};
  return (
    <div className="sidenav">
      <UserHeaderMenu />
      <nav>
        <div>
        <NavLink to="/" style={style} activeStyle={activeStyle} exact>
          Home
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
        <NavLink to="/locations" style={style} activeStyle={activeStyle} exact>
          Locations
        </NavLink>
        </div>
      </nav>
    </div>
  );
};

export default Header;
