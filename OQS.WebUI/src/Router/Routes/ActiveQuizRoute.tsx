import { RouteObject } from "react-router-dom";
import ActiveQuizPage, {
  ActiveQuizLoader,
  ActiveQuizLoaderMock,
} from "../../Pages/ActiveQuizPage";
import ErrorPage from "../../Pages/ErrorPage";
import { config } from "../../config";

const loader = config.useBackend ? ActiveQuizLoader : ActiveQuizLoaderMock;

export const ActiveQuizRoute: RouteObject = {
  path: "active-quiz/:id",
  loader,
  element: <ActiveQuizPage />,
  errorElement: <ErrorPage />,
};
