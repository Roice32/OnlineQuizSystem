import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import QuizStatsPage from "../../Pages/CreatedQuizStatsPage";


export const QuizResultsRoute: RouteObject = {
  path: "quizResults/:userId/:quizId",
  element: <QuizStatsPage />,
  errorElement: <ErrorPage />,
};