import { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import { QuizResult } from "../../utils/types/results-and-statistics/quiz-result";
import QuestionResultDisplay from '../../Components/ResultsAndStatistics/QuestionResultDisplays/QuestionResultDisplay';
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../redux/store';
import ErrorComponent from '../../Components/ResultsAndStatistics/ErrorComponent';
import { openSnackbar } from '../../redux/Snackbar/SnackbarState';

const QuizResultsPage = () => {
  const { resultId } = useParams<{ resultId: string }>();
  const userState = useSelector((state: RootState) => state.user);
  const dispatch = useDispatch();
  const [quizResult, setQuizResult] = useState<QuizResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [errorOccured, setErrorOccured] = useState('');
  const [recipientEmail, setRecipientEmail] = useState('');

  const handleBackClick = () => {
    window.history.back();
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 6);
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0];
  };

  useEffect(() => {
    const getQuizResult = async (resultId: string) => {
      try {
        const token = userState.user?.token;
        const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${resultId}`,
          {
            headers: {
                'Authorization': `Bearer ${token}`
            }
          }
        );
        setQuizResult(response.data);
      } catch (error) {
        setErrorOccured("An unexpected error occured.");
        if (axios.isAxiosError(error)) {
          if (error.response?.status === 404) {
            setErrorOccured("Could not find the requested quiz result.");
          } else if (error.response?.status === 401) {
            setErrorOccured("You do not have permission to view this quiz result.");
          } else {
            setErrorOccured("Invalid JWT token provided.");
          }
        }
      } finally {
        setLoading(false);
      }
    };

    if (resultId) {
      getQuizResult(resultId);
    }
  }, [resultId]);

  const sendQuizResultViaEmail = async () => {
    try {
      dispatch(
        openSnackbar({ message: "Sending...", severity: "info" })
      );
      const token = userState.user?.token;
      await axios.get(`http://localhost:5276/api/email/sendQuizResultViaEmail?recipientEmail=${recipientEmail}&resultId=${resultId}`,
        {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        }
      );
      dispatch(
        openSnackbar({ message: "Quiz result successfully sent to specified email.", severity: "success" })
      );
    } catch (error) {
      console.log(error);
      dispatch(
        openSnackbar({ message: "Failed to send quiz result to specified email. Check input field and try again.", severity: "error" })
      );
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
        <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
          <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Result</h1>
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        </div>
      </div>
    );
  }

  if (errorOccured !== '') {
    return (
      <ErrorComponent err={errorOccured} />
    )
  }

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Results</h1>
        <div className="grid grid-cols-3 gap-4 text-center p-4 rounded-3xl rounded-b-none border-4 border-solid border-gray-400">
          <p className="text-lg bg-gray-200 p-2 rounded-md flex items-center justify-center">Username: {quizResult?.quizResultHeader.userName}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md flex items-center justify-center">Quiz Name: {quizResult?.quizResultHeader.quizName}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md flex items-center justify-center">Score: {quizResult?.quizResultHeader.score}</p>
          <p className="text-lg bg-gray-200 p-2 rounded-md flex items-center justify-center">
            {quizResult?.quizResultHeader.reviewPending ? 'Review Pending' : 'Review Not Pending'}
          </p>
          <p className="col-span-4 text-lg bg-gray-200 p-2 rounded-md flex items-center justify-center">Submitted at: {quizResult ? formatDate(quizResult.quizResultHeader.submittedAtUtc) : "Unknown"}</p>
        </div>
        <div className="p-4 rounded-3xl rounded-t-none border-4 border-t-0 border-solid border-gray-400">
          <h2 className="text-lg font-bold mb-2">Questions:</h2>
          <ul>
            {quizResult?.quizResultBody?.questions.map((header, index) => {
              const questionResult = quizResult.quizResultBody?.questionResults.find(item => item.questionId === header.id);
              return (
                <div key={index} className="flex items-center justify-center mb-2 mr-2">
                  <QuestionResultDisplay question={header}
                    questionResult={questionResult!}
                    asQuizCreator={quizResult.asQuizCreator} />
                </div>
              );
            })}
          </ul>
        </div>
        <div className="mt-6">
          <label className="block mb-2">
            Recipient Email:
            <input
              type="email"
              className="block w-full mt-1 p-2 border-1 border-solid border-gray-300 rounded"
              value={recipientEmail}
              onChange={(e) => setRecipientEmail(e.target.value)}
            />
          </label>
          <button
            className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
            onClick={sendQuizResultViaEmail}
          >
            Send Quiz Result Via Email
          </button>
        </div>
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

export default QuizResultsPage;
