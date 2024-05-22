import React, { useEffect, useState } from "react";
import axios from "axios";
import { TakenQuizStats } from "../utils/types/results-and-statistics/taken-quizzes-history";
import { useNavigate } from "react-router-dom";

export default function TakenQuizzesHistory({ userId }: { userId: string }) {
  const [quizHistory, setQuizHistory] = useState<TakenQuizStats | null>(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

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
    date.setHours(date.getHours() + 2); // Convert to Romania local time (UTC+2)
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0]; 
  };


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
            <ul>
              {quizHistory.quizResultHeaders.map((header, index) => (
                <li key={index} className="mb-2">
                  <p>Quiz Name: {quizHistory.quizNames[header.quizId]}</p>
                  <p>Score: {header.score}</p>
                  <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
                  <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
                </li>
              ))}
            </ul>
          )
        )}
      </div>
    </div>
    
  );
}

