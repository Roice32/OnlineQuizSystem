import { createBrowserRouter } from "react-router-dom";
import QuizStatsPage from "../Pages/CreatedQuizStatsPage";
import Root from "../Pages/Root";
import { QuizRoute } from "./Routes/QuizRoute";
import SubmittedQuiz from "../Pages/SubmittedQuiz";
import { ActiveQuizRoute } from "./Routes/ActiveQuizRoute";
import { SubmittedQuizRoute } from "./Routes/SubmittedQuizRoute";
import TakenQuizzesHistoryPage from "../Pages/TakenQuizHistoryPage";
import QuizResultsPage from "../Pages/QuizResultsPage";
import UpdatedQuizResultsPage from "../Pages/UpdatedQuizResultsPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: [
      {
        path: "/",
        element: <SubmittedQuiz />,
      },
      {
        path: "quizStats/:quizId",
        element: <QuizStatsPage />,
      },
      {
        path: "takenQuizzesHistory/:userId",
        element: <TakenQuizzesHistoryPage />,
      },
      {
        path: "quizResults/:userId/:quizId",
        element: <QuizResultsPage />,
      },
      {
        path: "updatedQuizResults/:userId/:quizId",
        element: <UpdatedQuizResultsPage />,
      },
      QuizRoute,
      ActiveQuizRoute,
      SubmittedQuizRoute,
    ],
  },
]);