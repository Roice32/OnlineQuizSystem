import { createBrowserRouter } from "react-router-dom";
import { QuizRoute } from "./Routes/QuizRoute";
import HomePage from "../Pages/HomePage";
import Root from "../Pages/Root";
import { ActiveQuizRoute } from "./Routes/ActiveQuizRoute";
import { SubmittedQuizRoute } from "./Routes/SubmittedQuizRoute";

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
      SubmittedQuizRoute,
    ],
  },
]);
