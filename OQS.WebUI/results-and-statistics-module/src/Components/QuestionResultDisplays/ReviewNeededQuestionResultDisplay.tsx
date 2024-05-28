import React, { useState } from 'react';
import { QuestionType } from "../../utils/types/questions";
import { Question, QuestionResult, QuizResults } from '../../utils/types/results-and-statistics/quiz-results';
import { AnswerResult, QuestionReview } from '../../utils/types/results-and-statistics/question-review';
import axios from 'axios';
import classNames from 'classnames';
import QuizResultsDisplay from '../QuizResultsDisplay';

interface ReviewNeededQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function ReviewNeededQuestionResultDisplay({ question, questionResult }: ReviewNeededQuestionResultDisplayProps) {
  const [needReview, setNeedReview] = useState(false);
  const [questionReview, setReviewResults] = useState<QuestionReview>();
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);

  const getQuizReview = async (userId: string, quizId: string, questionId: string, score: number) => {
    try {
      console.log(`Fetching review for user ID: ${userId} quizId: ${quizId} questionId: ${questionId} score: ${score}`);
      const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult?userId=${userId}&quizId=${quizId}&questionId=${questionId}&finalScore=${score}`);
      console.log(response.data);
      setReviewResults(response.data);
      setNeedReview(true);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  const getQuizResult = async (userId: string, quizId: string) => {
    setShowMessage3(true);
    setShowMessage1(false);
    setShowMessage2(false);
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
      console.log(response.data);
      response.data.userId = userId;
      response.data.quizId = quizId;
      setQuizResults(response.data);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  if (showMessage3) {
    return (
      <div>
        <div>
          {quizResults && <QuizResultsDisplay quizResults={quizResults} />}
        </div>
      </div>
    );
  } else {
    if (needReview === true) {
      return (
        <p>
          <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={() => getQuizResult(questionResult.userId, question.quizId)}>
            Grade
          </button>
          <p>LLMResponse: {questionReview?.updatedQuestionResult.LLMReview}</p>
        </p>
      );
    }
    return (
      <div className="space-y-4">
        <div className="flex items-center justify-center mb-2">
          <label
            className={classNames(
              'p-2 rounded-full rounded-full w-full max-w-md text-left',
              {
                'bg-green-500 text-white border-4 border-solid border-green-700': questionResult.reviewNeededResult === AnswerResult.Correct,
                'bg-red-500 text-white border-4 border-solid border-red-700': questionResult.reviewNeededResult === AnswerResult.Wrong || questionResult.reviewNeededResult === AnswerResult.NotAnswered,
                'bg-yellow-300 text-black border-4 border-solid border-yellow-500': questionResult.reviewNeededResult === AnswerResult.PartiallyCorrect,
                'bg-purple-500 text-white border-4 border-solid border-purple-700': questionResult.reviewNeededResult === AnswerResult.Pending,
              }
            )}
          >
            Your Answer: {questionResult.reviewNeededAnswer}
          </label>
        </div>
        {question.type === QuestionType.ReviewNeeded && questionResult.reviewNeededResult === AnswerResult.Pending &&
          <button
            className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={() => getQuizReview(questionResult.userId, question.quizId, questionResult.questionId, questionResult.score)}
          >
            Review
          </button>
        }
      </div>
    );
  }
}
