import React  from 'react';
import { Link } from 'react-router-dom';
import '../components/navBarProfile.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import { useNavigate, useParams } from 'react-router-dom';

const Navbar = () => {

    const navigate= useNavigate();
    const { id } = useParams();

    return (
        <div className="navbar-item">
            <div className="profile-item">
                <Link className = "profile-link" to={`/profile/${id}`}>Profile</Link>
            </div>
            <div className="menu">
                <li className = "nav-item">
                    <Link className = "nav-link" to="/about">My quizzes</Link>
                </li>
                <li className = "nav-item">
                    <Link className = "nav-link" to="/contact">Contact</Link>
                </li>
                <li className = "nav-item">
                    <Link className = "nav-link" to="/home">I want to be an admin</Link>
                </li>
                <li className = "nav-item">
                <Link className="nav-link" to = "/logout">
                        <FontAwesomeIcon icon={faSignOutAlt} /> Logout
                    </Link>
                </li>
            </div>
        </div>
    );
}

export default Navbar;