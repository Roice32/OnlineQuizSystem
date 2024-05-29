/* eslint-disable @typescript-eslint/no-unused-vars */

import { Link, useLoaderData, useNavigate } from "react-router-dom";
import { Quiz } from "../utils/types/quiz";
import { Result } from "../utils/types/result";
import axios from "axios";
import { GetAllQuizzesResponse } from "../utils/types/get-all-quizzes-response";
import { useSelector } from "react-redux";
import { RootState } from "../redux/store";

export const QuizzesLoader = async (): Promise<Quiz[]> => {
  const response = (
    await axios.get("http://localhost:5276/api/quizzes?offset=0&limit=10", {})
  ).data as GetAllQuizzesResponse;
  console.log(response);
  return response.quizzes as Quiz[];
};

export default function QuizPage() {
  const quizzes = useLoaderData() as Quiz[];
  const activeQuizzes = useSelector((state: RootState) => state.activeQuizzes);
  console.log("Active Quizzes:", activeQuizzes.length);
  /* console.log(quizzes); */

  return (
    <div>
      <h1>Quiz</h1>
      <ul>
        {quizzes.map((quiz) => (
          <li key={quiz.id}>
            <Link to={`/quiz/${quiz.id}`} key={quiz.id}>
              {quiz.name}
            </Link>
          </li>
        ))}
      </ul>
    </div>
  );
}
