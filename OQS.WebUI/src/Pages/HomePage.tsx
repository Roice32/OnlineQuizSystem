import React from "react";

export default function HomePage() {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center animate-bounce">
        <h1 className="text-6xl font-bold animate-pulseColor animate-blingBling mb-4">
          Welcome to the Quiz App
        </h1>
      </div>
      <div className="w-1/2 bg-white rounded-xl p-6 text-center mt-8">
        <p className="text-1.2xl font-bold text-[#1c4e4f]">
        Dive into a world of fun and learning with our interactive quiz app. Test your knowledge by participating in exciting quizzes, join live quizzes for a real-time challenge, or unleash your creativity by creating your own quizzes to stump your friends. Think you’ve got what it takes? Let’s find out!
        </p>
      </div>
    </div>
  );
}