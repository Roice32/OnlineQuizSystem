import React from "react";
import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <div
      className="min-h-screen flex flex-col items-start justify-center p-6 "
      style={{
        backgroundImage: 'url("https://wallpaperswide.com/download/polygon_green-wallpaper-1280x960.jpg")',
        backgroundSize: 'cover',
        backgroundPosition: 'center',
      }}
    >
      <div className="text-left animate-bounce  bg-[#1c4e4f] p-8 rounded-3xl shadow-md">
        <h1 className="text-6xl text-[#efd7cf] animate-pulseColor mb-4">
          Ready to Quiz?
        </h1>
      </div>
      <div className="w-2/3  rounded-xl mb-12 p-8 shadow-2xl">
        <p className="text-3xl  text-[#f7ebe7] mb-4">
          Dive into a world of fun and learning with our interactive quiz app. Test your knowledge by participating in exciting quizzes, join live quizzes for a real-time challenge, or unleash your creativity by creating your own quizzes to stump your friends. Think you’ve got what it takes? Let’s find out!
        </p>
      </div>
      <Link
        className="no-underline text-[#efd7cf] px-4 py-2 text-lg transition duration-300 hover:bg-[#879693] rounded-full bg-[#0a2d2e] flex items-center justify-center"
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
