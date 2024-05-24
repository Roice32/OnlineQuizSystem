import React from 'react';
import { Link } from 'react-router-dom';
import '../components/navBarProfile.css';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';

const Navbar = () => {
  const role = localStorage.getItem('role');

  return (
    <div className="navbar-item">
      <div className="profile-item">
        <Link className="profile-link" to={`/profile`}>Profile</Link>
      </div>
      <div className="menu">
        {role === 'User' && (
          <>
            <li className="nav-item">
                <Link className="nav-link" to="/about">My quizzes</Link>
            </li>
            <li className="nav-item">
                <Link className="nav-link" to="/contact">Contact</Link>
            </li>
          </>
        )}
        {role === 'Admin' && (
          <>
            <li className="nav-item">
              <Link className="nav-link" to="/add_admin">Add Admin</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/profile/get_users">View Users</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/profile/delete_user">Delete User</Link>
            </li>
          </>
        )}
        <li className="nav-item">
          <Link className="nav-link" to="/logout">
            <FontAwesomeIcon icon={faSignOutAlt} /> Logout
          </Link>
        </li>
      </div>
    </div>
  );
}

export default Navbar;
