import { Outlet, RouteObject } from "react-router-dom";
import LoginPage from "../../Pages/LoginPage";
import RegisterPage from "../../Pages/RegisterPage";
import ErrorPage from "../../Pages/ErrorPage";
import ForgotPasswordPagse from "../../Pages/ForgotPasswordPage";

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
    {
      path: "forgot-password",
      element: <ForgotPasswordPagse />,
      errorElement: <ErrorPage />,
    }
  ],
};
