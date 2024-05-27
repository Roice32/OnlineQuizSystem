import React, { useState } from 'react';
import { QuestionBase, QuestionType } from "../../utils/types/questions";
import { Answer } from "../../utils/types/active-quiz";
import { Question, QuestionResult, QuizResults } from '../../utils/types/results-and-statistics/quiz-results';
import { AnswerResult, QuestionReview } from '../../utils/types/results-and-statistics/question-review';
import axios from 'axios';
import QuizResultsDisplay from '../QuizResultsDisplay';
import classNames from 'classnames';

interface ReviewNeededQuestionResultDisplayProps {
    question: Question;
    questionResult: QuestionResult;
    questionText: string;
    questionScore: number;
}

export default function ReviewNeededQuestionResultDisplay({ question, questionResult, questionText, questionScore }: ReviewNeededQuestionResultDisplayProps) {
    const [needReview, setNeedReview] = useState(false);
    const [questionReview, setReviewResults] = useState<QuestionReview>();
    const [showMessage1, setShowMessage1] = useState(false);
    const [showMessage2, setShowMessage2] = useState(false);
    const [showMessage3, setShowMessage3] = useState(false);
    const [quizResults, setQuizResults] = useState<QuizResults | null>(null);

    const getQuizReview = async (userId: string, quizId: string, questionId: string, score: number) => {
        try {
            console.log(`Fetching review for user ID: ${userId} quizId: ${quizId} questionId: ${questionId} score: ${score}`);
            const response = await axios.put(`http://localhost:5276/api/quizResults/reviewResult?userId=${userId}&quizId=${quizId}&questionId=${questionId}&finalScore=${score}`);
            console.log(response.data);
            setReviewResults(response.data);
            setNeedReview(true);
        } catch (error) {
            console.error('Error fetching quiz result:', error);
        }
    };

    const getQuizResult = async (userId: string, quizId: string) => {
        setShowMessage3(true);
        setShowMessage1(false);
        setShowMessage2(false);
        try {
          const response = await axios.get(`http://localhost:5276/api/quizResults/getQuizResult/${userId}/${quizId}`);
          console.log(response.data);
          response.data.userId = userId;
          response.data.quizId = quizId;
          setQuizResults(response.data);
        } catch (error) {
          console.error('Error fetching quiz result:', error);
        }
      };

      if (showMessage3) {
        return (
          <div>
            <div>
              {quizResults && <QuizResultsDisplay quizResults={quizResults} />}
            </div>
          </div>
        );
      }
      else{
        if (needReview === true) {
            return (
                <p>
                    <button className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
                    onClick={() => getQuizResult(questionResult.userId, question.quizId)}>
                        Grade
                    </button>
                    <p>LLMResponse : {questionReview?.updatedQuestionResult.llmReview}</p>
                </p>
            );
        }
        return (
            <div className="flex flex-col items-center">
                <div className="w-full max-w-4xl bg-[#6a8e8f] rounded-lg">
                    <h2 className="text-2xl font-bold mb-4 text-center">{questionText}</h2>
                    <h3 className="text-1xl mb-4 text-center">Score: {questionScore} points</h3>
                    <div className="flex items-center">
                        <div className="w-1/3 p-4">
                            <img src="/src/utils/mocks/question-mark.png" alt="Question" className="w-full h-auto rounded-lg shadow-2xl" />
                        </div>
                        <div className="w-2/3 p-4 bg-[#6a8e8f] rounded-lg">
                            <div className="space-y-4">
                                <div className="flex items-center justify-center mb-2">
                                    <label
                                        className={classNames(
                                            'p-2 rounded-full border border-gray-300 w-full max-w-md text-left text-white',
                                            {
                                                'bg-red-500': questionResult.reviewNeededResult === AnswerResult.Wrong,
                                                'bg-yellow-500': questionResult.reviewNeededResult === AnswerResult.PartiallyCorrect,
                                                'bg-green-500': questionResult.reviewNeededResult === AnswerResult.Correct,
                                                'bg-purple-500': questionResult.reviewNeededResult === AnswerResult.Pending,
                                            }
                                        )}
                                    >
                                        Your Answer: {questionResult.reviewNeededAnswer}
                                    </label>
                                </div>
                                {questionResult.type === QuestionType.ReviewNeeded && questionResult.reviewNeededResult === AnswerResult.Pending &&
                                    <button
                                        className="block w-72 h-12 mx-auto bg-teal-700 text-white rounded-full text-center leading-12 text-lg no-underline mt-4"
                                        onClick={() => getQuizReview(questionResult.userId, question.quizId, questionResult.questionId, questionScore)}
                                    >
                                        Review
                                    </button>
                                }
                            </div>
                        </div>
    
                    </div>
                </div>
            </div>
        );
    }
      }


    