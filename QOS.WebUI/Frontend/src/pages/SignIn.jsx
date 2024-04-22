import { useRef, useState } from 'react';
import './SignUp.css';
import FormInput from '../components/FormInput';

function SignIn() {
    const [ values, setValues] = useState({
        username: "",
        password: "",
    });

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
            name: "password",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: "Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
    ];

    const handleSubmit = (e) => {
        e.preventDefault();
    }

    const onChange = (e) => {setValues({ ...values, [e.target.name]: e.target.value})};

    console.log(values);

    return ( <div className="App">
        <form onSubmit={handleSubmit}>
            <h1>Sign In</h1>
            {inputs.map((input) => (
                <FormInput key={input.id} {...input} value={values[input.name]} onChange={onChange}/>
            ))}
            <button>Submit</button>
        </form>
    </div>);
}

export default SignIn;