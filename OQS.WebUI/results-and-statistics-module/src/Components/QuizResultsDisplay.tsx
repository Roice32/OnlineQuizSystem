import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { QuestionResult, QuizResults } from "../utils/types/results-and-statistics/quiz-results";
import { QuestionReview } from "../utils/types/results-and-statistics/question-review";
import QuestionResultDisplay from './QuestionResultDisplays/QuestionResultDisplay';

export default function QuizResultsDisplay({ quizResults: initialQuizResults }: { quizResults: QuizResults }) {
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);
  const [quizResultss, setQuizResults] = useState<QuizResults | null>(initialQuizResults);
  const [questionReview, setReviewResults] = useState<QuestionReview | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (initialQuizResults) {
      setQuizResults(initialQuizResults);
    }
  }, [initialQuizResults]);

  const fetchQuizResults = async () => {
    try {
      setLoading(true);
      const response = await axios.get('/api/quizResults'); // Adjust the endpoint accordingly
      setQuizResults(response.data);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching quiz results:', error);
      setLoading(false);
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
      setShowMessage3(false); // Ensure that the message is hidden after the data is fetched
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2);
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0];
  };

  const getReview = async (userId: string, quizId: string, questionId?: string, score?: number) => {
    setShowMessage3(true);
    setShowMessage1(false);
    setShowMessage2(false);
    try {
      console.log(`Fetching review for user ID: ${userId} quizId: ${quizId} questionId: ${questionId} score: ${score}`);
      const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult/`, {
        userId: userId,
        quizId: quizId,
        questionId: questionId,
        score: score,
      });
      console.log(response.data);
      setReviewResults(response.data);
      await getQuizResult(userId, quizId); // Refresh the quiz results after submitting the review
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  useEffect(() => {
    console.log("QuizResults props:", quizResultss);
    console.log("QuizResultHeaders:", quizResultss?.quizResultHeader);
  }, [quizResultss]);

  if (loading || !quizResultss) {
    return <p>Loading quiz results...</p>;
  }

  return (
    <div className="flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-4xl bg-white shadow-lg rounded-lg p-6 border-4 border-solid border-green-700">
        <h1 className="text-2xl font-bold mb-4 text-center">Quiz Results</h1>
        {showMessage3 ? (
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        ) : (
          <div>
            <div className="grid grid-cols-1 md:grid-cols-5 gap-4 text-center p-4 rounded-3xl rounded-b-none border-4 border-solid border-gray-400">
              <p className="text-lg bg-gray-200 p-2 rounded-md">Username: {quizResultss.quizResultHeader.userName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Quiz Name: {quizResultss.quizResultHeader.quizName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Completion Time: {quizResultss.quizResultHeader.completionTime}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Score: {quizResultss.quizResultHeader.score}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">
                {quizResultss.quizResultHeader.reviewPending ? "Review Pending" : "Review Not Pending"}
              </p>
            </div>
            <div className="p-4 rounded-3xl rounded-t-none border-4 border-t-0 border-solid border-gray-400">
              <h2 className="text-lg font-bold mb-2">Questions:</h2>
              <ul>
                {quizResultss.quizResultBody?.questions.map((header, index) => {
                  const questionResult2 = quizResultss.quizResultBody.questionResults.find(item => item.questionId === header.id) as QuestionResult;
                  return (
                    <div key={index} className="flex items-center justify-center mb-2 mr-2">
                      <QuestionResultDisplay question={header} questionResult={questionResult2} />
                    </div>
                  );
                })}
              </ul>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
