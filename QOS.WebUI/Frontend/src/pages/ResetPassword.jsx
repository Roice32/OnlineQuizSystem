import { useParams, useNavigate } from 'react-router-dom';
import { useRef, useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';
import axios from 'axios';
import { Link } from 'react-router-dom';

const ResetPassword = (props) => {
    const { userId } = useParams();

    const [ values, setValues] = useState({
        newPassword: "",
        confirmPassword: "",
    });
    const navigate = useNavigate();
    const [response, setResponse] = useState(null);
    const { token } = useParams();
    const decodedToken = decodeURIComponent(token);

    const inputs = [
        {
            id: 1,
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: " New Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
        {
            id: 2,
            name: "passwordConfirm",
            type: "password",
            placeholder: "confirm password",
            errorMessage: "Password confirmation does not match!",
            label: "Confirm Password",
            pattern : values.password || "",
            required: true,
        },
    ];


    const handleSubmit = async (event) => {
        event.preventDefault();

        if (password !== confirmPassword) {
            alert('Parolele nu se potrivesc!');
            return;
        }

        try {
            await axios.post(`https://localhost:7117/api/resetPassword/:token`, { password });
            alert('Parola a fost resetată cu succes!');
        } catch (error) {
            alert('A apărut o eroare. Te rog să încerci din nou.');
        }
    };
    

    const onChange = (e) => {setValues({ ...values, [e.target.name]: e.target.value})};

    return ( <div className="App">
        <form onSubmit={handleSubmit}>
            <h1>Reset Password</h1>
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

export default ResetPassword;