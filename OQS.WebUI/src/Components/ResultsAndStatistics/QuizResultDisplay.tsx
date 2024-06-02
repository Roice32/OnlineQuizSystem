import { useEffect, useState } from 'react';
import axios from 'axios';
import { QuestionResult, QuizResults } from "../../utils/types/results-and-statistics/quiz-results";
import { QuestionReview } from "../../utils/types/results-and-statistics/question-review";
import QuestionResultDisplay from './QuestionResultDisplays/QuestionResultDisplay';

export default function QuizResultsDisplay({ quizResults }: { quizResults: QuizResults }) {
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);
  const [quizResultss, setQuizResults] = useState<QuizResults | null>(null);
  const [questionReview, setReviewResults] = useState<QuestionReview | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (quizResults) {
      setQuizResults(quizResults);
      setLoading(false);
    }
  }, [quizResults]);

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
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  useEffect(() => {
    console.log("QuizResults props:", quizResults);
    console.log("QuizResultHeaders:", quizResults?.quizResultHeader);
  }, [quizResults]);

  if (!quizResults.quizResultHeader) {
    return <p>Loading quiz results...</p>;
  }

  if (showMessage3) { // rescrie cu reviewul
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
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Results</h1>
        {loading ? (
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        ) : (
          <div>
  <div className="grid grid-cols-1 md:grid-cols-6 gap-4 text-center p-4 rounded-3xl rounded-b-none border-4 border-solid border-gray-400">
              <p className="text-lg bg-gray-200 p-2 rounded-md">Username: {quizResults.quizResultHeader.userName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Quiz Name: {quizResults.quizResultHeader.quizName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Completion Time: {quizResults.quizResultHeader.completionTime}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Score: {quizResults.quizResultHeader.score}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Submitted At: {formatDate(quizResults.quizResultHeader.submittedAt)}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">
                {quizResults.quizResultHeader.reviewPending ? "Review Pending" : "Review Not Pending"}
              </p>
            </div>
            <div className="p-4 rounded-3xl rounded-t-none border-4 border-t-0 border-solid border-gray-400">
              <h2 className="text-lg font-bold mb-2">Questions:</h2>
              <ul>
                {quizResults?.quizResultBody?.questions.map((header, index) => {
                  const questionResult2 = quizResults.quizResultBody.questionResults.find(item => item.questionId === header.id) as QuestionResult;
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
