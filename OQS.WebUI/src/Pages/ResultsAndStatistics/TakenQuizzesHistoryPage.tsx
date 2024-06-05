import { useEffect, useState } from "react";
import axios from "axios";
import { Link, useParams } from "react-router-dom";
import { TakenQuizzesHistory } from "../../utils/types/results-and-statistics/taken-quizzes-history";
import ErrorComponent from "../../Components/ResultsAndStatistics/ErrorComponent";
import { useSelector } from "react-redux";
import { RootState } from "../../redux/store";

const TakenQuizzesHistoryPage = () => {
  const { userId } = useParams<{ userId: string }>();
  const userState = useSelector((state: RootState) => state.user);
  const [quizHistory, setQuizHistory] = useState<TakenQuizzesHistory | null>(null);
  const [loading, setLoading] = useState(true);
  const [errorOccured, setErrorOccured] = useState('');

  useEffect(() => {
    const getTakenQuizzesHistory = async (userId: string) => {
      try {
        const token = userState.user?.token;
        const response = await axios.get(`http://localhost:5276/api/quizResults/getTakenQuizzesHistory/${userId}`,
        {
          headers: {
              'Authorization': `Bearer ${token}`
          }
        }
        );
        setQuizHistory(response.data);
      } catch (error) {
        setErrorOccured("An unexpected error occured.");
        if (axios.isAxiosError(error)) {
          if (error.response?.status === 404) {
            setErrorOccured("Could not find the requested user.");
          } else if (error.response?.status === 401) {
            setErrorOccured("You do not have permission to view another user's history.");
          } else {
            setErrorOccured("Invalid JWT token provided.");
          }
        }
      } finally {
        setLoading(false);
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

  if(loading) {
    return (
      <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
        <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
          <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Taken Quizzes History</h1>
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        </div>
      </div>
    )
  }

  if (errorOccured !== '') {
    return (
      <ErrorComponent err={errorOccured} />
    )
  }

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Taken Quizzes History</h1>
        {!quizHistory || quizHistory.quizResultHeaders.length === 0 ? (
            <div className="text-center">
              <p className="text-lg">No quizzes taken yet.</p>
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
                    <Link className="no-underline" to={`/quiz-result/${header.resultId}`}>View more details</Link>
                  </button>
                </div>
              ))}
            </div>
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