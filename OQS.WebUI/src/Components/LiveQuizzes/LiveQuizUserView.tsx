import React, { useEffect, useState } from "react";

interface PageProps {
  exitQuiz: () => void;
}

export default function LiveQuizAdminView({ exitQuiz }: PageProps) {
  // State for the animated dots
  const [dots, setDots] = useState('');

  useEffect(() => {
    const interval = setInterval(() => {
      setDots(prevDots => (prevDots.length < 3 ? prevDots + '.' : ''));
    }, 500); // Change the dot every 500ms

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center mb-8">
        <h1 className="text-4xl font-bold text-white mb-4">Live Quiz</h1>
        <p className="text-xl text-gray-200">Waiting for creator to start the quiz{dots}</p>
      </div>
      <div className="spinner"></div>
      <button 
        onClick={exitQuiz} 
        className="mt-8 px-4 py-2 bg-red-600 text-white font-bold rounded-full"
      >
        Exit
      </button>
      <style>{`
        .spinner {
          border: 8px solid rgba(255, 255, 255, 0.3);
          border-top: 8px solid white;
          border-radius: 50%;
          width: 60px;
          height: 60px;
          animation: spin 1s linear infinite;
        }

        @keyframes spin {
          0% {
            transform: rotate(0deg);
          }
          100% {
            transform: rotate(360deg);
          }
        }
      `}</style>
    </div>
  );
}
