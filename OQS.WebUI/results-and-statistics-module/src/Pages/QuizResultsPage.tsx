import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { QuizResults } from "../utils/types/results-and-statistics/quiz-results";
import QuestionResultDisplay from '../Components/QuestionResultDisplays/QuestionResultDisplay';

const QuizResultsPage = () => {
  const { userId, quizId } = useParams<{ userId: string, quizId: string }>();
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [loading, setLoading] = useState(true);

  const handleBackClick = () => {
    window.history.back();
  };

  useEffect(() => {
    const getQuizResult = async (userId: string, quizId: string) => {
        try {
          const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
          console.log(response.data);
          response.data.userId = userId;
          response.data.quizId = quizId;
          setQuizResults(response.data);
          setLoading(false);
        } catch (error) {
          console.error('Error fetching quiz result:', error);
        }
      };

      if (userId && quizId) {
        getQuizResult(userId, quizId);
      }
  }, [userId, quizId]);

  if (loading) {
    return <p>Loading...</p>;
  }

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Results</h1>
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4 text-center p-4 rounded-3xl rounded-b-none border-4 border-solid border-gray-400">
          <p className="text-lg bg-gray-200 p-2 rounded-md">Username: {quizResults?.quizResultHeader.userName}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md">Quiz Name: {quizResults?.quizResultHeader.quizName}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md">Completion Time: {quizResults?.quizResultHeader.completionTime}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md">Score: {quizResults?.quizResultHeader.score}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md">
            {quizResults?.quizResultHeader.reviewPending ? 'Review Pending' : 'Review Not Pending'}
          </p>
        </div>
        <div className="p-4 rounded-3xl rounded-t-none border-4 border-t-0 border-solid border-gray-400">
          <h2 className="text-lg font-bold mb-2">Questions:</h2>
          <ul>
            {quizResults?.quizResultBody?.questions.map((header, index) => {
              const questionResult = quizResults.quizResultBody?.questionResults.find(item => item.questionId === header.id);
              return (
                <div key={index} className="flex items-center justify-center mb-2 mr-2">
                  <QuestionResultDisplay question={header} questionResult={questionResult} />
                </div>
              );
            })}
          </ul>
        </div>
        <button
          className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
          onClick={handleBackClick}
        >
          Back
        </button>
      </div>
    </div>
  );
};

export default QuizResultsPage;