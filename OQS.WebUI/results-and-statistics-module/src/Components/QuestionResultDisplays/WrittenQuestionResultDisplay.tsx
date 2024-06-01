import React from 'react';
import classNames from 'classnames';
import { Question, QuestionResult } from '../../utils/types/results-and-statistics/quiz-results';

interface WrittenQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function WrittenQuestionResultDisplay({ question, questionResult }: WrittenQuestionResultDisplayProps) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-center mb-2">
        <label className='p-2 rounded-3xl border-4 border-solid border-green-400 w-full max-w-md text-left bg-green-200 text-black'>
          Correct answer(s): { question.writtenAcceptedAnswers?.map((answer, index) => (
          <p key={index} className="m-0">{answer}</p>))}
        </label>
      </div>
      <div className="flex items-center justify-center mb-2">
        <label className={classNames('p-2 rounded-full w-full max-w-md text-left', {
          'bg-green-500 text-white border-4 border-solid border-green-700': questionResult.writtenAnswer === question.writtenAcceptedAnswers?.find(answer => answer === questionResult.writtenAnswer),
          'bg-red-500 text-white border-4 border-solid border-red-700': questionResult.writtenAnswer !== question.writtenAcceptedAnswers?.find(answer => answer === questionResult.writtenAnswer),
        })}>
          Your answer: {questionResult.writtenAnswer}
        </label>
      </div>
    </div>
  );
}
