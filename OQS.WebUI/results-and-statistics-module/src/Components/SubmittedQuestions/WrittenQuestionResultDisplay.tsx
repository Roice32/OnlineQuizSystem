import React from 'react';
import { Question, QuestionResult } from "../../utils/types/results-and-statistics/quiz-results";
import classNames from 'classnames';

interface WrittenQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function WrittenQuestionResultDisplay({ question, questionResult }: WrittenQuestionResultDisplayProps) {
  return (
    <div className="flex flex-col items-center">
      <div className="w-full max-w-4xl bg-[#6a8e8f] rounded-lg">
        <h2 className="text-2xl font-bold mb-4 text-center">{question.text}</h2>
        <h3 className="text-1xl mb-4 text-center">Score: {questionResult.score} / {question.allocatedPoints} points</h3>
        <div className="flex items-center">
          <div className="w-1/3 p-4">
            <img src="/src/utils/mocks/question-mark.png" alt="Question" className="w-full h-auto rounded-lg shadow-2xl" />
          </div>
          <div className="w-2/3 p-4 bg-[#6a8e8f] rounded-lg">
            <div className="space-y-4">
              <div className="flex items-center justify-center mb-2">
                <label className='p-2 rounded-full border border-gray-300 w-full max-w-md text-left bg-green-200 text-black'>
                  Correct Answer: {question.writtenAcceptedAnswers?.join('\n ')}
                </label>
              </div>
              <div className="flex items-center justify-center mb-2">
                <label className={classNames('p-2 rounded-full border border-gray-300 w-full max-w-md text-left', {
                  'bg-green-500 text-white': questionResult.writtenAnswer === question.writtenAcceptedAnswers?.find(answer => answer === questionResult.writtenAnswer),
                  'bg-red-500 text-white': questionResult.writtenAnswer !== question.writtenAcceptedAnswers?.find(answer => answer === questionResult.writtenAnswer),
                })}>
                  Your Answer: {questionResult.writtenAnswer}
                </label>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
