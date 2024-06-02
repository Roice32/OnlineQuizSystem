import { RouteObject } from "react-router-dom";
import ErrorPage from "../../Pages/ErrorPage";
import LiveQuizPage from "../../Pages/LiveQuizPage";

export const LiveQuizRoute: RouteObject = {
  path: "live-quiz/:id",
  element: <LiveQuizPage />,
  errorElement: <ErrorPage />,
};
