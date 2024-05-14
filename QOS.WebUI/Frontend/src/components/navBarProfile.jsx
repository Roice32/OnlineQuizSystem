import React from 'react';
import { Link } from 'react-router-dom';
import '../components/navBarProfile.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';


const Navbar = () => {
    return (
        <div className="navbar-item">
            <div className="profile-item">
                <Link className = "profile-link" to="/">Profile</Link>
            </div>
            <div className="menu">
                <li className = "nav-item">
                    <Link className = "nav-link" to="/home">Setting</Link>
                </li>
                <li className = "nav-item">
                    <Link className = "nav-link" to="/about">History</Link>
                </li>
                <li className = "nav-item">
                    <Link className = "nav-link" to="/contact">Contact</Link>
                </li>
                <li className = "nav-item-logout">
                    <Link to="/logout" className="nav-link">
                        <FontAwesomeIcon icon={faSignOutAlt} /> Logout
                    </Link>
                </li>
            </div>
        </div>
    );
}

export default Navbar;