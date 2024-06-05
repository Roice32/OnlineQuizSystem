import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { RootState } from "../redux/store";
import { useCookies } from "react-cookie";
import { clearUser } from "../redux/User/UserState";
import axios from "../utils/axios-service";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import React from "react";
import Logo from '../Logo.png'; // Ensure this path is correct

export default function Navbar() {
  const userState = useSelector((state: RootState) => state.user);
  const dispatch = useDispatch();
  const [cookies, setCookie, removeCookie] = useCookies();
  const signOut = async () => {
    try {
      const response = (
        await axios.post(`/api/profile/${userState.user?.id}/logout`)
      ).data;
      console.log(response);
      if (response.message === "User logged out successfully!") {
        removeCookie("token");
        dispatch(clearUser());
      } else {
        throw new Error("Could not sign you out");
      }
    } catch (e) {
      removeCookie("token");
      dispatch(clearUser());
      /* dispatch(
        openSnackbar({ message: "Could not sign you out", severity: "error" })
      ); */
    }
  };

  return (
    <>
      <nav className="flex items-center bg-[#1c4e4f] h-20 p-4 justify-between">
        <div className="flex items-center">
        <img src={Logo} alt="Logo" className="h-20 w-20 mr-4" />

        </div>
        <div className="flex justify-center flex-grow">
          <ul className="flex items-center space-x-4 text-stone-50 list-none">
            <li>
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-[#deae9f]"
                to="/"
              >
                Home
              </Link>
            </li>
            <li>
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-[#deae9f]"
                to="/quizzes"
              >
                Quiz
              </Link>
            </li>
          </ul>
        </div>
        <div className="flex items-center space-x-4">
          {!userState.isLogged && (
            <>
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-[#deae9f]"
                to="/auth/login"
              >
                Login
              </Link>
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-[#deae9f]"
                to="/auth/register"
              >
                Register
              </Link>
            </>
          )}
          {userState.isLogged && (
            <>
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-[#deae9f]"
                to="/profile"
              >
                {userState.user?.username}
              </Link>
              <button
                onClick={signOut}
                className="no-underline text-slate-50 px-2 py-1 text-xl transition duration-300 hover:text-red-600 bg-transparent border-none cursor-pointer"
              >
                Sign out
              </button>
            </>
          )}
        </div>
      </nav>
      <div className="relative w-full">
        <div className="absolute inset-0 mx-auto w-full" style={{ height: '2px', background: 'linear-gradient(to right, transparent 0%, white 50%, transparent 100%)', boxShadow: '0 0 10px white, 0 0 20px rgba(255, 255, 255, 0.5)' }}></div>
      </div>
    </>
  );
}
