import React from 'react';
import { QuestionBase } from "../../utils/types/questions";
import { Answer } from "../../utils/types/active-quiz";
import { Question, QuestionResult } from "../../utils/types/results-and-statistics/quiz-results";

interface WrittenQuestionDisplayProps {
  question: Question;
  questionResult: QuestionResult;

}

export default function WrittenQuestionDisplay({ question, questionResult}: WrittenQuestionDisplayProps) {
  return (
    <div className="flex flex-col items-center">
      <div className="w-full max-w-4xl bg-[#6a8e8f] rounded-lg">
        <h2 className="text-2xl font-bold mb-4 text-center">{question.text}</h2>
        <h3 className="text-1xl mb-4 text-center"> / {question.allocatedPoints} points</h3>
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
                  Your Answer: {questionResult.writtenAnswer?.toString()}
                </label>
              </div>
              <div className="flex items-center justify-center mb-2">
                <label
                  className='p-2 rounded-full border border-gray-300 w-full max-w-md text-left bg-blue-500 text-white'
                >
                  Correct Answer: {question.writtenAcceptedAnswers?.toString()}
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}