import React, { useEffect, useState } from "react";
import axios from "axios";
import { TakenQuizStats } from "../../utils/types/results-and-statistics/taken-quizzes-history";
import { QuizResults } from "../../utils/types/results-and-statistics/quiz-results";
import QuizResultsDisplay from "./QuizResultDisplay";

export default function TakenQuizzesHistory({ userId }: { userId: string }) {
  const [quizHistory, setQuizHistory] = useState<TakenQuizStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);

  useEffect(() => {
    const fetchQuizHistory = async () => {
      try {
        console.log(`Fetching quiz history for user ID: ${userId}`);
        const response = await axios.get<TakenQuizStats>(`http://localhost:5276/api/quizResults/getTakenQuizzesHistory/${userId}`);
        console.log("Response data:", response.data);
        setQuizHistory(response.data);
      } catch (error) {
        console.error('Error fetching quiz history:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchQuizHistory();
  }, [userId]);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2);
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0]; 
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
  }


  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Taken Quizzes History</h1>
        {loading ? (
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        ) : (
          !quizHistory || quizHistory.quizResultHeaders.length === 0 ? (
            <div className="text-center">
              <p className="text-lg">No quiz history found.</p>
            </div>
          ) : (
            <div>
              {quizHistory.quizResultHeaders.map((header, index) => (
                <div key={index} className="mb-2 p-5 rounded-[50px] border-2 border-gray-500">
                  <p>Quiz Name: {quizHistory.quizNames[header.quizId]}</p>
                  <p>Score: {header.score}</p>
                  <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
                  <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
                  <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4" onClick={() => getQuizResult( userId , header.quizId)}>Show more details about the quiz</button>
                </div>
              ))}
            </div>
          )
        )}
      </div>
    </div>
    
  );
}
