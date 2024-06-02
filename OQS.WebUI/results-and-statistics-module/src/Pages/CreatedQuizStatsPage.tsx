import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { CreatedQuizStats } from '../utils/types/results-and-statistics/created-quiz-stats';
import { QuizResults } from '../utils/types/results-and-statistics/quiz-results';
import { useParams } from 'react-router-dom';
import { useNavigate } from "react-router-dom";

const QuizStatsPage = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const [quizStats, setQuizStats] = useState<CreatedQuizStats | null>(null);
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);
  const [showMessage3, setShowMessage3] = useState(false);
  const [loading, setLoading] = useState(true);
  const [recipientEmail, setRecipientEmail] = useState('');
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const navigate = useNavigate();

  useEffect(() => {
    const getCreatedQuizStats = async (quizId: string) => {
      try {
        const response = await axios.get(`http://localhost:5276/api/quizResults/getCreatedQuizStats/${quizId}`);
        console.log(response.data); // Log the full response to inspect its structure
        setQuizStats(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching created quiz stats:', error);
      }
    };

    if (quizId) {
      getCreatedQuizStats(quizId);
    }
  }, [quizId]);

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    date.setHours(date.getHours() + 2); // Convert to Romania local time (UTC+2)
    const isoString = date.toISOString();
    return isoString.replace('T', ' ').split('.')[0];
  };

  const getQuizResult = async (userId: string, quizId: string) => {
    try {
      const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
      console.log(response.data);
      response.data.userId = userId;
      response.data.quizId = quizId;
      setQuizResults(response.data);
      navigate(`/quizResults/${userId}/${quizId}`);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  const sendQuizStatsViaEmail = async () => {
    try {
      await axios.post(`http://localhost:5276/api/email/sendCreatedQuizStatsViaEmail`, {
        recipientEmail: recipientEmail,
        quizId: quizId,
        startDateLocal: startDate,
        endDateLocal: endDate
      });
      alert('Quiz stats have been sent via email successfully.');
      console.log(recipientEmail, quizId, startDate, endDate);
    } catch (error) {
      console.error('Error sending quiz stats via email:', error);
      alert('Failed to send quiz stats via email.');
    }
  };

  const handleBackClick = () => {
    navigate(-1);
  };

  if (showMessage3) {
    return (
      <div>
        <button
          className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
          onClick={handleBackClick}
        >
          Back
        </button>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Created Quiz Stats</h1>
        {loading ? (
          <p>Loading...</p>
        ) : (
          <>
            <p className="text-lg text-center mb-4">Quiz Name: {quizStats?.quizName}</p>
            <h2 className="text-lg font-bold mb-2">Results:</h2>
            <div>
              {quizStats?.quizResultHeaders.map((header, index) => {
                console.log(quizStats.userNames[header.userId]);
                return (
                  <div key={index} className="mb-2" style={borderStyle}>
                    <p>Username: {quizStats.userNames[header.userId]}</p>
                    <p>Score: {header.score}</p>
                    <p>Submitted at: {formatDate(header.submittedAtUtc.toLocaleString())}</p>
                    <p>Review Pending: {header.reviewPending ? 'Yes' : 'No'}</p>
                    <button
                      className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
                      onClick={() => getQuizResult(header.userId, header.quizId)}
                    >
                      Show more details about the quiz
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
                  className="block w-full mt-1 p-2 border border-gray-300 rounded"
                  value={recipientEmail}
                  onChange={(e) => setRecipientEmail(e.target.value)}
                />
              </label>
              <label className="block mb-2">
                Start Date:
                <input
                  type="date"
                  className="block w-full mt-1 p-2 border border-gray-300 rounded"
                  value={startDate}
                  onChange={(e) => setStartDate(e.target.value)}
                />
              </label>
              <label className="block mb-2">
                End Date:
                <input
                  type="date"
                  className="block w-full mt-1 p-2 border border-gray-300 rounded"
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

const borderStyle = {
  borderRadius: '50px',
  border: '1px solid gray',
  padding: '20px',
};

export default QuizStatsPage;