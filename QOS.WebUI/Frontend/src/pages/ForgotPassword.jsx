import { useParams, useNavigate } from 'react-router-dom';
import { useRef, useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';
import axios from 'axios';
import { Link } from 'react-router-dom';

function ForgotPassword() {
    const { userId } = useParams();

    const [ values, setValues] = useState({
        username: "",
        email: "",
    });
    const navigate = useNavigate();
    const [response, setResponse] = useState(null);

    const inputs = [
        {
            id: 1,
            name: "username",
            type: "text",
            placeholder: "e.g. stephen.king",
            errorMessage: "Username should be 3-16 characters and shouldn't include any special character!",
            label: "Username",
            required: true,
        },
        {
            id: 2,
            name: "email",
            type: "email",
            placeholder: "e.g. stephen.king@gmail.com",
            errorMessage: "It should be a valid email address!",
            pattern:".+@gmail\.com|.+@email\.com|.+@yahoo\.com",
            label: "Email",
            required: true,
        },
    ];



    const handleLogin = async (e) => {
    e.preventDefault(); 
    const { username, email } = values; 

    try {
        const response = await axios.post(
            'https://localhost:7117/api/forgot_password',
            { username, email }
        );

        setResponse(response.data);

        if (response.data.message != null) {
            navigate('/waiting_to_reset_password');
        }
        
    } catch (error) {
        console.error('Reset password failed', error);
    }
    };

    const handleClose = () => {
        navigate('/login');
    };

    const onChange = (e) => {setValues({ ...values, [e.target.name]: e.target.value})};

    return ( <div className="App">
        <form onSubmit={handleLogin}>
            <h1>Forget Password</h1>
            <p className = "little-message">Enter your username and email for password reset. You will receive instructions via email.</p>
            {response && (
                <div className="response">
                     {response.message}
                </div>
            )}
            {inputs.map((input) => (
                <FormInput key={input.id} {...input} value={values[input.name]} onChange={onChange}/>
            ))}
            <div className = "buttons">
                <button type="button" className = "button-in-row-close" onClick={handleClose} >Close</button>
                <button type="submit" className = "button-in-row-reset">Reset</button>
            </div>
        </form>
    </div>);
}

export default ForgotPassword;