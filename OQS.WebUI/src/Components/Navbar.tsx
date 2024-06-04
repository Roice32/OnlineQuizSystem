import React, { useState } from 'react';
import { useDispatch, useSelector } from "react-redux";
import { Link } from "react-router-dom";
import { RootState } from "../redux/store";
import { Button } from "@mui/material";
import { useCookies } from "react-cookie";
import { clearUser } from "../redux/User/UserState";
import axios from "../utils/axios-service";
import { AiOutlineClose, AiOutlineMenu } from 'react-icons/ai';

export default function Navbar() {
  const [nav, setNav] = useState(false);
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
    }
  };

  const handleNav = () => {
    setNav(!nav);
  };

  const navItems = [
    { id: 1, text: 'Home', path: '/' },
    { id: 2, text: 'Quiz', path: '/quizzes' },
    { id: 3, text: 'Play Quizzes', path: '/quizzes/play' },
    ...(userState.isLogged ? [
      { id: 4, text: userState.user?.username, path: '/profile' },
      { id: 5, text: 'Create Quiz', path: '/quizzes/create' },
      { id: 6, text: 'Sign out', path: '#', action: signOut }
    ] : [
      { id: 4, text: 'Login', path: '/auth/login' },
      { id: 5, text: 'Register', path: '/auth/register' }
    ])
  ];

  return (
    <nav className="bg-[#0A2D2E] w-full flex justify-between items-center h-24 px-6 text-[#f7ebe7]">
      {/* Logo */}
      <h1 className='text-3xl font-bold text-[#DEAE9F]'>B4QUIZ</h1>

      {/* Desktop Navigation */}
      <ul className='hidden md:flex'>
        {navItems.map(item => (
          item.text !== 'Sign out' ? (
            <li key={item.id}>
              <Link className="p-4 hover:bg-[#DEAE9F] rounded-xl m-2 cursor-pointer duration-300 hover:text-black whitespace-nowrap" to={item.path}>
                {item.text}
              </Link>
            </li>
          ) : (
            <li key={item.id}>
              <Button className="p-4 hover:bg-[#DEAE9F] rounded-xl m-2 cursor-pointer duration-300 hover:text-red whitespace-nowrap" variant="outlined" color="error" onClick={item.action}>
                Sign out
              </Button>
            </li>
          )
        ))}
      </ul>

      {/* Mobile Navigation Icon */}
      <div onClick={handleNav} className='block md:hidden'>
        {nav ? <AiOutlineClose size={20} /> : <AiOutlineMenu size={20} />}
      </div>

      {/* Mobile Navigation Menu */}
      <ul className={
          nav
            ? 'fixed md:hidden left-0 top-0 w-[60%] h-full border-r border-r-gray-900 bg-[#0A2D2E] ease-in-out duration-500'
            : 'ease-in-out w-[60%] duration-500 fixed top-0 bottom-0 left-[-100%]'
        }
      >
        {/* Mobile Logo */}
        <h1 className='w-full text-3xl font-bold text-[#DEAE9F] m-4'>B4QUIZ</h1>

        {/* Mobile Navigation Items */}
        {navItems.map(item => (
          item.text !== 'Sign out' ? (
            <Link to={item.path} onClick={() => setNav(false)} key={item.id}>
              <li className='p-4 border-b rounded-xl hover:bg-[#DEAE9F] duration-300 hover:text-black cursor-pointer border-gray-600'>
                {item.text}
              </li>
            </Link>
          ) : (
            <li key={item.id} className='p-4 border-b rounded-xl hover:bg-[#DEAE9F] duration-300 hover:text-black cursor-pointer border-gray-600'>
              <Button className="w-full text-left" variant="outlined" color="error" onClick={() => { item.action?.(); setNav(false); }}>
                Sign out
              </Button>
            </li>
          )
        ))}
      </ul>
    </nav>
  );
}
