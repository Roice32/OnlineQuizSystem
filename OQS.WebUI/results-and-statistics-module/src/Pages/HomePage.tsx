import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

export default function HomePage() {
  // redirect to /quiz
  const navigate = useNavigate();
  useEffect(() => {
    navigate("/quiz");
  }, [navigate]);
  
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-[#1c4e4f] p-6">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-white mb-4">Welcome to the Quiz App</h1>
        <p className="text-xl text-gray-200">Redirecting you to the quiz...</p>
      </div>
    </div>
  );
}
