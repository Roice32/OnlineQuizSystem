import { RouteObject } from "react-router-dom";
import SubmittedQuiz from "../../Pages/SubmittedQuiz";
import ErrorPage from "../../Pages/ErrorPage";

export const SubmittedQuizRoute: RouteObject = {
  path: "submittedQuiz",
  element: <SubmittedQuiz />,
  errorElement: <ErrorPage />,
};