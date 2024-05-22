import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

const UserProfile = () => {
  const navigate = useNavigate();
  const [response, setResponse] = useState(null);
  useEffect(() => {
    async function fetchUsername() {
        try {
            const token = localStorage.getItem('authToken');
            const response = await axios.get('https://localhost:7117/api/id', {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            })

            if(response.data != null){
                localStorage.setItem('userId', response.data.id);
                localStorage.setItem('role', response.data.role);

                const url = `/profile`; 
                navigate(url);
            } 


            
        } catch (error) {
            console.error('Failed to fetch username:', error);
        }
    }

    fetchUsername();
}, []);

};

export default UserProfile;
