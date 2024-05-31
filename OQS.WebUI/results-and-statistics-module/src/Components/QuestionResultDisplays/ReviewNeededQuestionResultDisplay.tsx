import React, { useState } from 'react';
import axios from 'axios';
import classNames from 'classnames';
import { Question, QuestionResult, QuizResults } from '../../utils/types/results-and-statistics/quiz-results';
import { AnswerResult, QuestionReview } from '../../utils/types/results-and-statistics/question-review';
import { QuestionType } from '../../utils/types/questions';

interface ReviewNeededQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
  refreshQuizResults: () => void; // New prop to refresh the quiz results
}

const ReviewNeededQuestionResultDisplay: React.FC<ReviewNeededQuestionResultDisplayProps> = ({ question, questionResult, refreshQuizResults }) => {
  const [needReview, setNeedReview] = useState(false);
  const [questionReview, setReviewResults] = useState<QuestionReview | null>(null);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [loading, setLoading] = useState(false);
  const [score, setScore] = useState<number | null>(null);
  const [error, setError] = useState<string | null>(null);

  const getQuizResult = async (userId: string, quizId: string) => {
    setLoading(true);
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
      response.data.userId = userId;
      response.data.quizId = quizId;
      setQuizResults(response.data);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleGrade = async (userId: string, quizId: string, questionId: string, score: number) => {
    if (score < 0 || score > question.allocatedPoints) {
      setError(`Score must be between 0 and ${question.allocatedPoints}`);
      return;
    }

    try {
      const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult?userId=${userId}&quizId=${quizId}&questionId=${questionId}&finalScore=${score}`);
      setReviewResults(response.data);
      console.log('Review results:', response.data);
      await getQuizResult(userId, quizId);
      setNeedReview(false);
      questionResult.reviewNeededResult = response.data.updatedQuestionResult.reviewNeededResult;
      questionResult.score = score; 
      setError(null);
      setScore(score); // Update the score state
      refreshQuizResults(); // Call the refresh function to update the header
    } catch (error) {
      console.error('Error grading the answer!');
    }
  };

  if (loading) {
    return (
      <div className="fixed inset-0 bg-white flex justify-center items-center">
        <h1 className="text-2xl font-bold mb-4 text-center">Quiz Results</h1>
        <p>Loading quiz results...</p>
      </div>
    );
  }

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-center mb-2">
        <label
          className={classNames(
            'p-2 rounded-full w-full max-w-md text-left',
            {
              'bg-green-500 text-white border-4 border-solid border-green-700': questionResult.reviewNeededResult === AnswerResult.Correct,
              'bg-red-500 text-white border-4 border-solid border-red-700': questionResult.reviewNeededResult === AnswerResult.Wrong || questionResult.reviewNeededResult === AnswerResult.NotAnswered,
              'bg-yellow-300 text-black border-4 border-solid border-yellow-500': questionResult.reviewNeededResult === AnswerResult.PartiallyCorrect,
              'bg-teal-500 text-white border-4 border-solid border-teal-700': questionResult.reviewNeededResult === AnswerResult.Pending,
            }
          )}
        >
          Your Answer: {questionResult.reviewNeededAnswer}
        </label>
      </div>
      {needReview && (
        <div className="space-y-4">
          <label
            className={classNames(
              'p-2 full max-w-md text-left',
              'bg-teal-300 text-white border-4 border-solid border-teal-500 rounded-md'
            )}
            style={{
              display: 'grid',
              width: '90%',
              minHeight: '100px',
              maxHeight: '200px', 
              overflowY: 'auto',
            }}
          >
            AI Review: {questionReview?.updatedQuestionResult.LLMReview}
          </label>
      
          <input
            type="number"
            step="any"
            inputMode="numeric"
            value={score || ''}
            onChange={(e) => {
              const value = e.target.value;
              if ( parseFloat(value) >= 0 && parseFloat(value) <= question.allocatedPoints) {
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
            className="block w-24 mx-auto p-2 border border-gray-300 rounded text-center"
            placeholder="Score"
            style={{ appearance: 'textfield', MozAppearance: 'textfield', WebkitAppearance: 'none' }}
          />
    
          {error && <p className="text-red-500 text-center">{error}</p>}
          <button
            className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={() => handleGrade(questionResult.userId, question.quizId, question.id, score || 0)}
          >
            Grade
          </button>
        </div>
      )}
      {!needReview && question.type === QuestionType.ReviewNeeded && questionResult.reviewNeededResult === AnswerResult.Pending && (
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
