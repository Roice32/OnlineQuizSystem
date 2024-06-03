import React, { useEffect, useState } from "react";
import axios from "axios";
import QuizResultsDisplay from "./ResultsAndStatistics/QuizResultDisplay";
import { QuizResult } from "../utils/types/results-and-statistics/quiz-result";

const ParentComponent = ({ quizId }: { quizId: string }) => {
  const [quizResults, setQuizResults] = useState<QuizResult | null>(null);

  useEffect(() => {
    const fetchQuizResults = async () => {
      try {
        const response = await axios.get<QuizResult>(`http://localhost:5276/api/quizResults/getQuizResults/${quizId}`);
        console.log("Fetched quiz results:", response.data);
        setQuizResults(response.data);
      } catch (error) {
        console.error("Error fetching quiz results:", error);
      }
    };

    fetchQuizResults();
  }, [quizId]);

  return (
    <div>
      {quizResults ? (
        <QuizResultsDisplay quizResults={quizResults} />
      ) : (
        <p>Loading...</p>
      )}
    </div>
  );
};

export default ParentComponent;
