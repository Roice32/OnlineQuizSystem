import React from 'react';
import { QuestionBase } from "../../utils/types/questions";
import { Answer } from "../../utils/types/active-quiz";
import classNames from 'classnames';

interface SingleChoiceQuestionResultDisplayProps {
  question: QuestionBase;
  userAnswer: Answer;
  correctAnswer: boolean;
  questionText: string;
  questionScore: number;
  choices: string[];
}

export default function SingleChoiceQuestionResultDisplay({ question, userAnswer, correctAnswer, questionText, questionScore, choices }: SingleChoiceQuestionResultDisplayProps) {
  const isAnswerCorrect = userAnswer.singleChoiceAnswer === correctAnswer;

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
              {choices.map((choice, i) => (
                <div key={i} className="flex items-center justify-center mb-2">
                  <label
                    className={classNames(
                      'p-2 rounded-full border border-gray-300 w-full max-w-md text-left',
                      {
                        'bg-green-500 text-white': userAnswer.singleChoiceAnswer === choice && isAnswerCorrect,
                        'bg-red-500 text-white': userAnswer.singleChoiceAnswer === choice && !isAnswerCorrect,
                        'bg-white text-black': userAnswer.singleChoiceAnswer !== choice
                      }
                    )}
                  >
                    {choice}
                  </label>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}