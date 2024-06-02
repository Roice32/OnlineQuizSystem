import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import TakenQuizzesHistoryPage from "../../Pages/ResultsAndStatistics/TakenQuizzesHistoryPage";

export const TakenQuizzesHistoryRoute: RouteObject = {
  path: "taken-quizzes-history/:userId",
  element: <TakenQuizzesHistoryPage />,
  errorElement: <ErrorPage />,
};