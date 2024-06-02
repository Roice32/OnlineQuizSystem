import { createBrowserRouter } from "react-router-dom";
import HomePage from "../Pages/HomePage";
import Root from "../Pages/Root";
import QuizStatsPage from "../Pages/CreatedQuizStatsPage";

import TakenQuizzesHistoryPage from "../Pages/TakenQuizHistoryPage";
import QuizResultsPage from "../Pages/QuizResultsPage";

import { QuizRoute } from "./Routes/QuizRoute";
import { ActiveQuizRoute } from "./Routes/ActiveQuizRoute";
import { LiveQuizRoute } from "./Routes/LiveQuizRoute";
import { AuthRoute } from "./Routes/AuthRoute";
import { ProfileRoute } from "./Routes/ProfileRoute";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: [
      {
        path: "/",
        element: <HomePage />,
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
      QuizRoute,
      ActiveQuizRoute,
      LiveQuizRoute,
      AuthRoute,
      ProfileRoute,
      
    ],
  },
]);

export default router;
