/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable prefer-const */
import { useDispatch, useSelector } from "react-redux";
import { activeQuizMock } from "../utils/mocks/activeQuizMock";
import { Quiz } from "../utils/types/quiz";
import {
  LoaderFunctionArgs,
  useLoaderData,
  useLocation,
  useNavigate,
} from "react-router-dom";
import { RootState } from "../redux/store";
import { ActiveQuiz, Answer } from "../utils/types/active-quiz";
import {
  ActiveQuizState,
  addActiveQuiz,
  nextQuestion,
  previousQuestion,
  goToFirstQuestion,
  goToLastQuestion,
  deleteActiveQuiz,
} from "../redux/ActiveQuiz/ActiveQuizzesState";
import QuestionDisplay from "../Components/Questions/QuestionDisplay";
import { useEffect, useState } from "react";
import axios from "../utils/axios-service";
import { Result } from "../utils/types/result";
import { NakedQuiz } from "../utils/types/naked-quiz";
import { useCookies } from "react-cookie";
import { openSnackbar } from "../redux/Snackbar/SnackbarState";
interface PageProps {
  params: { id: string };
}

export const ActiveQuizLoaderMock = async ({
  params: { id },
}: LoaderFunctionArgs<PageProps>): Promise<NakedQuiz> => {
  const response = activeQuizMock;
  if (response.isFailure) {
    throw response.error;
  }
  return response.value as NakedQuiz;
};

export const ActiveQuizLoader = async ({
  params: { id },
}: LoaderFunctionArgs<PageProps>): Promise<NakedQuiz> => {
  const response = (await axios.get(`/api/active-quizzes/${id}`)).data;
  if (response.isFailure) {
    throw response.error;
  }

  return response.value as NakedQuiz;
};

const formatTime = (seconds: number) => {
  const minutes = Math.floor(seconds / 60);
  const remainingSeconds = seconds % 60;
  return `${minutes}:${remainingSeconds < 10 ? "0" : ""}${remainingSeconds}`;
};

