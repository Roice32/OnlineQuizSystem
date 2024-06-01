import { Outlet, RouteObject } from "react-router-dom";
import ProfilePage from "../../Pages/ProfilePage";
import ErrorPage from "../../Pages/ErrorPage";
import EditProfile from "../../Pages/EditProfile";
import ResetPassword from "../../Pages/UpdatePasswordPage";



export const ProfileRoute: RouteObject = {
    path: "profile",
    element: <Outlet />,
    children: [
      {
        path: "",
        element: <ProfilePage />,
        errorElement: <ErrorPage />,
      },
      {
        path: "edit-profile",
        element: <EditProfile />,
        errorElement: <ErrorPage />,
      },
      {
        path: "reset-password",
        element: <ResetPassword />,
        errorElement: <ErrorPage />,
      }
    ],
  };