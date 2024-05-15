import './App.css'
import React from 'react'
import QuizCreate from './pages/QuizCreate'
import QuizCreateFoo from './pages/QuizCreateFoo'
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom'

import Navbar from './components/Navbar'
import HomePage from './pages/HomePage'

import TagsPage from './pages/TagsPage'
import QuizzesPage from './pages/QuizzesPage'
import FormGetId from './pages/QuizGet'
import ShowInfo from './components/ShowQuizInfo'

function App(): JSX.Element {
  return (
    <Router>
      <div className="min-h-screen" style={{
  background: `radial-gradient(circle at 100% 100%, #6a8e8f 4%, rgba(56, 87, 30, 0) 40%),
              radial-gradient(circle at 6.504% 88.037%, #879693 0%, rgba(187, 201, 170, 0) 50%),
              radial-gradient(circle at 6.165% 12.617%, #879693 0%, rgba(135, 152, 106, 0) 83%),
              radial-gradient(circle at 93.687% 11.426%, #6a8e8f 0%, rgba(233, 245, 219, 0) 70%),
              radial-gradient(circle at 48.901% 49.521%, #efd7cf 0%, rgba(255, 255, 255, 0) 100%)`
}}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/create-quiz" element={<QuizCreate />} />
          <Route path="/create-foo" element={<QuizCreateFoo />} />

          <Route path="/my-quizzes" element={<HomePage />} />
          <Route path="/tags" element={<TagsPage />} />
          <Route path="/quiz" element={<FormGetId />} />
          <Route path="/quiz/:id" element={<FormGetId />} />
          <Route path="/profile" element={<HomePage />} />
          <Route path="/quizzes" element={<QuizzesPage />} />
        </Routes>
      </div>
    </Router>
  )
}

export default App
