import React from 'react';
import { Link } from 'react-router-dom';
import './SignUp.css';

const SignUpConfirmation = () => {
    return (
    <div className="App">
        <div className = "sign-up-confirmation">
            <h2 className = "confirmation-title">Account created successfully!</h2>
            <p className = "message-for-confirmation">Vă rugăm să vă autentificați pentru a continua.</p>
            <Link to="/login" className="login-button">Autentificare</Link>
        </div>
    </div>
    );
}

export default SignUpConfirmation;
