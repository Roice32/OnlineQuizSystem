import './App.css';
import React from 'react';
import QuizCreate from './pages/QuizCreate';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

import Navbar from './components/Navbar';
import HomePage from './pages/HomePage';
import TagsPage from './pages/TagsPage';

function App() : JSX.Element {
  return (
    <Router>
      <div className="bg-[#0A2D2E] h-screen">
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/create-quiz" element={<QuizCreate />} />
          <Route path="/my-quizzes" element={<HomePage />} /> 
          <Route path="/tags" element={< TagsPage />} /> 
        </Routes>
      </div>
    </Router>
  );
}

export default App;
