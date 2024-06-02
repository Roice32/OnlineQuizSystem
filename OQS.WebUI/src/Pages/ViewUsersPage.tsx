import React, { useState, useEffect } from 'react';
import { useSelector } from "react-redux";
import { RootState } from "../redux/store";
import useAuth from "../hooks/UseAuth";
import ProfileNavbar from "../Components/ProfileNavbar";
import { useNavigate } from "react-router-dom";
import axios from "../utils/axios-service";
import { useDispatch } from "react-redux";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";


function ViewUsers(){
    const user = useAuth();
    const userState = useSelector((state: RootState) => state.user);
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const [users, setUsers] = useState<any[]>([]);

    useEffect(() => {
        const getUsers = async () =>{
            try{
                const token = userState.user?.token;
                const response = await axios.get('/api/getUsers', {
                    headers: {
                        'Authorization': `Bearer ${token}`,
                    }
                });

                if (Array.isArray(response.data)) {
                    setUsers(response.data);
                } else {
                    console.error('Expected an array but got:', response.data);
                }
            }
            catch(error){
                console.error(error);
            }
        };

        getUsers();
    }, [])

    const handleDelete = async (username: string) => {
        try{
            const token = userState.user?.token;
            const response = await axios.delete(`api/profile/delete_user`, {
                headers: {
                    'Authorization': `Bearer ${token}`,
                }, 
                params: { username }
            });

            if(response.data.message === "User deleted successfully!"){
                dispatch(
                    openSnackbar({
                      message: "User deleted successfully!",
                      severity: "success",
                    })
                  );
            }
            else
            {
                dispatch(
                    openSnackbar({
                      message: response.data.message,
                      severity: "error",
                    })
                  );
            }

            setUsers(users.filter(user => user.userName !== username));
        }
        catch(error){
            console.error(error);
        }
    }

    return (
        <div className="flex flex-row bg-[#6a8e8f]">
            <div className="navbar">
                <ProfileNavbar />
            </div>
            <div className="flex flex-col items-center justify-center w-full py-7.5"> 
                <h1 className="text-center text-[#1c4e4f] text-5xl pb-10 pt-5 font-bold">Users List</h1>
                <div className="bg-[#efd7cf] p-7.5 w-7/10 rounded-lg">
                <table className="user-table w-full">
  <thead>
    <tr className="bg-[#f2f2f2] font-bold">
      <th className="border border-[#ddd] px-10 py-2  text-left">ID</th>
      <th className="border border-[#ddd] px-10 py-2 text-left">First Name</th>
      <th className="border border-[#ddd] px-10 py-2 text-left">Last Name</th>
      <th className="border border-[#ddd] px-10 py-2 text-left">Username</th>
      <th className="border border-[#ddd] px-10 py-2 text-left">Email</th>
      <th className="border border-[#ddd] px-10 py-2 text-left">Actions</th>
    </tr>
  </thead>
  <tbody>
    {users.map((user, index) => (
      <tr key={user.id} className={`h-12 border border-[#ddd] px-6 py-2 text-center ${index % 2 === 0 ? 'bg-[#f9f9f9]' : ''} hover:bg-[#879693]`}>
        <td>{user.id}</td>
        <td>{user.firstName}</td>
        <td>{user.lastName}</td>
        <td>{user.userName}</td>
        <td>{user.email}</td>
        <td>
          <button
            className="delete-button bg-red-500 text-white border-0 px-3 py-1 cursor-pointer rounded-md hover:bg-red-700"
            onClick={() => handleDelete(user.userName)}
          >
            X
          </button>
        </td>
      </tr>
    ))}
  </tbody>
</table>
                </div>
            </div>
        </div>
    );
}

export default ViewUsers;