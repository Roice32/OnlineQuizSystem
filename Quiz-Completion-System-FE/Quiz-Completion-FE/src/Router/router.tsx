import { createBrowserRouter } from "react-router-dom";
import { QuizRoute } from "./Routes/QuizRoute";
import HomePage from "../Pages/HomePage";
import Root from "../Pages/Root";
import { ActiveQuizRoute } from "./Routes/ActiveQuizRoute";
import { LiveQuizRoute } from "./Routes/LiveQuizRoute";
import { AuthRoute } from "./Routes/AuthRoute";
import { ProfileRoute } from "./Routes/ProfileRoute";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: [
      {
        path: "/",
        element: <HomePage />,
      },
      QuizRoute,
      ActiveQuizRoute,
      LiveQuizRoute,
      AuthRoute,
      ProfileRoute,
    ],
  },
]);
