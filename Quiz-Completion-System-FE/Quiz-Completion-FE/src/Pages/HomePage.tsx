import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function HomePage() {
  // redirect to /quiz
  const navigate = useNavigate();
  useEffect(() => {
    const timer = setTimeout(() => {
      navigate("/quiz");
    }, 3000); // Adding a 3-second delay for demonstration purposes

    return () => clearTimeout(timer); // Cleanup timer on component unmount
  }, [navigate]);

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
        <h1 className="text-4xl font-bold text-white mb-4">Welcome to the Quiz App</h1>
        <p className="text-xl text-gray-200">Redirecting you to the quiz{dots}</p>
      </div>
      <div className="spinner"></div>
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
