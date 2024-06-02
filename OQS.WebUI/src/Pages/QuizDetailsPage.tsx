import axios from "../utils/axios-service";
import { Quiz } from "../utils/types/quiz";
import { Result } from "../utils/types/result";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useNavigate,
} from "react-router-dom";
import { userMock } from "../utils/mocks/userMock";
import { activeQuizMock } from "../utils/mocks/activeQuizMock";
import { config } from "../config";
import { useDispatch, useSelector } from "react-redux";
import { RootState } from "../redux/store";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
import useAuth from "../hooks/UseAuth";

interface PageProps {
  params: { id: string };
}

export const QuizDetailsLoader = async ({
  params: { id },
}: LoaderFunctionArgs<PageProps>): Promise<Quiz> => {
  const response = (await axios.get(`/api/quizzes/${id}`, {}))
    .data as Result<Quiz>;
  if (response.isFailure) {
    throw new Error(response.error?.message);
  }
  return response.value as Quiz;
};

type CreateActiveQuizRequest = {
  quizId: string;
};

type CreateLiveQuizRequest = {
  quizId: string;
  userId: string;
};

export default function QuizDetailsPage() {
  const quiz = useLoaderData() as Quiz;
  console.log(quiz);
  const user = useAuth();
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const userState = useSelector((state: RootState) => state.user);
  const startQuiz = async () => {
    const body: CreateActiveQuizRequest = {
      quizId: quiz.id,
    };
    const responseWrapped = await axios.post(`/api/active-quizzes`, body);

    const response = responseWrapped.data as Result<string>;
    if (response.isFailure) {
      dispatch(
        openSnackbar({
          message: response.error?.message as string,
          severity: "error",
        })
      );
    } else {
      dispatch(
        openSnackbar({
          message: `The quiz has started!`,
          severity: "success",
        })
      );
      navigate(`/active-quiz/${response.value}`);
    }
  };

  const startLiveQuiz = async () => {
    const body: CreateLiveQuizRequest = {
      quizId: quiz.id,
      userId: userState.user?.id as string,
    };
    console.log(body);
    const responseWrapped = await axios.post(`/api/live-quizzes`, body);

    const response = responseWrapped.data as Result<string>;
    if (response.isFailure) {
      dispatch(
        openSnackbar({
          message: response.error?.message as string,
          severity: "error",
        })
      );
    } else {
      dispatch(
        openSnackbar({
          message: `Live quiz created! Your code is ${response.value}`,
          severity: "success",
        })
      );
      navigate(`/live-quiz/${response.value}`);
    }
  };

  const startQuizMock = () => {
    navigate(`/active-quiz/${activeQuizMock.value?.id}`);
  };

  const startLiveQuizMock = () => {
    navigate(`/live-quizzes/${activeQuizMock.value?.id}`);
  };

  const handleStart = () => {
    config.useBackend ? startQuiz() : startQuizMock();
  };

  const handleStartLive = () => {
    config.useBackend ? startLiveQuiz() : startLiveQuizMock();
  };

  return (
    <div>
      <h1>{quiz.name}</h1>
      {quiz.description && <p>{quiz.description}</p>}
      <p>{quiz.timeLimitMinutes} minutes</p>
      <button onClick={handleStart}>Start Quiz</button>
      <button onClick={handleStartLive}>Start Live Quiz</button>
    </div>
  );
}
