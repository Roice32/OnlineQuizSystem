import { useEffect } from 'react';
import axios from 'axios';
import { useNavigate, useParams } from 'react-router-dom';

const Logout = () => {
    const navigate = useNavigate();
    const { id } = useParams(); 

    useEffect(() => {
        const logout = async () => {
            try {
                const response = await axios.post(`https://localhost:7117/api/profile/${id}/logout`);
                localStorage.removeItem('authToken');
                navigate('/login');
            } catch (error) {
                console.error('Eroare la logout:', error);
            }
        };

        logout();
    }, [id, navigate]); 
}

export default Logout;
