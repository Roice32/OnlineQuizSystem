import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import QuizResultsPage from "../../Pages/QuizResultsPage";


export const QuizResultsRoute: RouteObject = {
  path: "quizResults/:userId/:quizId",
  element: <QuizResultsPage />,
  errorElement: <ErrorPage />,
};