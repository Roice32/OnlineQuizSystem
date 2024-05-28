import { Outlet, RouteObject } from "react-router-dom";
import LoginPage from "../../Pages/LoginPage/LoginPage";
import RegisterPage from "../../Pages/RegisterPage/RegisterPage";
import ErrorPage from "../../Pages/ErrorPage";

export const AuthRoute: RouteObject = {
  path: "auth",
  element: <Outlet />,
  children: [
    {
      path: "login",
      element: <LoginPage />,
      errorElement: <ErrorPage />,
    },
    {
      path: "register",
      element: <RegisterPage />,
      errorElement: <ErrorPage />,
    },
  ],
};
