import React from "react";
import {msalApp} from "../../../config/authUtils";

export const UserContext = React.createContext({account: {}});

export const Provider = UserContext.Provider;
export const Consumer = UserContext.Consumer;

export const USERCONTEXTKEY = "userContext";

class UserProvider extends React.Component {
    state = {
        profile: {
            userID: null,
            userRoles: null,
            firstName: null,
            lastName: null
        }
    };

    render() {
        return <Provider
            value={{
                profile: this.state.profile,
                updateProfile: profile => {
                    this.setState({
                        ...this.state,
                        profile: profile
                    });
                    let profileString = JSON.stringify(profile);
                    sessionStorage.setItem(USERCONTEXTKEY, profileString);
                }
            }}>
            {this.props.children}
        </Provider>
    }
}

export const isProfileLoaded = (profile) => {
    for (let key of Object.keys(profile)) {
        if (profile[key] === null || typeof(profile[key]) === "undefined") {
            return false;
        }
    }
    return true;
};

export const fetchProfileFromLocalStorage = () => {
    let profileString = sessionStorage.getItem(USERCONTEXTKEY);
    let profile;
    if (profileString === null) {
        profile = {};
        msalApp.logout();
    } else {
        profile = JSON.parse(profileString);
    }
    return profile;
};

export const getUserRoles = (userContext) => {
    if (!isProfileLoaded(userContext.profile)) {
        let profile = fetchProfileFromLocalStorage();
        userContext.updateProfile(profile);
        return profile.userRoles;
    }
    return userContext.profile.userRoles;
}

export const isAdminUser = (roles) => {
    return roles.includes('adminUser')
}

export { UserProvider, Consumer as UserContextConsumer };
