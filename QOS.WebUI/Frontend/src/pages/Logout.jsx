import { useEffect } from 'react';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router-dom';

const Logout = () => {
    const navigate = useNavigate();
    const userId = localStorage.getItem('userId');

    useEffect(() => {
        const logout = async () => {
            try {
                const token = localStorage.getItem('authToken');
                const response = await axios.post(`https://localhost:7117/api/profile/${userId}/logout`, 
                {
                    headers: {
                        'Authorization': `Bearer ${token}`
                    }
                });
                localStorage.removeItem('authToken');
                localStorage.removeItem('userId');
                localStorage.removeItem('role');
                navigate('/login');
            } catch (error) {
                console.error('Eroare la logout:', error);
            }
        };

        logout();
    }, [userId, navigate]); 
}

export default Logout;
