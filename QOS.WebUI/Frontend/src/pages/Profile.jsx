import { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate} from 'react-router-dom';
import Navbar from '../components/navBarProfile';
import '../components/navBarProfile.css';
import './EditProfile.css'; 


const UserProfile = () => {
  const navigate = useNavigate();
  const userId = localStorage.getItem('userId');
  const [userData, setUserData] = useState(null);

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
        setUserData(response.data); 

      } catch (error) {
        console.error('Failed to fetch username:', error);
      }
    }

    fetchUsername();
  }, []);

  const deleteAccount = async () => {
    try {
      await axios.delete(`https://localhost:7117/api/profile/${userId}/delete_account`);
      localStorage.removeItem('authToken');
      localStorage.removeItem('userId');
      localStorage.removeItem('role');
      navigate('/'); 
    } catch (error) {
      console.error('Failed to delete account:', error);
    }
  };

  const resetPassword = async () => {
    navigate(`/profile/verify_current_password`);
  }

  const editProfile = async () => {
    navigate(`/profile/edit_profile`);
  }

  return (
    <div className="edit-profile">
      <div className="navbar">
        <Navbar />
      </div>
      <div className="page">
        <h1>User Account Details</h1>
        <div className="page-info">
          <div className = "profile">
            <div className="profile-info">
            <div className="name-field">
              <label>First Name</label>
              <div className="user-item">{userData?.firstName}</div> 
            </div>
            <div className="name-field">
              <label>Last Name</label>
              <div className="user-item">{userData?.lastName}</div> 
            </div>
            <div className="name-field">
              <label>Username</label>
              <div className="user-item">{userData?.username}</div> 
            </div>
            <div className="name-field">
              <label>Email</label>
              <div className="user-item">{userData?.email}</div> 
            </div>
            <div className="name-field">
              <label>Password</label>
              <div className="user-item">********</div>
            </div>
            </div>
            
          </div>
          <div className = "buttons">
          <button className="button" onClick={editProfile}>Edit Profile</button>
          <button className="button" onClick={resetPassword}>Reset Password</button>
          <button className="button" onClick={deleteAccount} >Delete Account</button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default UserProfile;
