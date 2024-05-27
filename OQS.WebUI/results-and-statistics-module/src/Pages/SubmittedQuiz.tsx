import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import CreatedQuizStatsDisplay from "../Components/CreatedQuizStatsDisplay";
import TakenQuizzesHistoryDisplay from "../Components/TakenQuizzesHistoryDisplay";
import { CreatedQuizStats } from "../utils/types/results-and-statistics/created-quiz-stats";
import { QuizResults } from "../utils/types/results-and-statistics/quiz-results";
import QuizResultsDisplay from "../Components/QuizResultsDisplay";

export default function SubmittedQuiz() {
  const navigate = useNavigate();
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);
  const [userId, setUserId] = useState<string | null>(null);

  useEffect(() => {
    navigate("/submittedQuiz");
  }, [navigate]);

  const [quizStats, setQuizStats] = useState<CreatedQuizStats | null>(null);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);

  const getCreatedQuizStats = async (quizId: string) => {
    setShowMessage1(true);
    setShowMessage2(false);
    setShowMessage3(false);
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getCreatedQuizStats/${quizId}`);
      console.log(response.data);
      setQuizStats(response.data);
    } catch (error) {
      console.error('Error fetching created quiz stats:', error);
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
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  const getTakenQuizzesHistory = async (userId: string) => {
    setShowMessage2(true);
    setShowMessage1(false);
    setShowMessage3(false);
    setUserId(userId);
  };

  const handleBackClick = () => {
    setShowMessage1(false);
    setShowMessage2(false);
    setShowMessage3(false);
  };

  if (showMessage1) {
    return (
      <div>
        <div>
          {quizStats && <CreatedQuizStatsDisplay quizStats={quizStats} />}
        </div>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4" onClick={handleBackClick}>Back</button>
      </div>
    );
  }

  if (showMessage2 && userId) {
    return (
      <div>
        <div>
          <TakenQuizzesHistoryDisplay userId={userId} />
        </div>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4" onClick={handleBackClick}>Back</button>
      </div>
    );
  }

  if (showMessage3) {
    return (
      <div>
        <div>
          {quizResults && <QuizResultsDisplay quizResults={quizResults} />}
        </div>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4" onClick={handleBackClick}>Back</button>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center">
        <h1 style={{ fontSize: '40px', color: '#f7ebe7' }} className="submittedQuiz">Submitted Quiz Responses:</h1>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mb-4" onClick={() => getCreatedQuizStats("00000000-0000-0000-0002-000000000001")}>Get Created Quiz Stats Example</button>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mb-4" onClick={() => getTakenQuizzesHistory("00000000-0000-0000-0001-000000000001")}>Get Taken Quizzes History</button>
        <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mb-4" onClick={() => getQuizResult("00000000-0000-0000-0001-000000000003", "00000000-0000-0000-0002-000000000003")}>Get Quiz Result</button>
      </div>
    </div>
  );
}

const buttonStyle = {
  display: 'block',
  width: '300px',
  height: '50px',
  margin: '20px auto',
  backgroundColor: '#436e6f',
  color: 'white',
  borderRadius: '50px',
  textAlign: 'center',
  lineHeight: '50px',
  fontSize: '18px',
  textDecoration: 'none',
  textDecorationColor: '#f7ebe7',
};
