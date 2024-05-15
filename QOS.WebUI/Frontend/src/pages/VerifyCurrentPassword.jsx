import { useParams, useNavigate } from 'react-router-dom';
import { useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';
import axios from 'axios';

const VerifyCurrentPassword = () => {

    const [userValues, setUserValues] = useState({
        password: "",
    });
    const navigate = useNavigate();
    const [response, setResponse] = useState(null);
    const { id } = useParams();

    const inputs = [
        {
            id: 1,
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: " Your Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
    ];

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });    
    };


    const handleSubmit = async (e) => {
        e.preventDefault();
        const { password } = userValues; 

        try {
            const response = await axios.post(`https://localhost:7117/api/profile/${id}/verify_current_password`, { password }); 
            setResponse(response.data);

            if(response.data.message === " Confirmed Password !!"){
                navigate(`/profile/${id}/reset_current_password`);
            }
            
            
        } catch (error) {
            alert('A apărut o eroare. Te rog să încerci din nou.');
        }
    };


    const handleClose = () => {
       navigate(`/profile/${id}`);
    };

    return (
        <div className="App">
            <form onSubmit={handleSubmit}>
                <h1>Verify Password</h1>
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
                    <button type="button" className="button-in-row-close" onClick={handleClose}> Close</button>
                    <button type="submit" className="button-in-row-reset" onClick={handleSubmit}> Confirm</button>
                </div>
            </form>
        </div>
    );
}

export default VerifyCurrentPassword;
