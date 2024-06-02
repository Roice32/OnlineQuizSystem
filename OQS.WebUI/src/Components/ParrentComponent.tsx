import React, { useEffect, useState } from "react";
import axios from "axios";
import QuizResultsDisplay from "./ResultsAndStatistics/QuizResultDisplay";
import { QuizResults } from "../utils/types/results-and-statistics/quiz-results";

const ParentComponent = ({ quizId }: { quizId: string }) => {
  const [quizResults, setQuizResults] = useState<QuizResults | null>(null);

  useEffect(() => {
    const fetchQuizResults = async () => {
      try {
        const response = await axios.get<QuizResults>(`http://localhost:5276/api/quizResults/getQuizResults/${quizId}`);
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
