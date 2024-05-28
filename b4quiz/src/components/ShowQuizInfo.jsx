import React, { useState, useEffect } from "react";

function ShowInfo({ quizID }) {
  const [quizData, setQuizData] = useState(null);
  const [imageSrc, setImageSrc] = useState('');
  const [error, setError] = useState(null);

  const fetchQuiz = async () => {
    try {
      const response = await fetch(`http://localhost:5276/api/quizzes/${quizID}`);
      if (!response.ok) {
        throw new Error('Failed to fetch Quiz');
      }

      const data = await response.json();
      console.log("Received data:", data);
      setQuizData(data.value);
      setImageSrc(data.value.imageUrl);
    } catch (error) {
      console.error('Error fetching quiz:', error);
      setError(error);
    }
  };

  useEffect(() => {
    if (!quizID) return;
    fetchQuiz();
  }, [quizID]);

  useEffect(() => {
    if (quizData && quizData.imageUrl) {
      const img = new Image();
      img.src = quizData.imageUrl;
      img.onload = () => setImageSrc(quizData.imageUrl);
      img.onerror = () => setImageSrc('https://www.shutterstock.com/shutterstock/photos/2052894734/display_1500/stock-vector-quiz-and-question-marks-trivia-night-quiz-symbol-neon-sign-night-online-game-with-questions-2052894734.jpg');
    }
  }, [quizData]);

  return (
    <div className="container mx-auto p-6">
      {error && quizData ? (
        <p className="text-red-500 text-center">{error.message}</p>
      ) : null}
      {quizData ? (
 
 <div className="bg-[#0A2D2E] p-8 rounded-lg shadow-md text-white mt-10 flex-col flex">
 <div className="font-bold text-5xl capitalize mb-12 text-center p-4 text-[#DEAE9F]">{quizData.name}</div>
 <div className="flex justify-center items-center"> 
     <div className="w-1/2 pr-8">
         <img 
             src={imageSrc} 
             alt="Quiz Image" 
             className="w-full h-full object-cover rounded-lg" 
         />
     </div>
     <div className="w-1/2 pl-8 text-center"> 
    <div className="mt-5">
        <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">ID:</div>
        <p className="text-lg">{quizData.id}</p>
    </div>
    <div className="mt-5">
        <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">Language:</div> 
        <p className="text-lg">{quizData.language}</p>
    </div>
    <div className="mt-5">
        <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">Time Limit (Minutes):</div> 
        <p className="text-lg">{quizData.timeLimitMinutes}</p>
    </div>
    <div className="mt-5">
        <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">Created At:</div> 
        <p className="text-lg">{new Date(quizData.createdAt).toLocaleString()}</p>
    </div>
    <div className="mt-10"> 
        <div className="font-bold text-3xl capitalize mb-2 text-[#DEAE9F]">Description:</div> 
        <p className="text-lg">{quizData.description}</p> 
    </div>
    
    <div>
    <button class="bg-[#DEAE9F] hover:bg-[#a49e97] text-white font-bold py-2 px-4 rounded mt-10">
        Take Quiz
    </button>
</div>
</div>

 </div>
</div>

      ) : (
        <div className="bg-red-200 p-8 rounded-lg shadow-md text-white mt-10 text-center">
        <h2 className="text-red-800 text-center text-lg">Quiz not found</h2>
        </div>
      )}
    </div>
  );
}

export default ShowInfo;
