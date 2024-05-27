import { Answer } from "../../utils/types/active-quiz";
import { QuestionBase, QuestionType } from "../../utils/types/questions";
import MultipleChoiceQuestionDisplay from "./MultipleChoiceQuestionDisplay";
import SingleChoiceQuestionDisplay from "./SingleChoiceQuestion";
import TrueFalseQuestionDisplay from "./TrueFalseQuestionDisplay";
import WrittenQuestionDisplay from "./WrittenQuestionDisplay";

export interface QuestionDisplayProps {
  question: QuestionBase;
  selectedAnswer?: Answer;
  activeQuizId: string;
}

export default function QuestionDisplay({
  question,
  selectedAnswer,
  activeQuizId,
}: QuestionDisplayProps) {
  const props: QuestionDisplayProps = {
    question,
    selectedAnswer,
    activeQuizId,
  };
  return (
    <>
      {question.type === QuestionType.TrueFalse && (
        <TrueFalseQuestionDisplay {...props} />
      )}
      {question.type === QuestionType.SingleChoice && (
        <SingleChoiceQuestionDisplay {...props} />
      )}
      {question.type === QuestionType.MultipleChoice && (
        <MultipleChoiceQuestionDisplay {...props} />
      )}
      {(question.type === QuestionType.WriteAnswer ||
        question.type === QuestionType.ReviewNeeded) && (
        <WrittenQuestionDisplay {...props} />
      )}
    </>
  );
}
