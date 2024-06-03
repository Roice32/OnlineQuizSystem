import classNames from 'classnames';
import { QuestionResult } from "../../../utils/types/results-and-statistics/question-result";
import { Question } from "../../../utils/types/results-and-statistics/question";
import { AnswerResult } from '../../../utils/types/results-and-statistics/answer-result';

interface TrueFalseQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function TrueFalseQuestionResultDisplay({ question, questionResult }: TrueFalseQuestionResultDisplayProps) {
  return (
    <div className="space-y-4">
      <div className="flex items-center justify-center mb-2">
        <label
          className={classNames(
            'p-2 rounded-full w-full max-w-md text-left',
            {
              'bg-green-500 text-white border-4 border-solid border-green-700': question.trueFalseAnswer === true && questionResult.trueFalseAnswerResult === AnswerResult.Correct,
              'bg-green-200 text-black border-4 border-dashed border-green-500': question.trueFalseAnswer === true && questionResult.trueFalseAnswerResult === AnswerResult.Wrong,
              'bg-red-500 text-white border-4 border-solid border-red-700': question.trueFalseAnswer !== true && questionResult.trueFalseAnswerResult === AnswerResult.Wrong,
              "bg-gray-200 text-black": question.trueFalseAnswer !== true && questionResult.trueFalseAnswerResult === AnswerResult.Correct
            }
          )}
        >
          True
        </label>
      </div>
      <div className="flex items-center justify-center mb-2">
        <label
          className={classNames(
            'p-2 rounded-full border border-gray-300 w-full max-w-md text-left',
            {
              'bg-green-500 text-white border-4 border-solid border-green-700': question.trueFalseAnswer === false && questionResult.trueFalseAnswerResult === AnswerResult.Correct,
              'bg-green-200 text-black border-4 border-dashed border-green-500': question.trueFalseAnswer === false && questionResult.trueFalseAnswerResult === AnswerResult.Wrong,
              'bg-red-500 text-white border-4 border-solid border-red-700': question.trueFalseAnswer !== false && questionResult.trueFalseAnswerResult === AnswerResult.Wrong,
              "bg-gray-200 text-black": question.trueFalseAnswer !== false && questionResult.trueFalseAnswerResult === AnswerResult.Correct
            }
          )}
        >
          False
        </label>
      </div>
    </div>
  );
}
