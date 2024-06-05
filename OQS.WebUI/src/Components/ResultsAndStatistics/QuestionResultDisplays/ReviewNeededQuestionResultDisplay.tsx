import React, { useState } from 'react';
import axios from 'axios';
import classNames from 'classnames';
import { QuestionResult } from "../../../utils/types/results-and-statistics/question-result";
import { Question } from "../../../utils/types/results-and-statistics/question";
import { AnswerResult } from '../../../utils/types/results-and-statistics/answer-result';
import { QuestionType } from '../../../utils/types/questions';
import { useSelector } from 'react-redux';
import { RootState } from '../../../redux/store';
import ErrorComponent from '../ErrorComponent';

interface ReviewNeededQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
  asQuizCreator: boolean;
}

const ReviewNeededQuestionResultDisplay: React.FC<ReviewNeededQuestionResultDisplayProps> = ({ question, questionResult, asQuizCreator}) => {
  const userState = useSelector((state: RootState) => state.user);
  const [needReview, setNeedReview] = useState(false);
  const [loading, setLoading] = useState(false);
  const [score, setScore] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [errorOccured, setErrorOccured] = useState('');

  const handleGrade = async (score: number) => {
    if (score < 0 || score > question.allocatedPoints) {
      setError(`Score must be between 0 and ${question.allocatedPoints}`);
      return;
    }
    setLoading(true);
    try {
      const token = userState.user?.token;
      await axios.put(`http://localhost:5276/api/quizResults/reviewResult?resultId=${questionResult.resultId}&questionId=${questionResult.questionId}&finalScore=${score}`, {},
        {
          headers: {
              'Authorization': `Bearer ${token}`
          }
        });
      setError(null);
      setNeedReview(false);
      window.location.reload();
    } catch (error) {
      console.error(error);
      setErrorOccured("An unexpected error occured.");
      if (axios.isAxiosError(error)) {
        if (error.response?.status === 404) {
          setErrorOccured("Could not find the requested answer.");
        } else if (error.response?.status === 401) {
          setErrorOccured("You do not have permission to grade that answer.");
        } else {
          setErrorOccured("Invalid JWT token provided.");
        }
      }
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
        <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
          <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Processing your review...</h1>
        </div>
      </div>
    );
  }

  if (errorOccured !== '') {
    return (
      <ErrorComponent err={errorOccured} />
    )
  }
  
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-center mb-2">
        <label
          className={classNames(
            'p-2 pl-4 rounded-full w-full max-w-md text-left',
            {
              'bg-green-500 text-white border-4 border-solid border-green-700': questionResult.reviewNeededResult === AnswerResult.Correct,
              'bg-red-500 text-white border-4 border-solid border-red-700': questionResult.reviewNeededResult === AnswerResult.Wrong || questionResult.reviewNeededResult === AnswerResult.NotAnswered,
              'bg-yellow-300 text-black border-4 border-solid border-yellow-500': questionResult.reviewNeededResult === AnswerResult.PartiallyCorrect,
              'bg-teal-500 text-white border-4 border-solid border-teal-700': questionResult.reviewNeededResult === AnswerResult.Pending,
            }
          )}
        >
          Answer: {questionResult.reviewNeededAnswer}
        </label>
      </div>
      {needReview && (
        <div className="space-y-4">
          <label
            className={classNames(
              'p-2 full max-w-md text-left',
              'bg-teal-400 text-white border-4 border-solid border-teal-500'
            )}
            style={{
              display: 'grid',
              width: '90%',
              minHeight: '100px',
              maxHeight: '200px', 
              overflowY: 'auto',
            }}
          >
            AI Review: {questionResult.LLMReview ?? "No AI Review Available"}
          </label>
          <input
            type="number"
            step="any"
            inputMode="numeric"
            value={score !== null ? score : ''}
            onChange={(e) => {
              const value = e.target.value;
              if (value === '') {
                setScore(null);
                setError(null);
              } else if (parseFloat(value) >= 0 && parseFloat(value) <= question.allocatedPoints) {
                setScore(parseFloat(value));
                setError(null);
              } else {
                setError(`Score must be between 0 and ${question.allocatedPoints}`);
              }
            }}
            onKeyDown={(e) => {
              if (e.key === 'Backspace') {
                setScore(null);
              }
            }}
            className="block w-24 mx-auto p-2 bg-gray-100 border-1 border-solid border-teal-500 rounded text-center"
            placeholder="Score"
            style={{ appearance: 'textfield', MozAppearance: 'textfield', WebkitAppearance: 'none' }}
          />
          {error && <p className="text-red-500 text-center">{error}</p>}
          <button
            className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={() => handleGrade(score || 0)}
          >
            Grade
          </button>
        </div>
      )}
      {!needReview && question.type === QuestionType.ReviewNeeded && 
        questionResult.reviewNeededResult === AnswerResult.Pending && 
        asQuizCreator && (
        <button
          className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
          onClick={() => setNeedReview(true)}
        >
          Review
        </button>
      )}
    </div>
  );
};

export default ReviewNeededQuestionResultDisplay;
