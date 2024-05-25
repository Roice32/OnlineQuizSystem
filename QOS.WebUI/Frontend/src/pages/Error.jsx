import React from 'react';
import './Error.css';

const Error = () => {
    return (
        <div className="App">
            <div className="container">
                <h1 className="error-title">Oops...</h1>
                <p className="error-message">Page Not Found</p>
                <a href="/" className="error-link">Go to Homepage</a>
            </div>
        </div>
    );
};

export default Error;
