import React from 'react';
import Navbar from '../components/navBarProfile'; // Asigură-te că calea este corectă în funcție de structura proiectului tău
import '../pages/Profile.css';

const ProfilePage = () => {
    return (
        <div className = "profile-page">
            <div className = "navbar">
                <Navbar /> 
            </div>
            <div className="content">
                <p>Welcome to your profile!</p>
            </div>
        </div>
    );
}

export default ProfilePage;
