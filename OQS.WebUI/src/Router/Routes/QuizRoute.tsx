import { Outlet, RouteObject } from "react-router-dom";
import QuizPage, { QuizzesLoader } from "../../Pages/QuizPage";
import QuizDetailsPage, {
  QuizDetailsLoader,
} from "../../Pages/QuizDetailsPage";
import ErrorPage from "../../Pages/ErrorPage";
import QuizCreate from "../../Pages/QuizCreate/QuizCreate.tsx";

export const QuizRoute: RouteObject = {
  path: "quizzes",
  element: <Outlet />,
  children: [
    {
      path: "",
      loader: QuizzesLoader,
      errorElement: <ErrorPage />,
      element: <QuizPage />,
    },
    {
      path: ":id",
      loader: QuizDetailsLoader,
      errorElement: <ErrorPage />,
      element: <QuizDetailsPage />,
    },
    {
      path: "create",
      errorElement: <ErrorPage />,
      element: <QuizCreate />,
    },
  ],
};
