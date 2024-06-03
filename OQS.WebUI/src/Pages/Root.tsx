import { Outlet } from "react-router-dom";
import GeneralSnackbar from "../Components/Snackbar";
import Navbar from "../Components/Navbar";
import { Theme } from "@radix-ui/themes";
import { useEffect } from "react";
import { RootState } from "../redux/store";
import { useDispatch, useSelector } from "react-redux";
import { clearUser } from "../redux/User/UserState";
import { useCookies } from "react-cookie";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";

export default function Root() {
  const userState = useSelector((state: RootState) => state.user);
  const dispatch = useDispatch();
  const [cookies, setCookie, removeCookie] = useCookies();
  useEffect(() => {
    if (
      userState.authDeadline &&
      new Date() > new Date(userState.authDeadline)
    ) {
      dispatch(openSnackbar({ message: "Session expired", severity: "info" }));
      dispatch(clearUser());
      removeCookie("token");
    }
  }, []);
  return (
    <>
      <Theme accentColor="teal">
        <Navbar />
        <GeneralSnackbar />

        <Outlet />
      </Theme>
    </>
  );
}
