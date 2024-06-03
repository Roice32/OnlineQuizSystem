import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { RootState } from "../redux/store";
import { Button } from "@mui/material";
import { useCookies } from "react-cookie";
import { clearUser } from "../redux/User/UserState";
import axios from "../utils/axios-service";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

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
    <nav className="flex flex-row items-center bg-[#1c4e4f]">
      <ul className="flex flex-row justify-evenly items-center space-x-3 w-full text-stone-50">
        <li>
          <Link className="no-underline text-slate-50" to="/">
            Home
          </Link>
        </li>
        <li>
          <Link className="no-underline text-slate-50" to="/quizzes">
            Quiz
          </Link>
        </li>
        <li>
          <Link className="no-underline text-slate-50" to="/quizzes/play">
            Play Quizzes
          </Link>
        </li>

        {!userState.isLogged && (
          <>
            <li>
              <Link className="no-underline text-slate-50" to="/auth/login">
                Login
              </Link>
            </li>
            <li>
              <Link className="no-underline text-slate-50" to="/auth/register">
                Register
              </Link>
            </li>
          </>
        )}

        {userState.isLogged && (
          <>
            <li>
              <Link className="no-underline text-slate-50" to="/profile">
                {userState.user?.username}
              </Link>
            </li>
            <li>
              <Link className="no-underline text-slate-50" to="/quizzes/create">
                Create Quiz
              </Link>
            </li>

            <li>
              <Button variant="outlined" color="error" onClick={signOut}>
                Sign out
              </Button>
            </li>
          </>
        )}
      </ul>
    </nav>
  );
}
