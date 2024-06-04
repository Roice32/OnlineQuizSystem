import { useState, useEffect } from 'react';
import axios from 'axios';
import { CreatedQuizStats } from '../../utils/types/results-and-statistics/created-quiz-stats';
import { Link, useParams } from 'react-router-dom';
import { useNavigate } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux';
import { RootState } from '../../redux/store';
import ErrorComponent from '../../Components/ResultsAndStatistics/ErrorComponent';
import { openSnackbar } from '../../redux/Snackbar/SnackbarState';

const QuizStatsPage = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const userState = useSelector((state: RootState) => state.user);
  const [quizStats, setQuizStats] = useState<CreatedQuizStats | null>(null);
  const [loading, setLoading] = useState(true);
  const [errorOccured, setErrorOccured] = useState('');
  const [recipientEmail, setRecipientEmail] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const navigate = useNavigate();
  const dispatch = useDispatch();

  useEffect(() => {
    const getCreatedQuizStats = async (quizId: string) => {
      try {
        const token = userState.user?.token;
        const response = await axios.get(`http://localhost:5276/api/quizResults/getCreatedQuizStats/${quizId}`,
          {
            headers: {
                'Authorization': `Bearer ${token}`
            }
          }
        );
        setQuizStats(response.data);
      } catch (error) {
        setErrorOccured("An unexpected error occured.");
        if (axios.isAxiosError(error)) {
          if (error.response?.status === 404) {
            setErrorOccured("Could not find the requested quiz.");
          } else if (error.response?.status === 401) {
            setErrorOccured("You do not have permission to view stats for quiz you did not create.");
          } else {
            setErrorOccured("Invalid JWT token provided.");
          }
        }
      } finally {
        setLoading(false);
      }
    };

    if (quizId) {
      getCreatedQuizStats(quizId);
    }
  }, [quizId]);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2);
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0];
  };

  const sendQuizStatsViaEmail = async () => {
    try {
      dispatch(
        openSnackbar({ message: "Sending...", severity: "info" })
      );
      const token = userState.user?.token;
      await axios.get(`http://localhost:5276/api/email/sendCreatedQuizStatsViaEmail?quizId=${quizId}&recipientEmail=${recipientEmail}&startDate=${startDate}&endDate=${endDate}`,
      {
        headers: {
          'Authorization': `Bearer ${token}`
        }
      }
    );
      dispatch(
        openSnackbar({ message: "Quiz stats for given time period successfully sent to specified email.", severity: "success" })
      );
    } catch (error) {
      console.log(error);
      dispatch(
        openSnackbar({ message: "Failed to send quiz stats to specified email. Check input fields and try again.", severity: "error" })
      );
    }
  };

  const handleBackClick = () => {
    navigate(-1);
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
        <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6">
          <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Created Quiz Stats</h1>
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
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Created Quiz Stats</h1>
        {quizStats?.quizResultHeaders.length ? (
          <>
            <p className="text-lg text-center mb-4">Quiz Name: {quizStats.quizName}</p>
            <h2 className="text-lg font-bold mb-2">Results:</h2>
            <div>
              {quizStats.quizResultHeaders.map((header, index) => {
                return (
                  <div key={index} className="mb-2 p-4 rounded-[50px] border-2 border-gray-500">
                    <p>Username: {quizStats.userNames[header.userId]}</p>
                    <p>Score: {header.score}</p>
                    <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
                    <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
                  <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4">
                    <Link className="no-underline" to={`/quiz-result/${header.userId}/${quizId}`}>View more details</Link>
                  </button>
                  </div>
                );
              })}
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
              <label className="block mb-2">
                Start Date:
                <input
                  type="date"
                  className="block w-full mt-1 p-2 border-1 border-solid border-gray-300 rounded"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                />
              </label>
              <label className="block mb-2">
                End Date:
                <input
                  type="date"
                  className="block w-full mt-1 p-2 border-1 border-solid border-gray-300 rounded"
                  value={endDate}
                  onChange={(e) => setEndDate(e.target.value)}
                />
              </label>
              <button
                className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
                onClick={sendQuizStatsViaEmail}
              >
                Send Quiz Stats Via Email
              </button>
            </div>
          </>
        ) : (
          <p className="text-lg text-center mb-4">Nobody took your quiz yet.</p>
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

export default QuizStatsPage;
