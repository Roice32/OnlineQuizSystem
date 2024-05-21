import { createBrowserRouter } from "react-router-dom";
import { QuizRoute } from "./Routes/QuizRoute";
import SubmittedQuiz from "../Pages/SubmittedQuiz";
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
        element: <SubmittedQuiz />,
      },
      QuizRoute,
      ActiveQuizRoute,
      SubmittedQuizRoute,
    ],
  },
]);
