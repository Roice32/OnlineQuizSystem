import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import QuizResultPage from "../../Pages/ResultsAndStatistics/QuizResultPage";


export const QuizResultRoute: RouteObject = {
  path: "quiz-result/:userId/:quizId",
  element: <QuizResultPage />,
  errorElement: <ErrorPage />,
};