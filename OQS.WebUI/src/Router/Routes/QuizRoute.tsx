import {Outlet, RouteObject} from "react-router-dom";
import QuizPage, {QuizzesLoader} from "../../Pages/QuizPage";
import QuizDetailsPage, {
    QuizDetailsLoader,
} from "../../Pages/QuizDetailsPage";
import QuizzesPage from "../../Pages/QuizzesPage";
import ErrorPage from "../../Pages/ErrorPage";
import QuizCreate from "../../Pages/QuizCreate/QuizCreate.tsx";
import EditQuizPage from "../../Pages/QuizEdit/EditQuizPage.tsx";

export const QuizRoute: RouteObject = {
  path: "quizzes",
  element: <Outlet />,
  children: [
    {
      path: "",
      loader: QuizzesLoader,
      errorElement: <ErrorPage />,
      element: <QuizzesPage />,
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
    {
      path: "update/:id",
      errorElement: <ErrorPage/>,
      element: <EditQuizPage/>,
    },
    {
      path: "play",
      errorElement: <ErrorPage />,
      element: <QuizzesPage />,
    },
  ],
};
