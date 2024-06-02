import { useState } from "react";
import axios from "../utils/axios-service";
import FormInput from "../Components/Reusable/FormInput";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

function RegisterPage() {
  const [userValues, setUserValues] = useState({
    firstName: "",
    lastName: "",
    username: "",
    email: "",
    password: "",
    passwordConfirm: "",
  });
  const navigate = useNavigate();
  const dispatch = useDispatch();

  const inputs = [
    {
      id: 1,
      name: "firstName",
      type: "text",
      placeholder: "e.g. Stephen",
      errorMessage:
        "First Name should be at least 7 characters and shouldn't include any special character!",
      label: "First Name",
      pattern: "^[A-Za-z]{2,40}$",
      required: true,
    },
    {
      id: 2,
      name: "lastName",
      type: "text",
      placeholder: "e.g. Stephen",
      errorMessage:
        "Last Name should be at least 7 characters and shouldn't include any special character!",
      label: "Last Name",
      pattern: "^[A-Za-z]{2,40}$",
      required: true,
    },
    {
      id: 3,
      name: "username",
      type: "text",
      placeholder: "e.g. stephen.king",
      errorMessage:
        "Username should be 3-16 characters and shouldn't include any special character!",
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
      pattern: ".+@gmail.com|.+@email.com|.+@yahoo.com",
      label: "Email",
      required: true,
    },
    {
      id: 5,
      name: "password",
      type: "password",
      placeholder: "password",
      errorMessage:
        "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
      label: "Password",
      pattern:
        "^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&*._])[a-zA-Z0-9!@#$%^&*]{6,}$",
      required: true,
    },
    {
      id: 6,
      name: "passwordConfirm",
      type: "password",
      placeholder: "confirm password",
      errorMessage: "Password confirmation does not match!",
      label: "Confirm Password",
      pattern: userValues.password || "",
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
      const response = await axios.post("/api/registration", userValues);
      if (response.data.message === "User created successfully!") {
        dispatch(
          openSnackbar({
            message: "Account created successfully! Please login to continue.",
            severity: "success",
          })
        );
        navigate("/auth/login");
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
        <h1 className = "text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Sign Up</h1>
        {inputs.map((input) => (
          <FormInput
            key={input.id}
            {...input}
            value={userValues[input.name]}
            onChange={onChange}
          />
        ))}
        <button className="w-3/4 h-10 px-2 ml-7 bg-[#0a2d2e] text-[#efd7cf] border-none rounded-md font-bold text-lg cursor-pointer mt-5 hover:bg-[#879693] mb-2.5">Submit</button>
      </form>
    </div>
  );
}

export default RegisterPage;
