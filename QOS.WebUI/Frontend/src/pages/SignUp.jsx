import { useRef, useState } from 'react';
import './SignUp.css';
import axios from 'axios';
import FormInput from '../components/FormInput';
import { useNavigate } from 'react-router-dom';

function SignUp() {
    const [userValues, setUserValues] = useState({
        firstName: "",
        lastName: "",
        username: "",
        email: "",
        password: "",
        passwordConfirm: "",
    });
    const navigate = useNavigate();
    const [response, setResponse] = useState(null);

    const inputs = [
        {
            id: 1,
            name: "firstName",
            type: "text",
            placeholder: "e.g. Stephen",
            errorMessage: "First Name should be at least 7 characters and shouldn't include any special character!",
            label: "First Name",
            pattern: "^[A-Za-z]{2,40}$",
            required: true,
        },
        {
            id: 2,
            name: "lastName",
            type: "text",
            placeholder: "e.g. Stephen",
            errorMessage: "Last Name should be at least 7 characters and shouldn't include any special character!",
            label: "Last Name",
            pattern: "^[A-Za-z]{2,40}$",
            required: true,
        },
        {
            id: 3,
            name: "username",
            type: "text",
            placeholder: "e.g. stephen.king",
            errorMessage: "Username should be 3-16 characters and shouldn't include any special character!",
            label: "Username",
            pattern: "^(?=.*[A-Za-z]{3,})[A-Za-z0-9!@#$%^&*]{3,16}$",
            required: true,
        },
        {
            id: 4,
            name: "email",
            type: "email",
            placeholder: "e.g. stephen.king@gmail.com",
            errorMessage: "It should be a valid email address!",
            pattern:".+@gmail\.com|.+@email\.com|.+@yahoo\.com",
            label: "Email",
            required: true,
        },
        {
            id: 5,
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: "Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
        {
            id: 6,
            name: "passwordConfirm",
            type: "password",
            placeholder: "confirm password",
            errorMessage: "Password confirmation does not match!",
            label: "Confirm Password",
            pattern : userValues.password || "",
            required: true,
        },
       
    ];

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });    
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await axios.post('https://localhost:7117/api/registration', userValues);
            setResponse(response.data);
            if(response.data.message === "User created successfully!"){
                navigate('/sign_up_confirmation');
            }

        } catch (error) {
            console.error('Error:', error);
        }
    };

    return (
        <div className="App">
            <form onSubmit={handleSubmit}>
                <h1>Sign Up</h1>
                {response && (
                <div className="response">
                     {response.message}
                </div>
            )}
                {inputs.map((input) => (
                    <FormInput key={input.id} {...input} value={userValues[input.name]} onChange={onChange}/>
                ))}
                <button>Submit</button>
            </form>
        </div>
    );
}



export default SignUp;
