import './App.css';
import React from 'react';
import QuizCreate from './pages/QuizCreate';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import Navbar from './components/Navbar';
import HomePage from './pages/HomePage';
import QuizzesPage from "./pages/QuizzesPage";
import FormGetId from './pages/QuizGet';
import ShowInfo from './components/ShowQuizInfo';

function App() : JSX.Element {
  return (
    <Router>
      <div className="bg-[#0A2D2E] h-screen">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/create-quiz" element={<QuizCreate />} />
          <Route path="/quiz" element={<FormGetId />} />
          <Route path="/quiz/:id" element={<FormGetId />} />
          <Route path="/profile" element={<HomePage />} />
          <Route path="/quizzes" element={<QuizzesPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
