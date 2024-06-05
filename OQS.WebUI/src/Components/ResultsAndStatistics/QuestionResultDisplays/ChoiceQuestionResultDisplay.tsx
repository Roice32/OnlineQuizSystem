import classNames from 'classnames';
import { QuestionResult } from "../../../utils/types/results-and-statistics/question-result";
import { Question } from "../../../utils/types/results-and-statistics/question";
import { AnswerResult } from '../../../utils/types/results-and-statistics/answer-result';

interface ChoiceQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function ChoiceQuestionResultDisplay({ question, questionResult }: ChoiceQuestionResultDisplayProps) {
  const parsedObj = JSON.parse(questionResult.pseudoDictionaryChoicesResults!);
  const choicesResults = new Map<string, AnswerResult>(Object.entries(parsedObj).map(([key, value]) => [key, Number(value)]));
  return (
    <div className="space-y-4">
      {question.choices!.map((choice, i) => (
        <div key={i} className="flex items-center justify-center mb-2">
          <label
            className={classNames(
              'p-2 rounded-full border border-gray-300 w-full max-w-md text-left',
              {
                'bg-green-500 text-white border-4 border-solid border-green-700': choicesResults[choice] === AnswerResult.Correct,
                'bg-green-200 text-black border-4 border-dashed border-green-400': choicesResults[choice] === AnswerResult.CorrectNotPicked ||
                    (question.multipleChoiceAnswers?.includes(choice) || question.singleChoiceAnswer! === choice),
                'bg-red-500 text-white border-4 border-solid border-red-700': choicesResults[choice] === AnswerResult.Wrong,
                'bg-gray-200 text-black': choicesResults[choice] === AnswerResult.Other ||
                    choicesResults.size === 0 || choicesResults.has(choice) === false,
              }
            )}
          >
            {choice}
          </label>
        </div>
      ))}
    </div>
  );
}
