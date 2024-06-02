import { useEffect } from "react";
import { useCookies } from "react-cookie";
import { Outlet } from "react-router-dom";
import { config } from "../config";
import { userMock, userMock2 } from "../utils/mocks/userMock";
import GeneralSnackbar from "../Components/Snackbar";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { setUser } from "../redux/User/UserState";
import { User } from "../utils/types/user";
import Navbar from "../Components/Navbar";

export default function Root() {
  return (
    <>
      <Navbar />
      <GeneralSnackbar />

      <Outlet />
    </>
  );
}
