import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import CreatedQuizStatsPage from "../../Pages/ResultsAndStatistics/CreatedQuizStatsPage";

export const CreatedQuizStatsRoute: RouteObject = {
  path: "created-quiz-stats/:quizId",
  element: <CreatedQuizStatsPage />,
  errorElement: <ErrorPage />,
};