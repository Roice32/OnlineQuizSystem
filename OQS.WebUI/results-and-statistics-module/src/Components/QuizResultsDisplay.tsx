import axios from "axios";
import { QuestionType } from "../utils/types/questions";
import { QuestionResult, QuizResults } from "../utils/types/results-and-statistics/quiz-results";
import { useEffect, useState } from "react";
import { AnswerResult, QuestionReview } from "../utils/types/results-and-statistics/question-review";
import TrueFalseQuestionResultDisplay from '../Components/SubmittedQuestions/TrueFalseQuestionResultDisplay';
import SingleChoiceQuestionResultDisplay from "./SubmittedQuestions/SingleChoiceQuestionResultDisplay";
import WrittenQuestionResultDisplay from "./SubmittedQuestions/WrittenQuestionResultDisplay";

export default function QuizResultsDisplay({ quizResults }: { quizResults: QuizResults }) {
  const [showMessage1, setShowMessage1] = useState(false);
  const [showMessage2, setShowMessage2] = useState(false);
  const [showMessage3, setShowMessage3] = useState(false);
  const [quizResultss, setQuizResults] = useState<QuizResults | null>(null);
  const [questionReview, setReviewResults] = useState<QuestionReview | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (quizResults) {
      setQuizResults(quizResults);
      setLoading(false);
    }
  }, [quizResults]);

  const getReview = async (userId: string, quizId: string, questionId?: string, score?: number) => {
    setShowMessage3(true);
    setShowMessage1(false);
    setShowMessage2(false);
    try {
      console.log(`Fetching review for user ID: ${userId} quizId: ${quizId} questionId: ${questionId} score: ${score}`);
      const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult/`, {
        userId: userId,
        quizId: quizId,
        questionId: questionId,
        score: score,
      });
      console.log(response.data);
      setReviewResults(response.data);
    } catch (error) {
      console.error('Error fetching quiz result:', error);
    }
  };

  useEffect(() => {
    console.log("QuizResults props:", quizResults);
    console.log("QuizResultHeaders:", quizResults?.quizResultHeader);
  }, [quizResults]);

  if (!quizResults.quizResultHeader) {
    return <p>Loading quiz results...</p>;
  }

  if (showMessage3) {
    return (
      <div>
        <div>
          {quizResults && <QuizResultsDisplay quizResults={quizResults} />}
        </div>
      </div>
    );
  }
  
  return (
    <div className="min-h-screen bg-[#1c4e4f] flex flex-col items-center p-6 font-mono">
      <div className="w-full max-w-2xl bg-white shadow-lg rounded-lg p-6 transition-transform transform hover:scale-105">
        <h1 className="text-2xl font-bold mb-4 animate-bounce text-center">Quiz Results</h1>
        {loading ? (
          <div className="text-center">
            <p className="text-lg">Loading...</p>
          </div>
        ) : (
          <div style={borderStyle}>
            <div style={borderStyle} className="grid grid-cols-1 md:grid-cols-5 gap-4 text-center">
              <p className="text-lg bg-gray-200 p-2 rounded-md">Username: {quizResults.quizResultHeader.userName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Quiz Name: {quizResults.quizResultHeader.quizName}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Completion Time: {quizResults.quizResultHeader.completionTime}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">Score: {quizResults.quizResultHeader.score}</p>
              <p className="text-lg bg-gray-200 p-2 rounded-md">
                {quizResults.quizResultHeader.reviewPending ? "Review Pending" : "Review Not Pending"}
              </p>
            </div>
            <div>
              <h2 className="text-lg font-bold mb-2">Questions:</h2>
              <ul>
                {quizResults?.quizResultBody?.questions.map((header, index) => {
                  const questionResult2 = quizResults.quizResultBody.questionResults.find(item => item.questionId === header.id) as QuestionResult;
                  let choicesResults = {};
                  if (questionResult2?.pseudoDictionaryChoicesResults) {
                    choicesResults = JSON.parse(questionResult2.pseudoDictionaryChoicesResults);
                  }
                  return (
                    <div key={index} className="flex items-center justify-center mb-2 mr-2">
                      {header.type === QuestionType.TrueFalse && (
                        <TrueFalseQuestionResultDisplay question={header} questionResult={questionResult2} />
                      )}
                      {header.type === QuestionType.SingleChoice && (
                        <SingleChoiceQuestionResultDisplay
                          question={header}
                          userAnswer={questionResult2}
                          correctAnswer={questionResult2.singleChoiceResult === AnswerResult.Correct}
                          questionText={header.text}
                          questionScore={questionResult2.score}
                          choices={Object.keys(choicesResults)}
                        />
                      )}
                      {header.type === QuestionType.MultipleChoice && (
                        <div>
                          <p>Possible answers: {Object.keys(choicesResults).join(", ")}</p>
                          <p>Correct answers: {Object.keys(choicesResults).filter(choice => choicesResults[choice] === AnswerResult.Correct || choicesResults[choice] === AnswerResult.CorrectNotPicked).join(", ")}</p>
                          <p>Your answers: {Object.keys(choicesResults).filter(choice => choicesResults[choice] !== AnswerResult.CorrectNotPicked && choicesResults[choice] !== AnswerResult.Other).join(", ")}</p>
                        </div>
                      )}
                      {header.type === QuestionType.WriteAnswer && (
                        <WrittenQuestionResultDisplay question={header} questionResult={questionResult2} />
                      )}
                      {header.type === QuestionType.ReviewNeeded && questionResult2.reviewNeededResult === AnswerResult.Pending && (
                        <button
                          className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
                          onClick={() => getReview(quizResults.userId, quizResults.quizId, questionResult2.questionId, questionResult2.score)}
                        >
                          Review
                        </button>
                      )}
                      {header.type === QuestionType.ReviewNeeded && questionResult2.reviewNeededResult !== AnswerResult.Pending && (
                        <WrittenQuestionResultDisplay question={header} questionResult={questionResult2} />
                      )}
                    </div>
                  );
                })}
              </ul>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

const borderStyle = {
  borderRadius: '50px',
  border: '1px solid gray',
  padding: '10px',
};
