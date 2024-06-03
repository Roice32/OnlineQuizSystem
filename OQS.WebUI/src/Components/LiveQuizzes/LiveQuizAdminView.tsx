import React from 'react';
import { HubConnection } from "@microsoft/signalr";
import { useLocation } from "react-router-dom";

interface PageProps {
  connectedUsers: string[];
  connection: HubConnection | null;
  startQuiz: () => void;
  exitQuiz: () => void;
}

export default function LiveQuizAdminView({
  connectedUsers,
  connection,
  startQuiz,
  exitQuiz,
}: PageProps) {
  const location = useLocation();
  const code = location.pathname.split("/")[2];
  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col justify-center items-center p-6 font-mono">
    <div className="w-full max-w-lg bg-white shadow-2xl rounded-xl p-6 mb-6 transform transition-transform duration-500 hover:scale-105">
      <h1 className="text-4xl font-bold text-[#1c4e4f] mb-6 text-center">Live Quiz</h1>
      <h2 className="text-2xl text-center text-[#1c4e4f] mb-4">Live Quiz Code: {code}</h2>
      <div className="bg-[#f5f5f5] rounded-xl shadow-inner p-4">
        <ul className="list-none m-0 p-0">
          {connectedUsers.map((user, i) => (
            <li 
              key={i} 
              className="text-lg text-black py-2 px-4 border-b border-[#6a8e8f] last:border-none hover:bg-[#d0e6e6] transition-colors duration-300 text-center"
            >
              {user}
            </li>
          ))}
        </ul>
      </div>
      <div className="flex justify-center space-x-4">
        <button 
          onClick={startQuiz} 
          className="mt-6 w-[50%] bg-[#6a8e8f] text-white font-bold px-4 py-2 rounded-full transform transition-transform duration-300 hover:bg-[#1c4e4f] hover:scale-110 shadow-lg"
        >
          Start Quiz
        </button>
        <button 
          onClick={exitQuiz} 
          className="mt-6 w-[50%] bg-red-500 text-white font-bold px-4 py-2 rounded-full transform transition-transform duration-300 hover:bg-red-700 hover:scale-110 shadow-lg"
        >
          Exit
        </button>
      </div>
    </div>
  </div>
  
  );
}
