import React from 'react';
import { authContext } from '../../config/adalConfig';

const UserHeaderMenu = () => {
  return (
    <nav>
      <button
        type="submit"
        className="logout-button"
        onClick={() => authContext.logOut()}
      >
        Log out
      </button>
    </nav>
  );
};

export default UserHeaderMenu;
