import { useState } from "react";
import axios from "../utils/axios-service";
import FormInput from "../Components/Reusable/FormInput";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import { useSelector } from "react-redux";
import { RootState } from "../redux/store";
import useAuth from "../hooks/UseAuth";
import ProfileNavbar from "../Components/ProfileNavbar";

function RegisterPage() {
  const [adminValues, setUserValues] = useState({
    firstName: "",
    lastName: "",
    username: "",
    email: "",
    password: "",
    passwordConfirm: "",
  });
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const user = useAuth();
    const userState = useSelector((state: RootState) => state.user);

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
      pattern: adminValues.password || "",
      required: true,
    },
  ];

  const onChange = (e) => {
    const { name, value } = e.target;
    setUserValues({ ...adminValues, [name]: value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
        const token = userState.user?.token;
        const response = await axios.post("api/add_admin", adminValues, 
        {
            headers: {
                'Authorization': `Bearer ${token}`
            }
      });
      if (response.data.message === "Admin created successfully!") {
        dispatch(
          openSnackbar({
            message: "Admin created successfully!",
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
    <div className="flex flex-row bg-[#6a8e8f]">
      <div>
        <ProfileNavbar />
      </div>
    <div className="flex flex-col justify-center items-center w-full text-xl text-[#0a2d2e] py-10">
        <form className="bg-[#f7ebe7] px-24 py-5 rounded-3xl shadow-lg border-2 border-[#6a8e8f]" onSubmit={handleSubmit}>
            <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Add Admin</h1>
            {inputs.map((input) => (
              <FormInput
                key={input.id}
                {...input}
                value={adminValues[input.name]}
                onChange={onChange}
              />
            ))}
            <button className="w-3/4 h-10 px-2 ml-7 bg-[#0a2d2e] text-[#efd7cf] border-none rounded-md font-bold text-lg cursor-pointer mt-5 hover:bg-[#879693] mb-2.5">Submit</button>
          </form>
        </div>
    </div>
  );
}

export default RegisterPage;
