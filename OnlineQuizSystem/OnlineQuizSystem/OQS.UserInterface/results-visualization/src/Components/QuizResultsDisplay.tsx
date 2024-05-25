import axios from "axios";
import { QuestionType } from "../utils/types/questions";
import { QuizResults } from "../utils/types/results-and-statistics/quiz-results";


export default function QuizResultsDisplay({ quizResults }: { quizResults: QuizResults }) {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2); 
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0]; 
  };
  const getReview= async (userId: string, quizId: string) => {
    // setShowMessage3(true);
    // setShowMessage1(false);
    // setShowMessage2(false);
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
      console.log(response.data);
      // setQuizResults(response.data);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Results</h1>
        <div className="text-center mb-4">
          <p className="text-lg">Username: {quizResults?.quizResultHeaders?.username}</p>
          <p className="text-lg">Quiz Name: {quizResults?.quizResultHeaders?.quizName}</p>
          <p className="text-lg">Completion Time: {quizResults?.quizResultHeaders?.completionTime}</p>
          <p className="text-lg">Score: {quizResults?.quizResultHeaders?.score}</p>
          <p className="text-lg">
            {quizResults?.quizResultHeaders?.reviewPending ? "Review Pending" : "Review Not Pending"}
          </p>
        </div>
        <div>
          <h2 className="text-lg font-bold mb-2">Questions:</h2>
          <ul>
            {quizResults?.quizResultBody?.questions.map((header, index) => {
              const questionResult2 = quizResults.quizResultBody.questionResults.find(item => item.questionId === header.id);
              return (
                <li key={index} className="mb-2">
                  <p>Question Text: {header.text}</p>
                  <p>
                    {questionResult2 ? `Question Score: ${questionResult2.score}` : "No Question Result Found"}
                  </p>
                  {header.type === QuestionType.ReviewNeeded && <button style={buttonStyle} onClick={() => getReview(userId, quizId, questionId, finalScore)}>Review</button>}
                </li>
              );
            })}
          </ul>
        </div>
      </div>
    </div>
  );
}

const buttonStyle = {
  display: 'block',
  width: '200px',
  height: '35px',
  backgroundColor: '#436e6f',
  color: 'white',
  borderRadius: '50px',
  alignItems: 'center',
  textAlign: 'center',
  fontSize: '12px',
  textDecoration: 'none',
  textDecorationColor: '#f7ebe7',
};