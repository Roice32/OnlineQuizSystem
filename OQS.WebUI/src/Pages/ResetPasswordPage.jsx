import { useState } from "react";
import axios from "../utils/axios-service";
import FormInput from "../Components/Reusable/FormInput";
import { useNavigate, useParams } from "react-router-dom";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

function ResetPasswordPage() {
    const [userValues, setUserValues] = useState({
        username: "",
        newPassword: "",
        confirmPassword: "",
    });
    const navigate = useNavigate();
    const { token } = useParams();
    const dispatch = useDispatch();

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
        try{
            console.log(token)
            console.log(encodeURIComponent(token))
            const response = await axios.post(`/api/resetPassword/${encodeURIComponent(token)}`, { newPassword, username});
            console.log(response.data.message)

            if(response.data.message === "Password reset successfully!!"){
                dispatch(
                    openSnackbar({
                      message: "Password reset successfully! Please login with your new password.",
                      severity: "success",
                    })
                  );
                navigate('/auth/login');
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
        }
        catch(error)
        {
            console.error(error);
        }
    }

    const handleClose = () => {
        navigate('/auth/login'); // sau spre homepage 
     };

    return (
        <div className="flex items-center justify-center min-h-[100vh] p-12 bg-[#6a8e8f]">
            <form className="bg-[#f7ebe7]  px-24 py-5 rounded-3xl shadow-lg border-2 border-[#6a8e8f]" onSubmit={handleSubmit}>
                <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Reset Password</h1>
                <p className="mb-5 w-80 text-center">Enter your new password and confirm it.</p>
                {inputs.map((input) => (
                    <FormInput key={input.id} {...input} value={userValues[input.name]} onChange={onChange}/>
                ))}
                <div className="flex">
                    <button type="button" className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#6a8e8f] text-[#efd7cf] border-none rounded-md font-bold text-lg hover:bg-[#879693]" onClick={handleClose} >Close</button>
                    <button type="submit" className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#0a2d2e] text-[#efd7cf] border-none rounded-md font-bold text-lg hover:bg-[#879693]">Reset</button>
                </div>
            </form>
        </div>
    );
}
export default ResetPasswordPage;