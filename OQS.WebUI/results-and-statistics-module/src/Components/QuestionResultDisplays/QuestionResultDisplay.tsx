import { QuestionType } from "../../utils/types/questions";
import { Question, QuestionResult } from '../../utils/types/results-and-statistics/quiz-results';
import TrueFalseQuestionResultDisplay from './TrueFalseQuestionResultDisplay';
import ChoiceQuestionResultDisplay from "./ChoiceQuestionResultDisplay";
import WrittenQuestionResultDisplay from "./WrittenQuestionResultDisplay";

interface CommonQuestionResultDisplayProps {
  question: Question;
  questionResult: QuestionResult;
}

export default function CommonQuestionResultDisplay({ question, questionResult }: CommonQuestionResultDisplayProps) {
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
            {question.type === QuestionType.TrueFalse && (
              <TrueFalseQuestionResultDisplay question={question} questionResult={questionResult} />
            )}
            {(question.type === QuestionType.SingleChoice || question.type === QuestionType.MultipleChoice) && (
              <ChoiceQuestionResultDisplay question={question} questionResult={questionResult} />
            )}
            {question.type === QuestionType.WrittenAnswer && (
              <WrittenQuestionResultDisplay question={question} questionResult={questionResult} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
