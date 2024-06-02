import { useSelector } from "react-redux";
import { RootState } from "../redux/store";
import useAuth from "../hooks/UseAuth";
import ProfileNavbar from "../Components/ProfileNavbar";
import { useNavigate } from "react-router-dom";
function ProfilePage() {

    const user = useAuth();
    const userState = useSelector((state: RootState) => state.user);
    const navigate = useNavigate();

    const editProfile = () => {
      navigate("/profile/edit-profile")
    }

    const resetPassword = () => {
      navigate("/profile/reset-password")
    }

    return (
      <div className="flex flex-row bg-[#6a8e8f]">
        <div>
          <ProfileNavbar />
        </div>
        <div className="flex flex-col justify-center items-center w-full text-xl text-[#0a2d2e]">
          <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Account Details</h1>
          <div className="flex flex-col">
            <div className = "mx-28 px-20 min-h-[150px] bg-[#efd7cf] rounded-lg p-8 flex flex-row items-center flex-wrap relative">
              <div className="flex-grow">
              <div className="flex items-center w-full py-1.5">
                <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">First Name:</label>
                <div className="text-3xl">{userState.user?.firstName}</div> 
              </div>
              <div className="flex items-center w-full py-1.5">
                <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Last Name:</label>
                <div className="text-3xl">{userState.user?.lastName}</div> 
              </div>
              <div className="flex items-center w-full py-1.5">
                <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Username:</label>
                <div className="text-3xl">{userState.user?.username}</div> 
              </div>
              <div className="flex items-center w-full py-1.5">
                <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Email:</label>
                <div className="text-3xl">{userState.user?.email}</div> 
              </div>
              <div className="flex items-center w-full py-1.5">
                <label className="inline-block w-3/10 text-right mr-5 mb-2 text-[#436e6f] text-3xl">Password:</label>
                <div className="text-3xl" >********</div>
              </div>
              </div>
              
            </div>
            <div className = "flex justify-center mt-5 mr-7.5 p-5">
            <button className="m-2.5 bg-[#0a2d2e] text-[#efd7cf] px-10 py-2 rounded-lg font-bold hover:bg-[#697e7a]" onClick={editProfile}>Edit Profile</button>
            <button className="m-2.5 bg-[#0a2d2e] text-[#efd7cf] px-10 py-2 rounded-lg font-bold hover:bg-[#697e7a]" onClick ={resetPassword}>Reset Password</button>
            <button className="m-2.5 bg-[#0a2d2e] text-[#efd7cf] px-10 py-2 rounded-lg font-bold hover:bg-[#697e7a]" >Delete Account</button>
            </div>
          </div>
        </div>
      </div>
    );
}

export default ProfilePage;
