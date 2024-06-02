import { useEffect, useState } from "react";
import axios from "axios";
import { Link, useParams } from "react-router-dom";
import { TakenQuizStats } from "../../utils/types/results-and-statistics/taken-quizzes-history";

const TakenQuizzesHistoryPage = () => {
  const { userId } = useParams<{ userId: string }>();
  const [quizHistory, setQuizHistory] = useState<TakenQuizStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const getTakenQuizzesHistory = async (userId: string) => {
      try {
        const response = await axios.get(`http://localhost:5276/api/quizResults/getTakenQuizzesHistory/${userId}`);
        setQuizHistory(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching created quiz stats:', error);
      }
    };
    if (userId)
      getTakenQuizzesHistory(userId);
  
  }, [userId]);


  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2);
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0];
  };

  const handleBackClick = () => {
    window.history.back();
  };

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
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
                  <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4">
                    <Link className="no-underline" to={`/quiz-result/${userId}/${header.quizId}`}>View more details</Link>
                  </button>
                </div>
              ))}
            </div>
          )
        )}
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

export default TakenQuizzesHistoryPage;