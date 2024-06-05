import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { Link } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faSignOutAlt } from '@fortawesome/free-solid-svg-icons';
import axios from "../utils/axios-service";
import { useCookies } from "react-cookie";
import { clearUser } from "../redux/User/UserState";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

export default function ProfileNavbar() {
    const userState = useSelector((state: RootState) => state.user);
    const dispatch = useDispatch();
    const role =  userState.user?.role as string;
    const [cookies, setCookie, removeCookie] = useCookies();

  
    const signOut = async () => {
      try {
        const response = (
          
          await axios.post(`/api/profile/${userState.user?.id}/logout`, {
              headers: { Authorization: userState.user?.token },
            })
        ).data;
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
        <div className="bg-[#0a2d2e] w-72 min-h-screen top-0 left-0 flex flex-col rounded-tr-lg rounded-br-lg list-none">
            <div >
          <div className="p-2 mb-20 text-center">
            <Link className="no-underline text-[#efd7cf] text-3xl" to={`/profile`}>Profile</Link>
          </div>
          <div className="menu">
            {role === 'User' && (
              <>
                <li className="p-2 cursor-pointer flex hover:bg-[#555] hover:rounded-tr-lg hover:rounded-br-lg">
                  <Link className="no-underline text-[#efd7cf] text-2xl" to={`/taken-quizzes-history/${userState.user?.id}`}>My Taken Quizzes</Link>
                </li>
                <li className="p-2 cursor-pointer flex hover:bg-[#555] hover:rounded-tr-lg hover:rounded-br-lg">
                  <Link className="no-underline text-[#efd7cf] text-2xl" to="/contact">Contact</Link>
                </li>
              </>
            )}
          {role === 'Admin' && (
                        <>
                            <li className="p-2 cursor-pointer flex hover:bg-[#555] hover:rounded-tr-lg hover:rounded-br-lg">
                                <Link className="no-underline text-[#efd7cf] text-2xl" to="/profile/add-admin">Add Admin</Link>
                            </li>
                            <li className="p-2 cursor-pointer flex hover:bg-[#555] hover:rounded-tr-lg hover:rounded-br-lg">
                                 <Link className="no-underline text-[#efd7cf] text-2xl" to="/profile/view-users">View Users</Link>
                            </li>
                        </>
                    )}
            <li className="p-2 cursor-pointer flex hover:bg-[#555] hover:rounded-tr-lg hover:rounded-br-lg">
              <button className="text-[#efd7cf] text-2xl" onClick={signOut}>
                <FontAwesomeIcon icon={faSignOutAlt} /> Logout
              </button>
            </li>
          </div>
        </div>
        </div>
      );
  
}