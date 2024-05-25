import { CreatedQuizStats } from "../utils/types/results-and-statistics/created-quiz-stats";

export default function CreatedQuizStatsDisplay({ quizStats }: { quizStats: CreatedQuizStats }) {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2); // Convert to Romania local time (UTC+2)
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0]; 
  };

  
  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Created Quiz Stats</h1>
        <p className="text-lg text-center mb-4">Quiz Name: {quizStats.quizName}</p>
        <h2 className="text-lg font-bold mb-2">Results:</h2>
        <ul>
          {quizStats.quizResultHeaders.map((header, index) => (
            <li key={index} className="mb-2">
              <p>Username: {quizStats.userNames[header.userId]}</p>
              <p>Score: {header.score}</p>
              <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
              <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
