import { createBrowserRouter } from "react-router-dom";
import HomePage from "../Pages/HomePage";
import Root from "../Pages/Root";
import { QuizRoute } from "./Routes/QuizRoute";
import { ActiveQuizRoute } from "./Routes/ActiveQuizRoute";
import { LiveQuizRoute } from "./Routes/LiveQuizRoute";
import { AuthRoute } from "./Routes/AuthRoute";
import { ProfileRoute } from "./Routes/ProfileRoute";
import { TakenQuizzesHistoryRoute } from "./Routes/TakenQuizzesHistoryRoute";
import { CreatedQuizStatsRoute } from "./Routes/CreatedQuizStatsRoute";
import { QuizResultRoute } from "./Routes/QuizResultRoute";
import ErrorPage from "../Pages/ErrorPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <Root />,
    children: [
      {
        path: "*",
        element: <ErrorPage />,
      },
      {
        path: "/",
        element: <HomePage />,
      },
      TakenQuizzesHistoryRoute,
      CreatedQuizStatsRoute,
      QuizResultRoute,
      QuizRoute,
      ActiveQuizRoute,
      LiveQuizRoute,
      AuthRoute,
      ProfileRoute,
    ],
  },
]);

export default router;
