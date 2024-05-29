import React from 'react';
import { QuestionBase } from "../../utils/types/questions";
import { Answer } from "../../utils/types/active-quiz";
import { QuestionResult } from '../../utils/types/results-and-statistics/quiz-results';

interface WrittenQuestionResultDisplayProps {
  question: QuestionBase;
  userAnswer: QuestionResult;
  correctAnswer: string;
  questionText: string;
  questionScore: number;
}

export default function WrittenQuestionResultDisplay({userAnswer, correctAnswer, questionText, questionScore }: WrittenQuestionResultDisplayProps) {
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
                  className='p-2 rounded-full border border-gray-300 w-full max-w-md text-left bg-green-500 text-white'
                >
                  Your Answer: {userAnswer.writtenAnswer || userAnswer.reviewNeededAnswer}
                </label>
              </div>
              <div className="flex items-center justify-center mb-2">
                <label
                  className='p-2 rounded-full border border-gray-300 w-full max-w-md text-left bg-blue-500 text-white'
                >
                  Correct Answer: {correctAnswer}
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}