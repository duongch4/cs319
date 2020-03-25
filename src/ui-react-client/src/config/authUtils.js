import { UserAgentApplication } from "msal";
import { UI_ROOT, AUTHORITY, CLIENT_ID, API_ID } from "./config.js";

export const requiresInteraction = errorMessage => {
    if (!errorMessage || !errorMessage.length) {
        return false;
    }

    return (
        errorMessage.indexOf("consent_required") > -1 ||
        errorMessage.indexOf("interaction_required") > -1 ||
        errorMessage.indexOf("login_required") > -1
    );
};

export const fetchMsGraph = async (url, accessToken) => {
    const response = await fetch(url, {
        headers: {
            Authorization: `Bearer ${accessToken}`
        }
    });

    return response.json();
};

export const isIE = () => {
    const ua = window.navigator.userAgent;
    const msie = ua.indexOf("MSIE ") > -1;
    const msie11 = ua.indexOf("Trident/") > -1;

    // If you as a developer are testing using Edge InPrivate mode, please add "isEdge" to the if check
    // const isEdge = ua.indexOf("Edge/") > -1;

    return msie || msie11;
};

export const GRAPH_SCOPES = {
    OPENID: "openid",
    PROFILE: "profile",
    USER_READ: "User.Read",
    MAIL_READ: "Mail.Read",
    API_ADMIN: API_ID + "Admin",
    API_REGULAR: API_ID + "Regular"
};

export const GRAPH_ENDPOINTS = {
    ME: "https://graph.microsoft.com/v1.0/me",
    MAIL: "https://graph.microsoft.com/v1.0/me/messages"
};

export const GRAPH_REQUESTS = {
    LOGIN: {
        scopes: [
            GRAPH_SCOPES.OPENID,
            GRAPH_SCOPES.PROFILE,
            GRAPH_SCOPES.USER_READ
        ]
    },
    API_ADMIN: {
        scopes: [
            GRAPH_SCOPES.OPENID,
            GRAPH_SCOPES.PROFILE,
            GRAPH_SCOPES.API_ADMIN,
            GRAPH_SCOPES.API_REGULAR
        ]
    },
    API_REGULAR: {
        scopes: [
            GRAPH_SCOPES.OPENID,
            GRAPH_SCOPES.PROFILE,
            GRAPH_SCOPES.API_REGULAR
        ]
    },
    EMAIL: {
        scopes: [GRAPH_SCOPES.MAIL_READ]
    }
};

export const msalApp = new UserAgentApplication({
    auth: {
        redirectUri: UI_ROOT,
        clientId: CLIENT_ID,
        authority: AUTHORITY,
        validateAuthority: true,
        postLogoutRedirectUri: UI_ROOT,
        navigateToLoginRequestUrl: false
    },
    cache: {
        // cacheLocation: "localStorage",
        cacheLocation: "sessionStorage",
        storeAuthStateInCookie: isIE()
    },
    system: {
        navigateFrameWait: 0,
        logger: {
            error: console.error,
            errorPii: console.error,
            info: console.log,
            infoPii: console.log,
            verbose: console.log,
            verbosePii: console.log,
            warning: console.warn,
            warningPii: console.warn
        }
    }
});

export const acquireToken = async (tokenReqScopes, redirect) => {
    return msalApp.acquireTokenSilent(tokenReqScopes).catch(error => {
        // Call acquireTokenPopup (popup window) in case of acquireTokenSilent failure
        // due to consent or interaction required ONLY
        if (requiresInteraction(error.errorCode)) {
            console.log(redirect);
            return redirect
                ? msalApp.acquireTokenRedirect(tokenReqScopes)
                : msalApp.acquireTokenPopup(tokenReqScopes);
        } else {
            console.error('Non-interactive error:', error.errorCode)
        }
    });
};

export const getHeaders = async (userRoles) => {
    try {
        const tokenRequest = (userRoles.includes("adminUser")) ? GRAPH_REQUESTS.API_ADMIN : GRAPH_REQUESTS.API_REGULAR;
        const tokenResponse = await acquireToken(tokenRequest, isIE());
        // console.log("MY TOKEN RESPONSE", tokenResponse); // TODO: Lots of Info here!!!
        return { Authorization: `Bearer ${tokenResponse.accessToken}` };
    }
    catch(error) {
        throw error;
    }
}
