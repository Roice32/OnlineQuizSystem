import { useRef, useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';


function SignIn() {
    
    const [ values, setValues] = useState({
        username: "",
        password: "",
    });
    const history = useNavigate();
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
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: "Password",
            required: true,
        },
    ];



    const handleLogin = async (e) => {
    e.preventDefault(); 
    const { username, password } = values; 

    try {
        const response = await axios.post(
            'https://localhost:7117/api/Authentication/login',
            { username, password }
        );

        console.log(response);
        console.log(response.data);
        console.log(response.data.message);
        setResponse(response.data);

        if (response.data.status == 1) {
            localStorage.setItem('jwt', response.data.token);
            history('/afterLogin');
        }
        
    } catch (error) {
        console.error('Login failed', error);
    }
};

    const onChange = (e) => {setValues({ ...values, [e.target.name]: e.target.value})};

    console.log(values);

    return ( <div className="App">
        <form onSubmit={handleLogin}>
            <h1>Sign In</h1>
            {response && (
                <div className="response">
                     {response.message}
                </div>
            )}
            {inputs.map((input) => (
                <FormInput key={input.id} {...input} value={values[input.name]} onChange={onChange}/>
            ))}
            <button>Submit</button>
        </form>
    </div>);
}

export default SignIn;