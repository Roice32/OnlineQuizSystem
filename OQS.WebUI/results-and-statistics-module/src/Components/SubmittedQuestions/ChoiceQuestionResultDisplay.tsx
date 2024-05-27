import classNames from 'classnames';
import { Question, QuestionResult } from '../../utils/types/results-and-statistics/quiz-results';
import { AnswerResult } from '../../utils/types/results-and-statistics/question-review';

interface ChoiceQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function ChoiceQuestionResultDisplay({ question, questionResult }: ChoiceQuestionResultDisplayProps) {
  const parsedObj = JSON.parse(questionResult.pseudoDictionaryChoicesResults!);
  const choicesResults = new Map<string, AnswerResult>(Object.entries(parsedObj).map(([key, value]) => [key, Number(value)]));
  return (
    <div className="flex flex-col items-center">
      <div className="w-full max-w-4xl bg-[#6a8e8f] rounded-lg border-4 border-solid border-[#1c4e4f]">
        <h2 className="text-2xl font-bold mb-4 text-center">{question.text}</h2>
        <h3 className="text-1xl mb-4 text-center">Score: {questionResult.score} / {question.allocatedPoints} points</h3>
        <div className="flex items-center">
          <div className="w-1/3 p-4">
            <img src="/src/utils/mocks/question-mark.png" alt="Question" className="w-full h-auto rounded-lg shadow-2xl" />
          </div>
          <div className="w-2/3 p-4 bg-[#6a8e8f] rounded-lg">
            <div className="space-y-4">
              { Array.from(choicesResults).map(([choice, result], i) => (
                <div key={i} className="flex items-center justify-center mb-2">
                  <label
                    className={classNames(
                      'p-2 rounded-full border border-gray-300 w-full max-w-md text-left',
                      {
                        'bg-green-500 text-white border-4 border-solid border-green-700': result === AnswerResult.Correct,
                        'bg-green-200 text-black border-4 border-dashed border-green-400': result === AnswerResult.CorrectNotPicked,
                        'bg-red-500 text-white border-4 border-solid border-red-700':  result === AnswerResult.Wrong,
                        'bg-gray-200 text-black': result === AnswerResult.Other,
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