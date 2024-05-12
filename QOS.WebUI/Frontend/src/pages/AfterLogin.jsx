import React, { useEffect, useState } from 'react';
import axios from 'axios';

function UserNamePage() {
    const [username, setUsername] = useState('');

    useEffect(() => {
        async function fetchUsername() {
            try {
                const token = localStorage.getItem('authToken');
                const response = await axios.post('https://localhost:7117/api/username', null, {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                setUsername(response.data);
            } catch (error) {
                console.error('Failed to fetch username:', error);
            }
        }

        fetchUsername();
    }, []);

    return (
        <div>
            <h1>User Name Page</h1>
            <p>Hello, {username || 'stranger'}</p>
        </div>
    );
}

export default UserNamePage;
