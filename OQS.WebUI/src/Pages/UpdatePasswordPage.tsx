import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import FormInput from "../Components/Reusable/FormInput";
import axios from "../utils/axios-service";
import { RootState } from "../redux/store";
import useAuth from "../hooks/UseAuth";
import { useSelector } from "react-redux";

function UpdatePasswordPage() {
    const [userValues, setUserValues] = useState({
        newPassword: "",
        confirmPassword: ""
    });
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const user = useAuth();
    const userState = useSelector((state: RootState) => state.user);

    const inputs = [
        {
            id: 1,
            name: "newPassword",
            type: "password",
            placeholder: "password",
            errorMessage: "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
            label: " New Password",
            pattern: '^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$',
            required: true,
        },
        {
            id: 2,
            name: "confirmPassword",
            type: "password",
            placeholder: "confirm password",
            errorMessage: "Password confirmation does not match!",
            label: "Confirm Password",
            pattern : userValues.confirmPassword || "",
            required: true,
        },
    ];

    const onChange = (e) => {
        const { name, value } = e.target;
        setUserValues({ ...userValues, [name]: value });    
    };

    const handleClose = () => {
        navigate("/profile");
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        const {newPassword, confirmPassword } = userValues;

        try {
            const token = userState.user?.token;
            const response = await axios.put(`/api/profile/${userState.user?.id}/reset_current_password`, { newPassword}, 
            {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            }
            );
            if (response.data.message === "Password reset successfully!!") {
                dispatch(
                    openSnackbar({
                        message: "Password reset successfully!",
                        severity: "success",
                    })
                );
                navigate("/profile");
            } else {
                dispatch(
                    openSnackbar({
                        message: response.data.message,
                        severity: "error",
                    })
                );
            }
        } catch (error) {
            console.error(error);
        }
    };

  
    return (
        <div className="flex items-center justify-center min-h-[100vh] p-12 bg-[#6a8e8f]">
            <form className="bg-[#f7ebe7]  px-24 py-5 rounded-3xl shadow-lg border-2 border-[#6a8e8f]" onSubmit={handleSubmit}>
                <h1 className = "text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Reset Your Password</h1>
                <p className="mb-5 w-80 text-center">Enter your new password and confirm it.</p>
                
                {inputs.map((input) => (
                    <FormInput key={input.id} {...input} value={userValues[input.name]} onChange={onChange}/>
                ))}
                <div className="buttons">
                    <button type="button"className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#6a8e8f] text-[#efd7cf] border-none text-center rounded-md font-bold text-lg hover:bg-[#879693]" onClick={handleClose} >Close</button>
                    <button type="submit" className="mt-5 w-2/5 h-10 p-2 ml-1 bg-[#0a2d2e] text-[#efd7cf] border-none text-center rounded-md font-bold text-lg hover:bg-[#879693]">Reset</button>
                </div>
            </form>
        </div>
    );
}

export default UpdatePasswordPage;