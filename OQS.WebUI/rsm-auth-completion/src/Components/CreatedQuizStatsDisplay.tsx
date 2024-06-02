import { useEffect, useState } from "react";
import { CreatedQuizStats } from "../utils/types/results-and-statistics/created-quiz-stats";
import { TakenQuizStats } from "../utils/types/results-and-statistics/taken-quizzes-history";
import axios from "axios";
import { QuizResults } from "../utils/types/results-and-statistics/quiz-results";
import QuizResultsDisplay from "./QuizResultsDisplay";

export default function CreatedQuizStatsDisplay({ quizStats }: { quizStats: CreatedQuizStats }) {
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [showMessage3, setShowMessage3] = useState(false);
  const [loading, setLoading] = useState(true);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2); // Convert to Romania local time (UTC+2)
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0]; 
  };

  const getQuizResult = async (userId: string, quizId: string) => {
    setShowMessage3(true);
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
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Created Quiz Stats</h1>
        <p className="text-lg text-center mb-4">Quiz Name: {quizStats.quizName}</p>
        <h2 className="text-lg font-bold mb-2">Results:</h2>
        <div>
          {quizStats.quizResultHeaders.map((header, index) => (
            <div key={index} className="mb-2"  style={borderStyle}>
              <p>Username: {quizStats.userNames[header.userId].username}</p>
              <p>Score: {header.score}</p>
              <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
              <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
              <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4" onClick={() => getQuizResult( header.userId, header.quizId)}>Show more details about the quiz</button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

const borderStyle = {
  borderRadius: '50px',
  border: '1px solid gray',
  padding: '20px',
};