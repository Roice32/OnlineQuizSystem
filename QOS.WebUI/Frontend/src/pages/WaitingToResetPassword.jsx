import React from 'react';
import { Link } from 'react-router-dom';
import './SignUp.css';

const WaitingToResetPassword = () => {
    return (
    <div className="App">
        <div className = "confirmation-content">
            <h2 className = "confirmation-title">Email sent successfully!</h2>
            <p className = "message-for-confirmation">Please check your email for password reset instructions. If you don’t see the email in your inbox, check your spam folder as well.</p>
            <Link to="/" className="link-button">Back</Link>
        </div>
    </div>
    );
}

export default WaitingToResetPassword;