export default function ActiveQuizPage() {
  const quizData = useLoaderData() as NakedQuiz;

  const location = useLocation();
  const dispatch = useDispatch();
  const [cookies, setCookie, removeCookie] = useCookies();
  const [quizNotSet, setQuizNotSet] = useState(true);
  const activeQuizId: string = location.pathname.split("/")[2];
  const navigate = useNavigate();
  const [timeLeft, setTimeLeft] = useState(quizData.timeLimitMinutes * 60); //transform minutes to seconds
  const [questionTransition, setQuestionTransition] = useState(false);

  let activeQuizState = useSelector((state: RootState) =>
    state.activeQuizzes.find((state) => state.activeQuiz.id === activeQuizId)
  );

  useEffect(() => {
    if (quizNotSet && !activeQuizState) {
      const currentTime = new Date();
      currentTime.setMinutes(
        currentTime.getMinutes() + quizData.timeLimitMinutes
      );

      const activeQuiz: ActiveQuiz = {
        id: activeQuizId,
        deadline: currentTime.toISOString(),
        quiz: quizData,
        answers: {} as Map<string, Answer>,
      };

      dispatch(addActiveQuiz(activeQuiz));
      setQuizNotSet(false);
    }
  }, [quizNotSet, activeQuizState, quizData, activeQuizId, dispatch]);

  const handleSubmission = async (
    activeQuizId: string,
    nav: ReturnType<typeof useNavigate>
  ) => {
    try {
      const body = Object.values(answers);
      /*  console.log("Body", body); */
      const responses = body.map((answer) => {
        return { ...answer, type: answer.questionType };
      });
      /* console.log("Responses", responses); */
      const response = await axios.post(`/api/active-quizzes/${activeQuizId}`, {
        activeQuizId: activeQuizId,
        answers: responses,
      });
      console.log("Response", response);

      if (response.data.isFailure) {
        dispatch(
          openSnackbar({
            message: response.data.error.message,
            severity: "error",
          })
        );
      } else {
        // reducer for deleting active quiz
        dispatch(
          openSnackbar({ message: "Quiz submitted", severity: "success" })
        );
        /* console.log("Submission", JSON.stringify(Object.values(answers))); */
        // redirect to quiz page
        nav(`/quizzes`);
        dispatch(deleteActiveQuiz(activeQuizId));
      }
    } catch (error) {
      console.error("Error submitting active quiz:", error);
      dispatch(
        openSnackbar({
          message: "Error submitting quiz. Please try again.",
          severity: "error",
        })
      );
    }
  };

  useEffect(() => {
    const timer = setInterval(() => {
      setTimeLeft((prevTime) => {
        if (prevTime > 0) {
          return prevTime - 1;
        } else {
          // handle submission + navigate to quiz
          handleSubmission(activeQuizId, navigate);
          return 0;
        }
      });
    }, 1000);

    return () => clearInterval(timer);
  }, [activeQuizId, navigate]);

  useEffect(() => {
    if (activeQuizState) {
      const updateRemainingTime = () => {
        const now = new Date().valueOf();
        const remainingTime = Math.floor(
          (new Date(activeQuizState.activeQuiz.deadline).valueOf() - now) / 1000
        );
        setTimeLeft(remainingTime);
      };

      updateRemainingTime();
      const intervalId = setInterval(updateRemainingTime, 1000);

      return () => clearInterval(intervalId);
    }
  }, [activeQuizState]);

  if (!activeQuizState) {
    return <div>Loading...</div>;
  }

  const {
    activeQuiz: { id, quiz, answers, deadline },
    currentQuestion: currentQuestionIndex,
  } = activeQuizState as ActiveQuizState;

  const handleNextQuestion = () => {
    setQuestionTransition(true);
    setTimeout(() => {
      dispatch(nextQuestion({ activeQuizId: id }));
      setQuestionTransition(false);
    }, 300);
  };

  const handlePreviousQuestion = () => {
    setQuestionTransition(true);
    setTimeout(() => {
      dispatch(previousQuestion({ activeQuizId: id }));
      setQuestionTransition(false);
    }, 300);
  };

  const handleFirstQuestion = () => {
    setQuestionTransition(true);
    setTimeout(() => {
      dispatch(goToFirstQuestion({ activeQuizId: id }));
      setQuestionTransition(false);
    }, 300);
  };

  const handleLastQuestion = () => {
    setQuestionTransition(true);
    setTimeout(() => {
      dispatch(goToLastQuestion({ activeQuizId: id }));
      setQuestionTransition(false);
    }, 300);
  };

  const currentQuestion = quiz.questions[currentQuestionIndex];

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">
          {quiz.name}
        </h1>
        <div className="flex justify-between mb-4 items-center">
          <div className="relative flex items-center justify-center w-16 h-16">
            <svg className="absolute w-full h-full">
              <circle
                className="text-white"
                strokeWidth="4"
                stroke="currentColor"
                fill="transparent"
                r="28"
                cx="32"
                cy="32"
              />
              <circle
                className="text-[#1c4e4f]"
                strokeWidth="4"
                strokeDasharray={2 * Math.PI * 28}
                strokeDashoffset={
                  (1 - timeLeft / (quizData.timeLimitMinutes * 60)) *
                  2 *
                  Math.PI *
                  28
                }
                strokeLinecap="round"
                stroke="currentColor"
                fill="transparent"
                r="28"
                cx="32"
                cy="32"
              />
            </svg>
            <span className="absolute text-xl">{formatTime(timeLeft)}</span>
          </div>
          <p className="text-lg">Time: {quiz.timeLimitMinutes} minutes</p>
        </div>
        <p className="mb-4 text-center">
          Question: {currentQuestionIndex + 1}/{quiz.questions.length}
        </p>
        <div
          className={`transition-opacity duration-300 ${
            questionTransition ? "opacity-0" : "opacity-100"
          }`}
        >
          <QuestionDisplay
            question={currentQuestion}
            activeQuizId={id}
            key={currentQuestion.id}
            selectedAnswer={answers[currentQuestion.id]}
          />
        </div>
        <div className="flex justify-between mt-6">
          <button
            className="bg-[#6a8e8f] text-black px-4 py-2 rounded-full disabled:opacity-50 transform transition-transform duration-300 hover:bg-[#1c4e4f] font-mono"
            onClick={handleFirstQuestion}
            disabled={currentQuestionIndex === 0}
          >
            First Question
          </button>
          <button
            className="bg-[#6a8e8f] text-black px-4 py-2 rounded-full disabled:opacity-50 transform transition-transform duration-300 hover:bg-[#1c4e4f] font-mono"
            onClick={handlePreviousQuestion}
            disabled={currentQuestionIndex === 0}
          >
            Previous
          </button>
          <button
            className="bg-[#6a8e8f] text-black px-4 py-2 rounded-full disabled:opacity-50 transform transition-transform duration-300 hover:bg-[#1c4e4f] font-mono"
            onClick={handleNextQuestion}
            disabled={currentQuestionIndex === quiz.questions.length - 1}
          >
            Next
          </button>
          <button
            className="bg-[#6a8e8f] text-black px-4 py-2 rounded-full disabled:opacity-50 transform transition-transform duration-300 hover:bg-[#1c4e4f] font-mono"
            onClick={handleLastQuestion}
            disabled={currentQuestionIndex === quiz.questions.length - 1}
          >
            Last Question
          </button>
          {currentQuestionIndex === quiz.questions.length - 1 && (
            <button
              className="bg-[#6a8e8f] text-black px-4 py-2 rounded-full transform transition-transform duration-300 hover:bg-[#1c4e4f] font-mono"
              onClick={() => handleSubmission(activeQuizId, navigate)}
            >
              Submit
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
