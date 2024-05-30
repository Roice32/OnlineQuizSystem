import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import QuizStatsPage from "../../Pages/CreatedQuizStatsPage";

export const QuizStatsRoute: RouteObject = {
  path: "quizStats/:quizId",
  element: <QuizStatsPage />,
  errorElement: <ErrorPage />,
};