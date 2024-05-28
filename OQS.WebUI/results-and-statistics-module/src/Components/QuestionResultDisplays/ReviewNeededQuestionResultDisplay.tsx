import React, { useState } from 'react';
import axios from 'axios';
import classNames from 'classnames';
import { Question, QuestionResult, QuizResults } from '../../utils/types/results-and-statistics/quiz-results';
import { AnswerResult, QuestionReview } from '../../utils/types/results-and-statistics/question-review';
import QuizResultsDisplay from '../QuizResultsDisplay';
import { QuestionType } from '../../utils/types/questions';

interface ReviewNeededQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

const ReviewNeededQuestionResultDisplay: React.FC<ReviewNeededQuestionResultDisplayProps> = ({ question, questionResult }) => {
  const [needReview, setNeedReview] = useState(false);
  const [questionReview, setReviewResults] = useState<QuestionReview | null>(null);
  const [showFullScreenResults, setShowFullScreenResults] = useState(false);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [loading, setLoading] = useState(false);

  const getQuizReview = async (userId: string, quizId: string, questionId: string, score: number) => {
    try {
      const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult?userId=${userId}&quizId=${quizId}&questionId=${questionId}&finalScore=${score}`);
      setReviewResults(response.data);
      setNeedReview(true);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  const getQuizResult = async (userId: string, quizId: string) => {
    setLoading(true);
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
      response.data.userId = userId;
      response.data.quizId = quizId;
      setQuizResults(response.data);
      setShowFullScreenResults(true);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="fixed inset-0 bg-white flex justify-center items-center">
        <p>Loading quiz results...</p>
      </div>
    );
  }

  if (showFullScreenResults && quizResults) {
    return (
      <div className="fixed inset-0 bg-teal-700 flex flex-col items-center overflow-auto p-6">
        <div className="w-full max-w-4xl bg-white shadow-lg rounded-lg p-6 border-4 border-green-700">
          <QuizResultsDisplay quizResults={quizResults} />
          <button 
            className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={() => setShowFullScreenResults(false)}
          >
            Back
          </button>
        </div>
      </div>
    );
  }

  if (needReview) {
    return (
      <div className="border-4 border-green-700 p-4">
        <button 
          className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
          onClick={() => getQuizResult(questionResult.userId, question.quizId)}
        >
          Grade
        </button>
        <p>LLMResponse: {questionReview?.updatedQuestionResult.LLMReview}</p>
      </div>
    );
  }

  return (
    <div className="space-y-4 border-4 border-green-700 p-4">
      <div className="flex items-center justify-center mb-2">
        <label
          className={classNames(
            'p-2 rounded-full w-full max-w-md text-left',
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
};

export default ReviewNeededQuestionResultDisplay;
