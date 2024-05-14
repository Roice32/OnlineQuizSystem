import { useParams, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';
import axios from 'axios';

const ResetPassword = () => {

    const [userValues, setUserValues] = useState({
        username: "",
        newPassword: "",
        confirmPassword: "",
    });
    const navigate = useNavigate();
    const [response, setResponse] = useState(null);
    const { token } = useParams();

    const inputs = [
        {
            id: 1,
            name: "username",
            type: "text",
            placeholder: "e.g. stephen.king",
            errorMessage: "Username should be 3-16 characters and shouldn't include any special character!",
            label: "Username",
            pattern: "^(?=.*[A-Za-z]{3,})[A-Za-z0-9!@#$%^&*]{3,16}$",
            required: true,
        },
        {
            id: 2,
            name: "newPassword",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: " New Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
        {
            id: 3,
            name: "confirmPassword",
            type: "password",
            placeholder: "confirm password",
            errorMessage: "Password confirmation does not match!",
            label: "Confirm Password",
            pattern : userValues.newPassword || "",
            required: true,
        },
    ];

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });    
    };


    const handleSubmit = async (e) => {
        e.preventDefault();
        const { username, newPassword, confirmPassword } = userValues;
        console.log(token);
        console.log(encodeURIComponent(token));

        try {
            const response = await axios.post(`https://localhost:7117/api/resetPassword/${encodeURIComponent(token)}`, { newPassword, username});
            console.log(response.data.message);

            if(response.data.message === "Password reset successfully!!"){
                navigate('/login');
            }
            
        } catch (error) {
            alert('A apărut o eroare. Te rog să încerci din nou.');
        }
    };


    const handleClose = () => {
       navigate('/login');
    };

    return (
        <div className="App">
            <form onSubmit={handleSubmit}>
                <h1>Reset Password</h1>
                <p className="little-message">Enter your new password and confirm it.</p>
                {response && (
                    <div className="response">
                        {response.message}
                    </div>
                )}
                {inputs.map((input) => (
                    <FormInput key={input.id} {...input} value={userValues[input.name]} onChange={onChange}/>
                ))}
                <div className="buttons">
                    <button type="button" className="button-in-row-close" onClick={handleClose} >Close</button>
                    <button type="submit" className="button-in-row-reset">Reset</button>
                </div>
            </form>
        </div>
    );
}

export default ResetPassword;
