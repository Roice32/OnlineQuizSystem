import React, { useState } from 'react';
import './EditProfile.css'; 
import profilePic from '../logo.svg';
import axios from 'axios';
import Navbar from '../components/navBarProfile';
import '../components/navBarProfile.css';


function EditProfile() {
  const [firstName, setFirstName] = useState('firstName');
  const [lastName, setLastName] = useState('lastName');
  const [username, setUsername] = useState('username');
  const [email, setEmail] = useState('your@email.com');
  const [password, setPassword] = useState('yourPassword');
  const [profilePicture, setProfilePicture] = useState(profilePic);
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [isEditing, setIsEditing] = useState(false); 

  const handleFirstNameChange = (event) => {
    setFirstName(event.target.value);
  };

  const handleLastNameChange = (event) => {
    setLastName(event.target.value);
  };

  const handleUsernameChange = (event) => {
    setUsername(event.target.value);
  };

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const handleProfilePictureChange = (event) => {
    const file = event.target.files[0];
    const reader = new FileReader();

    reader.onloadend = () => {
      setProfilePicture(reader.result);
    };

    if (file) {
      reader.readAsDataURL(file);
    }
  };

  const handleOldPasswordChange = (event) => {
    setOldPassword(event.target.value);
  };

  const handleNewPasswordChange = (event) => {
    setNewPassword(event.target.value);
  };

  const handleUpdateProfile = () => {
    const token = localStorage.getItem('token'); // Retrieve the JWT token from local storage
    axios.put('/api/profile', {
      oldPassword,
      newPassword,
      firstName,
      lastName,
      username,
      email,
      password,
      profilePicture,
    }, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}` 
      }
    })
    .then(response => console.log(response.data))
    .catch(error => console.error('Error:', error));
  };

  return (
    <div className="edit-profile">
      <div className = "navbar">
        <Navbar />
      </div>
      <div className = "page">
      <div className="profile">
        <div className="profile-picture">
          <img src={profilePicture} alt="Profile" />
          <input type="file" onChange={handleProfilePictureChange} accept="image/*" />
        </div>
        <div className="profile-info">
          <div className="name-field">
            <label>First Name</label>
            <input type="text" value={firstName} onChange={handleFirstNameChange} />
          </div>
          <div className="name-field">
            <label>Last Name</label>
            <input type="text" value={lastName} onChange={handleLastNameChange} />
          </div>
          <div className="username-field">
            <label>Username</label>
            <input type="text" value={username} onChange={handleUsernameChange} />
          </div>
          <div className="email-field">
            <label>Email</label>
            <input type="email" value={email} onChange={handleEmailChange} />
          </div>
          <div className="password-field">
            <label>Password</label>
            <input type="password" value={password} onChange={handlePasswordChange} />
          </div>
          {isEditing && (
            <>
              <div className="password-field">
                <label>Old Password</label>
                <input type="password" value={oldPassword} onChange={handleOldPasswordChange} />
              </div>
              <div className="password-field">
                <label>New Password</label>
                <input type="password" value={newPassword} onChange={handleNewPasswordChange} />
              </div>
            </>
          )}
        </div>
        <button className="edit-button" onClick={() => setIsEditing(!isEditing)}>
          {isEditing ? 'Save Profile' : 'Edit Profile'}
        </button>
      </div>
      </div>
    </div>
  );
}

export default EditProfile;