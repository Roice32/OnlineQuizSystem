import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";

export default function SubmittedQuiz() {
  const navigate = useNavigate();
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);

  useEffect(() => {
    navigate("/submittedQuiz");
  }, [navigate]);


  const getCreatedQuizStats = async () => {
    setShowMessage1(true);
    setShowMessage2(false);
    setShowMessage3(false);
    try {
      const quizId = "00000000-0000-0000-0002-000000000001";
      const response = await axios.get(`http://localhost:5276/api/quizResults/getCreatedQuizStats/${quizId}`);
      console.log(response.data);
      return response.data;
    } catch (error) {
      console.error('Error fetching created quiz stats:', error);
      return null;
    }
  };
  const getQuizResult = async (/* userId, quizId */) => {
    setShowMessage3(true);
    setShowMessage1(false);
    setShowMessage2(false);
   /*  try {
      const response = await axios.get(`/api/quizResults/getQuizResult/${userId}/${quizId}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching quiz result:', error);
      return null;
    } */
  };

  const getTakenQuizzesHistory = async (/* userId */) => {
     setShowMessage2(true);
     setShowMessage1(false);
     setShowMessage3(false);
     /* try {
       const response = await axios.get(`/api/quizResults/getTakenQuizzesHistory/${userId}`);
       return response.data;
     } catch (error) {
       console.error('Error fetching taken quizzes history:', error);
       return null;
     } */
  };

  const handleBackClick = () => {
    setShowMessage1(false);
    setShowMessage2(false);
    setShowMessage3(false);
  };

  if (showMessage1 || showMessage2 || showMessage3) {
    return (
      <div>
        <p style={{ fontSize: '30px', color: '#f7ebe7' }} 
           className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
           {showMessage1 && 'Clicked on first button'}
           {showMessage2 && 'Clicked on second button'}
           {showMessage3 && 'Clicked on third button'}
        </p>
        <button style={buttonStyle} onClick={handleBackClick}>Back</button>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center">
        <h1 style={{fontSize: '40px', color: '#f7ebe7'}} className="submittedQuiz">Submitted Quiz Responses:</h1>
        <button style={buttonStyle} onClick={() => getCreatedQuizStats(/* quizId */)}>GetCreatedQuizStatsExample</button>
        <button style={buttonStyle} onClick={() => getTakenQuizzesHistory(/* userId */)}>GetTakenQuizzesHistory</button>
        <button style={buttonStyle} onClick={() => getQuizResult(/* userId, quizId */)}>GetQuizResult</button>
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
