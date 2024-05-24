import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './EditProfile.css';
import Navbar from '../components/navBarProfile';

const UserList = () => {
    const [users, setUsers] = useState([]);

    useEffect(() => {
        const fetchUsers = async () => {
            try {
                const token = localStorage.getItem('authToken');
                console.log('Token:', token);

                const response = await axios.get('https://localhost:7117/api/getUsers', {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    }
                });

                console.log('Response data:', response.data);

                if (Array.isArray(response.data)) {
                    setUsers(response.data);
                } else {
                    console.error('Expected an array but got:', response.data);
                }
            } catch (error) {
                console.error('Failed to fetch users:', error);
            }
        };

        fetchUsers();
    }, []);

    return (
        <div className="edit-profile">
            <div className="navbar">
                <Navbar />
            </div>
            <div className="container-center-elements">
                <h1>User List</h1>
                <div className="container-peach">
                    <table className="user-table">
                        <thead>
                            <tr>
                                <th>ID</th>
                                <th>First Name</th>
                                <th>Last Name</th>
                                <th>Username</th>
                                <th>Email</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map((user) => (
                                <tr key={user.id}>
                                    <td>{user.id}</td>
                                    <td>{user.firstName}</td>
                                    <td>{user.lastName}</td>
                                    <td>{user.userName}</td>
                                    <td>{user.email}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
};

export default UserList;
