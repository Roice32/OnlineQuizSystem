import { useRef, useState } from "react";
import "./SignUP.css";
import FormInput from "../../Components/Reusable/FormInput/FormInput";
import axios from "../../utils/axios-service";
import { useNavigate } from "react-router-dom";
import { Link } from "react-router-dom";
import { useCookies } from "react-cookie";
import { useDispatch } from "react-redux";
import { setUser } from "../../redux/User/UserState";
import { openSnackbar } from "../../redux/Snackbar/SnackbarState";
import { User } from "../../utils/types/user";

function LoginPage() {
  const [values, setValues] = useState({
    username: "",
    password: "",
  });
  const [cookies, setCookie, removeCookie] = useCookies();
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const inputs = [
    {
      id: 1,
      name: "username",
      type: "text",
      placeholder: "e.g. stephen.king",
      errorMessage:
        "Username should be 3-16 characters and shouldn't include any special character!",
      label: "Username",
      required: true,
    },
    {
      id: 2,
      name: "password",
      type: "password",
      placeholder: "password",
      errorMessage:
        "Password should have at least 6 characters and include at least 1 lowercase, 1 uppercase, 1 number and 1 non-aplhabetical character!",
      label: "Password",
      required: true,
    },
  ];

  const handleLogin = async (e) => {
    e.preventDefault();
    const { username, password } = values;
    try {
      const response = await axios.post("/api/authentication", {
        username,
        password,
      });
      /*  setResponse(response.data); */
      if (response.data.token != null) {
        const idResponse = (
          await axios.get("/api/id", {
            headers: { Authorization: response.data.token },
          })
        ).data;
        const detailsResponse = (
          await axios.get(`/api/profile/${idResponse.id}`, {
            headers: { Authorization: response.data.token },
          })
        ).data;

        const user: User = {
          id: idResponse.id,
          username: detailsResponse.username,
          email: detailsResponse.email,
          token: response.data.token,
          firstName: detailsResponse.firstName,
          lastName: detailsResponse.lastName,
          role: idResponse.role,
        };
        setCookie("token", user.token, { path: "/" });
        dispatch(setUser(user));
        dispatch(
          openSnackbar({
            message: `Welcome back ${user.username}!`,
            severity: "success",
          })
        );
        navigate("/quiz");
      } else {
        dispatch(
          openSnackbar({ message: response.data.message, severity: "error" })
        );
      }
    } catch (error: unknown) {
      console.error(error);
      dispatch(
        openSnackbar({
          message: "We encountered an unexpected error",
          severity: "error",
        })
      );
      removeCookie("token", { path: "/" });
    }
  };

  const onChange = (e) => {
    setValues({ ...values, [e.target.name]: e.target.value });
  };

  return (
    <div className="App">
      <form onSubmit={handleLogin}>
        <h1>Sign In</h1>
        {inputs.map((input) => (
          <FormInput
            key={input.id}
            {...input}
            value={values[input.name]}
            onChange={onChange}
          />
        ))}
        <Link className="forgot-password-link" to="/forgot_password">
          Forgot Password?
        </Link>
        <button>Submit</button>
      </form>
    </div>
  );
}

export default LoginPage;
