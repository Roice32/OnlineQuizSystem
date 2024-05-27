import axios from "axios";
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

interface PageProps {
  params: { id: string };
}

export const QuizDetailsLoader = async ({
  params: { id },
}: LoaderFunctionArgs<PageProps>): Promise<Quiz> => {
  const response = (
    await axios.get(`http://localhost:5276/api/quizzes/${id}`, {})
  ).data as Quiz;

  return response;
};

type CreateActiveQuizRequest = {
  quizId: string;
  takenBy: string;
};

/* type CreateActiveQuizResponse = {
  id: string;
}; */

export default function QuizDetailsPage() {
  const quiz = useLoaderData() as Quiz;
  const navigate = useNavigate();

  const startQuiz = async () => {
    const body: CreateActiveQuizRequest = {
      quizId: quiz.id,
      takenBy: userMock.id,
    };
    const responseWrapped = await axios.post(
      `http://localhost:5276/api/active-quizzes`,
      body
    );

    const response = responseWrapped.data as Result<string>;
    if (response.isFailure) {
      alert(response.error?.message);
    } else {
      navigate(`/active-quiz/${response.value}`);
    }
  };

  const startQuizMock = () => {
    navigate(`/active-quiz/${activeQuizMock.value?.id}`);
  };

  const handleStart = () => {
    config.useBackend ? startQuiz() : startQuizMock();
  };
  return (
    <div>
      <h1>{quiz.name}</h1>
      {quiz.description && <p>{quiz.description}</p>}
      <p>{quiz.timeLimitMinutes} minutes</p>
      <button onClick={handleStart}>Start</button>
    </div>
  );
}
