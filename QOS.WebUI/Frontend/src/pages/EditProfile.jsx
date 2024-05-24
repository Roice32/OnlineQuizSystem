import React, { useState, useEffect } from 'react';
import './EditProfile.css'; 
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Navbar from '../components/navBarProfile';
import '../components/navBarProfile.css';

function EditProfile() {
  const navigate = useNavigate();
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const userId = localStorage.getItem('userId');
  const [message, setMessage] = useState('');


  useEffect(() => {
    async function fetchUsername() {
      try { 
        const token = localStorage.getItem('authToken');
        const response = await axios.get(`https://localhost:7117/api/profile/${userId}`, 
        {
          headers: {
              'Authorization': `Bearer ${token}`
          }
      }
        );

        const user = response.data;
        setFirstName(user.firstName);
        setLastName(user.lastName);
        setUsername(user.username);
        setEmail(user.email); 

      } catch (error) {
        console.error('Failed to fetch username:', error);
      }
    }

    fetchUsername();
  }, []);

  const handleUpdateProfile = async () => {
    try {
      const token = localStorage.getItem('authToken');
        const response = await axios.put(`https://localhost:7117/api/profile/${userId}/edit_profile`, {
            firstName,
            lastName,
            username,
            email,
        }, 
        {
          headers: {
              'Authorization': `Bearer ${token}`
          }
      }
      );
      setMessage(response.data.message);
        if (response.data.message === "Details reset successfully!") {
            navigate('/profile');
        }
    } catch (error) {
        console.error('Error updating profile:', error);
    }
};

  return (
    <div className="edit-profile">
      <div className="navbar">
        <Navbar />
      </div>
      <div className="page">
        <div className="profile">
          <div className="profile-info">
          {message && <p className = "error">{message}</p>}
            <div className="name-field">
              <label>First Name</label>
              <input type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
            </div>
            <div className="name-field">
              <label>Last Name</label>
              <input type="text" value={lastName} onChange={(e) => setLastName(e.target.value)} />
            </div>
            <div className="username-field">
              <label>Username</label>
              <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
            </div>
            <div className="email-field">
              <label>Email</label>
              <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
            </div>
          </div>
          <button className="edit-button" onClick={handleUpdateProfile}>Save edit</button>
        </div>
      </div>
    </div>
  );
}

export default EditProfile;
