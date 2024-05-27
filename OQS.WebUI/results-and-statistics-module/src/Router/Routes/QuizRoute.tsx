import { Outlet, RouteObject } from "react-router-dom";
import QuizPage, { QuizzesLoader } from "../../Pages/QuizPage";
import QuizDetailsPage, {
  QuizDetailsLoader,
} from "../../Pages/QuizDetailsPage";
import ErrorPage from "../../Pages/ErrorPage";

export const QuizRoute: RouteObject = {
  path: "quiz",
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
  ],
};
