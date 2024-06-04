import { Outlet, RouteObject } from "react-router-dom";
import ProfilePage from "../../Pages/ProfilePage";
import ErrorPage from "../../Pages/ErrorPage";
import EditProfile from "../../Pages/EditProfile";
import ResetPassword from "../../Pages/UpdatePasswordPage";
import AddAdmin from "../../Pages/AddAdminPage";
import ViewUsers from "../../Pages/ViewUsersPage";
import MyQuizzesPage from "../../Pages/MyQuizzesPage";


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
      },
      {
        path: "add-admin",
        element: <AddAdmin />,
        errorElement: <ErrorPage />,
      },
      {
        path: "view-my-quizzes/:userId",
        element: <MyQuizzesPage />,
        errorElement: <ErrorPage />,
      },
      {
        path: "view-users",
        element: <ViewUsers />,
        errorElement: <ErrorPage />,
      },
    ],
  };