import React, { useState } from 'react';
import axios from 'axios';
import Navbar from '../components/navBarProfile';
import './EditProfile.css';

const DeleteUser = () => {
    const [username, setUsername] = useState('');
    const [message, setMessage] = useState('');

    const handleDelete = async () => {
        if (!username) {
            setMessage('Please enter a username');
            return;
        }

       setMessage('');

        try {
            const token = localStorage.getItem('authToken');
            const response = await axios.delete(`https://localhost:7117/api/profile/delete_user`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
                params: { username }
            });

            setMessage(response.data.message);
        } catch (error) {
            setMessage(error.response?.data?.message || 'Failed to delete user');
        } 
    };

    return (
        <div className="edit-profile">
            <div className="navbar">
                <Navbar />
            </div>
            <div className="second-container">
                <div className="little-container">
                    <h1>Delete User</h1>
                    {message && <p className = "error">{message}</p>}
                    <input
                        className="input-field"
                        type="text"
                        value={username}
                        onChange={(e) => setUsername(e.target.value)}
                        placeholder="Enter username"
                    />
                    <button
                        className="button"
                        onClick={handleDelete}
                    >
                        Delete User
                    </button>
                </div>
            </div>
        </div>
    );
};

export default DeleteUser;
