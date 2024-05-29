import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import QuizStatsPage from "../../Pages/CreatedQuizStatsPage";
import UpdatedQuizResultsPage from "../../Pages/UpdatedQuizResultsPage";


export const UpdatedQuizResultsRoute: RouteObject = {
  path: "updatedQuizResults/:userId/:quizId",
  element: <UpdatedQuizResultsPage />,
  errorElement: <ErrorPage />,
};