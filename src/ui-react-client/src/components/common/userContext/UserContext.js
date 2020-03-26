import React from "react";
import {msalApp} from "../../../config/authUtils";

export const UserContext = React.createContext({account: {}});

export const Provider = UserContext.Provider;
export const Consumer = UserContext.Consumer;

export const USERCONTEXTKEY = "userContext";

class UserProvider extends React.Component {
    state = {
        profile: {}
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
    return !(Object.keys(profile).length === 0)
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

export { UserProvider, Consumer as UserContextConsumer };
