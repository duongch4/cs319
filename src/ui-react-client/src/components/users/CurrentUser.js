import React from "react";

const Json = ({ data }) => <pre>{JSON.stringify(data, null, 4)}</pre>;

export const CurrentUser = (props) => {
    const currentUser = props.location.state;
    return (
        <div className="activity-container">
            <section className="data">
                {currentUser.account && (
                    <div className="data-account">
                        <h2>Session Account Data</h2>
                        <Json data={currentUser.account} />
                    </div>
                )}
                {currentUser.profile && (
                    <div className="data-graph">
                        <h2>Graph Profile Data</h2>
                        <Json data={currentUser.profile} />
                    </div>
                )}
                {currentUser.emailMessages && (
                    <div className="data-graph">
                        <h2>Messages Data</h2>
                        <Json data={currentUser.emailMessages} />
                    </div>
                )}
            </section>
        </div>
    );
}
