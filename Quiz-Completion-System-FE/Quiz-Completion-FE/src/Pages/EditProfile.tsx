import ProfileNavbar from "../Components/ProfileNavbar";
import { useState, useEffect } from 'react';
import { RootState } from "../redux/store";
import useAuth from "../hooks/UseAuth";
import { useNavigate } from "react-router-dom";
import { useSelector } from "react-redux";
import axios from "../utils/axios-service";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

function EditProfile() {
    const user = useAuth();
    const userState = useSelector((state: RootState) => state.user);
    const navigate = useNavigate();
    const dispatch = useDispatch();

    const [firstName, setFirstName] = useState('');
    const [lastName, setLastName] = useState('');
    const [username, setUsername] = useState('');
    const [email, setEmail] = useState('');

    useEffect(() => {
        setFirstName(userState.user?.firstName || '');
        setLastName(userState.user?.lastName || '');
        setUsername(userState.user?.username || '');
        setEmail(userState.user?.email || '');
    }, [userState.user])

    const editProfile = async (e) => {
      e.preventDefault();
      const token = userState.user?.token;
      try{
        const response = await axios.put(`/api/profile/${userState.user?.id}/edit_profile`, {
            firstName,
            lastName,
            username,
            email,
        }, 
        {
          headers: {
              'Authorization': `Bearer ${token}`
          }
        });

        if (response.data.message === "Details reset successfully!") {
          dispatch(
            openSnackbar({
              message: "Account updated successfully!",
              severity: "success",
            })
          );
          navigate("/profile");
        }
        else
        {
          dispatch(
            openSnackbar({
              message: "Could not update account",
              severity: "error",
            })
          );
        }
      }
      catch(error){
        console.error(error);
      }
    }


    return (
        <div className="flex flex-row bg-[#6a8e8f]">
          <div>
            <ProfileNavbar />
          </div>
          <div className="flex flex-col justify-center items-center w-full text-xl text-[#0a2d2e]">
          <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Edit Profile</h1>
            <div className=" mx-28 px-20 min-h-[150px] bg-[#efd7cf] rounded-lg p-8 flex flex-row items-center flex-wrap relative">
              <div className="flex-grow">
                <div className="flex items-center w-full py-1.5">
                  <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">First Name</label>
                  <input className="w-7/10 p-2.5 mb-2.5 border border-[#6a8e8f] rounded-md" type="text" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
                </div>
                <div className="flex items-center w-full py-1.5">
                  <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Last Name</label>
                  <input className="w-7/10 p-2.5 mb-2.5 border border-[#6a8e8f] rounded-md" type="text" value={lastName} onChange={(e) => setLastName(e.target.value)} />
                </div>
                <div className="flex items-center w-full py-1.5">
                  <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Username</label>
                  <input className="w-7/10 p-2.5 mb-2.5 border border-[#6a8e8f] rounded-md" type="text" value={username} onChange={(e) => setUsername(e.target.value)} />
                </div>
                <div className="flex items-center w-full py-1.5">
                  <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Email</label>
                  <input className="w-7/10 p-2.5 mb-2.5 border border-[#6a8e8f] rounded-md" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                </div>
              </div>
            </div>
            <button className="m-2.5 mt-5 bg-[#0a2d2e] text-[#efd7cf] px-10 py-2 rounded-lg font-bold hover:bg-[#697e7a]" onClick ={editProfile}>Save edit</button>
          </div>
        </div>
      );
}

export default EditProfile