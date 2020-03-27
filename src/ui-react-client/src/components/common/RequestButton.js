import React from 'react';

export const RequestButton = (props) => {
    return (
        <nav>
            <button
                type="submit"
                className="logout-button"
                onClick={() => (props.useRedirect) ? props.onClick(props.useRedirect) : props.onClick()}
            >{props.text}</button>
        </nav>
    );
};
