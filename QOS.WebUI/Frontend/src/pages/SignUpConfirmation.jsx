import React from 'react';
import { Link } from 'react-router-dom';
import './SignUp.css';

const SignUpConfirmation = () => {
    return (
    <div className="App">
        <div className = "confirmation-content">
            <h2 className = "confirmation-title">Account created successfully!</h2>
            <p className = "message-for-confirmation">Vă rugăm să vă autentificați pentru a continua.</p>
            <Link to="/login" className="link-button">Autentificare</Link>
        </div>
    </div>
    );
}

export default SignUpConfirmation;
