import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { RootState } from "../redux/store";
import { useCookies } from "react-cookie";
import { clearUser } from "../redux/User/UserState";
import axios from "../utils/axios-service";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import React from "react";

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
      dispatch(
        openSnackbar({ message: "Could not sign you out", severity: "error" })
      );
    }
  };
  return (
    <>
      <nav className="flex items-center bg-[#1c4e4f] h-12 p-2">
        <ul className="flex items-center space-x-4 text-stone-50 list-none ml-auto">
          <li className="relative group">
            <Link
              className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-[#deae9f]"
              to="/"
            >
              Home
            </Link>
            <span className="absolute right-0 top-1/2 transform -translate-y-1/2 h-6 w-px bg-white"></span>
          </li>
          <li className="relative group">
            <Link
              className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-[#deae9f]"
              to="/quizzes"
            >
              Quiz
            </Link>
            <span className="absolute right-0 top-1/2 transform -translate-y-1/2 h-6 w-px bg-white"></span>
          </li>

          {!userState.isLogged && (
            <>
              <li className="relative group">
                <Link
                  className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-[#deae9f]"
                  to="/auth/login"
                >
                  Login
                </Link>
                <span className="absolute right-0 top-1/2 transform -translate-y-1/2 h-6 w-px bg-white"></span>
              </li>
              <li className="relative group">
                <Link
                  className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-[#deae9f]"
                  to="/auth/register"
                >
                  Register
                </Link>
                <span className="absolute right-0 top-1/2 transform -translate-y-1/2 h-6 w-px bg-white"></span>
              </li>
            </>
          )}

          {userState.isLogged && (
            <>
              <li className="relative group">
              <Link
                className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-[#deae9f]"
                to="/profile"
              >
                {userState.user?.username}
                </Link>
                <span className="absolute right-0 top-1/2 transform -translate-y-1/2 h-6 w-px bg-white"></span>
              </li>
              <li className="relative group">
                <button
                  onClick={signOut}
                  className="no-underline text-slate-50 px-2 py-1 text-lg transition duration-300 hover:text-red-600 bg-transparent border-none cursor-pointer"
                >
                  Sign out
                </button>
              </li>
            </>
          )}
        </ul>
      </nav>
      <div className="relative">
        <div className="absolute left-0 right-0 h-1 bg-gradient-to-r from-white via-transparent to-white mx-4"></div>
      </div>
    </>
  );
}
