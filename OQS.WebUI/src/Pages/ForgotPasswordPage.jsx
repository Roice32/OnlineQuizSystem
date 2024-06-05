import { useState } from "react";
import axios from "../utils/axios-service";
import FormInput from "../Components/Reusable/FormInput";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

function ForgotPasswordPage() {
    const [ userValues, setUserValues] = useState({
        username: "",
        email: "",
    });
    const navigate = useNavigate();
    const dispatch = useDispatch();

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

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });
      };

    const handleChange = async (e) => {
        e.preventDefault();
        const { username, email } = userValues;
        try{
            const response = await axios.post('/api/forgot_password',{ username, email });
            if(response.data.message ===  "Email sent successfully!")
            {
                dispatch(
                    openSnackbar({
                      message: "Email sent successfully! Please check your email for further instructions.",
                      severity: "success",
                    })
                  );
                navigate("/auth/login"); // aici cred ca ar trebui navigarea catre home page
            }
            else
            {
                dispatch(
                    openSnackbar({
                      message: response.data.message,
                      severity: "error",
                    })
                );
            }

        } catch(error){
            console.error(error);
        }
    }

    const handleClose = () => {
        navigate('/auth/login');
    };

    return ( <div className="flex items-center justify-center min-h-[100vh] p-12 bg-[#6a8e8f]">
        <form className="bg-[#f7ebe7]  px-24 py-5 rounded-3xl shadow-lg border-2 border-[#6a8e8f]" onSubmit={handleChange}>
            <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Forget Password</h1>
            <p className="mb-5 w-80 text-center">Enter your username and email for password reset. You will receive instructions via email.</p>
            {inputs.map((input) => (
                <FormInput key={input.id} {...input} value={userValues[input.name]} onChange={onChange}/>
            ))}
            <div className="flex">
                <button type="button" className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#6a8e8f] text-[#efd7cf] text-center border-none rounded-md font-bold text-lg hover:bg-[#879693]" onClick={handleClose} >Close</button>
                <button type="submit" className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#0a2d2e] text-[#efd7cf] text-center border-none rounded-md font-bold text-lg hover:bg-[#879693]">Reset</button>
            </div>
        </form>
    </div>);
}

export default ForgotPasswordPage;