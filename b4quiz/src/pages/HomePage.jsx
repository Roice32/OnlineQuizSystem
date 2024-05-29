import React, { useState, useEffect } from 'react';
import Navbar from '../components/Navbar';
import { useNavigate } from 'react-router-dom';

const HomePage = () => {
  const [faqs, setFaqs] = useState([
    { question: 'How do I create a quiz?', answer: 'To create a quiz, click on the "Create a Quiz" button.' },
    { question: 'Can I edit an existing quiz?', answer: 'Yes, you can edit any existing quiz from your dashboard.' },
  ]);

  const [newQuestion, setNewQuestion] = useState('');
  const [newAnswer, setNewAnswer] = useState('');
  const [quizzes, setQuizzes] = useState([]);
  const [quizQuestionsCount, setQuizQuestionsCount] = useState({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    fetch('http://localhost:5276/api/quizzes?offset=0&limit=8')
      .then(response => response.json())
      .then(async data => {
        const quizzes = data.quizzes;
        setQuizzes(quizzes);

        const counts = await Promise.all(quizzes.map(async (quiz) => {
          const response = await fetch(`http://localhost:5276/api/quizzes/${quiz.id}/questions`);
          const result = await response.json();
          return { quizId: quiz.id, count: result.value.length };
        }));

        const countsMap = counts.reduce((acc, { quizId, count }) => {
          acc[quizId] = count;
          return acc;
        }, {});

        setQuizQuestionsCount(countsMap);
        setLoading(false);
      })
      .catch(error => {
        console.error('Error fetching quizzes:', error);
        setError(error);
        setLoading(false);
      });
  }, []);

  const handleAddFaq = () => {
    if (newQuestion.trim() && newAnswer.trim()) {
      setFaqs([...faqs, { question: newQuestion, answer: newAnswer }]);
      setNewQuestion('');
      setNewAnswer('');
    }
  };

  const handleDeleteFaq = (indexToDelete) => {
    setFaqs(faqs.filter((_, index) => index !== indexToDelete));
  };

  const handleTakeQuiz = (quizId) => {
    navigate(`/quiz/${quizId}`);
  };

  return (
    <div>
      <Navbar />
      <div className="flex flex-col items-center">
        <h1 className="text-[#1c4e4f] text-4xl font-bold mt-6">B4Quiz</h1>
      </div>
      <div className="flex flex-wrap mt-10 px-6">
        <div className="w-full md:w-1/2 px-4">
          <h2 className="text-2xl text-[#1c4e4f] font-semibold mb-4">User Reviews</h2>
          <div className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 mb-4 rounded-md shadow-md">
            <p>⭐⭐⭐⭐⭐</p>
            <p>“This platform is fantastic! Easy to use and very efficient.” - Maria</p>
          </div>
          <div className="bg-[#efd7cf] bg-opacity-50 p-4 mb-4 rounded-md shadow-md">
            <p>⭐⭐⭐⭐</p>
            <p>“I really enjoyed using this platform for my quizzes.” - John</p>
          </div>
        </div>
        <div className="w-full md:w-1/2 px-4">
          <h2 className="text-2xl text-[#1c4e4f] font-semibold mb-4">Frequently Asked Questions (FAQ)</h2>
          <div className="space-y-4 mb-6">
            {faqs.map((faq, index) => (
              <div key={index} className="bg-[#436e6f] text-[#f7ebe7] bg-opacity-50 p-4 rounded-md shadow-md relative">
                <h3 className="font-semibold">{faq.question}</h3>
                <p>{faq.answer}</p>
                <button
                  onClick={() => handleDeleteFaq(index)}
                  className="absolute top-2 right-2 text-[#a49e97] hover:text-[#1c4e4f]"
                >
                  Delete
                </button>
              </div>
            ))}
          </div>
          <div className="bg-[#efd7cf] bg-opacity-50 p-4 rounded-md shadow-md">
            <input
              type="text"
              placeholder="New Question"
              value={newQuestion}
              onChange={(e) => setNewQuestion(e.target.value)}
              className="w-full p-2 mb-2 border border-gray-300 rounded-md bg-[#f7ebe7] bg-opacity-50"
            />
            <input
              type="text"
              placeholder="New Answer"
              value={newAnswer}
              onChange={(e) => setNewAnswer(e.target.value)}
              className="w-full p-2 mb-2 border border-gray-300 rounded-md bg-[#f7ebe7] bg-opacity-50"
            />
            <button
              onClick={handleAddFaq}
              className="w-full p-2 bg-[#436e6f] text-[#f7ebe7] rounded-md hover:bg-[#efd7cf] hover:text-[#1c4e4f]"
            >
              Add
            </button>
          </div>
        </div>
      </div>

      <div className="flex justify-start px-6 mt-10">
        <h2 className="text-2xl text-[#1c4e4f] font-semibold">Take a quiz</h2>
      </div>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 mt-10 px-6">
        {loading ? (
          <div className="col-span-full flex justify-center mt-10">
            <div className="spinner"></div>
          </div>
        ) : (
          quizzes.map((quiz, index) => (
            <div key={quiz.id} className={`p-4 rounded-md shadow-md bg-opacity-50 ${
              index % 2 === 0 ? 'bg-[#436e6f] text-[#f7ebe7]' : 'bg-[#efd7cf]'
            }`}>
              <img src={quiz.imageUrl} alt={quiz.imageUrl} onError={(e) => e.target.src = 'https://www.shutterstock.com/shutterstock/photos/2052894734/display_1500/stock-vector-quiz-and-question-marks-trivia-night-quiz-symbol-neon-sign-night-online-game-with-questions-2052894734.jpg'} className="w-full h-32 object-cover rounded-t-md"  />
              <div className="p-2">
                <h3 className="font-semibold">{quiz.name}</h3>
                <p>{quiz.description}</p>
                <div className="flex justify-between mt-2">
                  <button onClick={() => handleTakeQuiz(quiz.id)}
                  className={`${
              index % 2 === 0 ? 'bg-[#efd7cf] text-[#1c4e4f]' : ' bg-[#436e6f] text-[#f7ebe7]'
            } px-4 py-2 rounded-md ${index % 2 === 0 ? 'hover:bg-[#CBC2BB] hover:text-[#1c4e4f]' : 'hover:bg-[#1c4e4f] hover:text-[#f7ebe7]'}`}>
                    Take Quiz
                  </button>
                  <p>{quizQuestionsCount[quiz.id] || 0} Questions</p> {/* Afișează numărul de întrebări */}
                </div>
              </div>
            </div>
          ))
        )}
      </div>
    </div>
  );
};

export default HomePage;
