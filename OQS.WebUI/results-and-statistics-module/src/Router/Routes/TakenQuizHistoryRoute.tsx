import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import TakenQuizzesHistory from "../../Pages/TakenQuizHistoryPage";

export const SubmittedQuizRoute: RouteObject = {
  path: "takenQuizzesHistory/:userId",
  element: <TakenQuizzesHistory />,
  errorElement: <ErrorPage />,
};