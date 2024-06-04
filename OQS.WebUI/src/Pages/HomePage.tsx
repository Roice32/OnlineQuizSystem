import React from "react";
import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center animate-bounce">
        <h1 className="text-6xl font-bold animate-pulseColor animate-blingBling mb-4">
          Welcome to the Quiz App
        </h1>
      </div>
      <div className="w-1/2 bg-white rounded-xl p-6 text-center mt-8">
        <p className="text-1.2xl font-bold text-[#1c4e4f] mb-4">
        Dive into a world of fun and learning with our interactive quiz app. Test your knowledge by participating in exciting quizzes, join live quizzes for a real-time challenge, or unleash your creativity by creating your own quizzes to stump your friends. Think you’ve got what it takes? Let’s find out!
        </p>
      </div>
      <Link
        className="no-underline text-white px-4 py-2 text-lg transition duration-300 hover:bg-[#deae9f] rounded-full bg-[#1c4e4f] mt-4 flex items-center justify-center"
        to="/quizzes"
      >
        Go to quizzes
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke="currentColor" className="h-6 w-6 ml-2">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M14 5l7 7m0 0l-7 7m7-7H3" />
        </svg>
      </Link>
    </div>
  );
}