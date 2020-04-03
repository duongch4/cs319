import React, { Component } from "react";
import {
    msalApp,
    acquireToken,
    fetchMsGraph,
    isIE,
    GRAPH_ENDPOINTS,
    GRAPH_SCOPES,
    GRAPH_REQUESTS
} from "./authUtils";
import { UserContextConsumer, USERCONTEXTKEY, UserProvider } from "../components/common/userContext/UserContext";

// If you support IE, our recommendation is that you sign-in using Redirect APIs
const useRedirectFlow = isIE();
// const useRedirectFlow = true;

export default C => (
    class AuthProvider extends Component {
        constructor(props) {
            super(props);

            this.state = {
                account: null,
                error: null,
                emailMessages: null,
                graphProfile: null
            };
        }

        async onSignIn(redirect) {
            if (redirect) {
                return msalApp.loginRedirect(GRAPH_REQUESTS.LOGIN);
            }
            else {
                return await this.handleSignInPopupFlow();
            }
        }

        async componentDidMount() {
            await this.handleSignInRedirectFlow();
        }

        onSignOut() {
            msalApp.logout();
            sessionStorage.removeItem(USERCONTEXTKEY);
        }

        async onRequestEmailToken() {
            const tokenResponse = await acquireToken(
                GRAPH_REQUESTS.EMAIL,
                useRedirectFlow
            ).catch(e => {
                this.setState({
                    error: "Unable to acquire access token for reading email."
                });
            });

            if (tokenResponse) {
                return this.readMail(tokenResponse.accessToken);
            }
        }

        tryReadMail(tokenResponse) {
            if (tokenResponse.scopes.indexOf(GRAPH_SCOPES.MAIL_READ) > 0) {
                return this.readMail(tokenResponse.accessToken);
            }
        }

        async readMail(accessToken) {
            const emailMessages = await fetchMsGraph(
                GRAPH_ENDPOINTS.MAIL,
                accessToken
            ).catch(() => {
                this.setState({
                    error: "Unable to fetch email messages."
                });
            });

            if (emailMessages) {
                this.setState({
                    emailMessages,
                    error: null
                });
            }
        }

        async handleSignInPopupFlow() {
            const loginResponse = await this.getLoginResponsePopup();
            if (loginResponse) {
                await this.setUserInfo(loginResponse.account, false);
            }
        }

        async getLoginResponsePopup() {
            const loginResponse = await msalApp.loginPopup(
                GRAPH_REQUESTS.LOGIN
            ).catch(error => {
                this.setState({
                    error: error.message
                });
            });
            return loginResponse;
        }

        async getTokenResponse(isRedirectOnInteractionError = false) {
            const tokenResponse = await acquireToken(
                GRAPH_REQUESTS.LOGIN, isRedirectOnInteractionError
            ).catch(error => {
                this.setState({
                    error: error.message
                });
            });
            return tokenResponse;
        }

        async handleSignInRedirectFlow() {
            msalApp.handleRedirectCallback(error => {
                if (error) {
                    const errorMessage = error.errorMessage ? error.errorMessage : "Unable to acquire access token.";
                    // setState works as long as navigateToLoginRequestUrl: false
                    this.setState({
                        error: errorMessage
                    });
                }
            });
            const account = msalApp.getAccount();
            if (account) {
                await this.setUserInfo(account, useRedirectFlow);
            }
        }

        async setUserInfo(account, isRedirectOnInteractionError) {
            this.setState({
                account: account,
                error: null
            });
            const tokenResponse = await this.getTokenResponse(isRedirectOnInteractionError);
            if (tokenResponse) {
                const graphProfile = await this.getGraphProfile(tokenResponse);
                this.callUpdateProfileFromContext(graphProfile, account);
                this.tryReadMail(tokenResponse);
            }
        }

        async getGraphProfile(tokenResponse) {
            const graphProfile = await fetchMsGraph(
                GRAPH_ENDPOINTS.ME,
                tokenResponse.accessToken
            ).catch(() => {
                this.setState({
                    error: "Unable to fetch Graph profile."
                });
            });
            return graphProfile;
        }

        callUpdateProfileFromContext(graphProfile, account) {
            if (graphProfile) {
                this.setState({
                    graphProfile
                });

                this.updateProfile({
                    userID: account.accountIdentifier,
                    userRoles: account.idToken.roles,
                    firstName: graphProfile.givenName,
                    lastName: graphProfile.surname
                });
            }
        }

        render() {
            return (
                <UserProvider>
                    <UserContextConsumer>
                        {({ profile, updateProfile }) => {
                            this.updateProfile = updateProfile;
                            return (
                                <C
                                    {...this.props}
                                    account={this.state.account}
                                    emailMessages={this.state.emailMessages}
                                    error={this.state.error}
                                    graphProfile={this.state.graphProfile}
                                    onSignIn={() => this.onSignIn(useRedirectFlow)}
                                    onSignOut={() => this.onSignOut()}
                                    onRequestEmailToken={() => this.onRequestEmailToken()}
                                />
                            )
                        }}
                    </UserContextConsumer>
                </UserProvider>
            );
        }
    }
);
