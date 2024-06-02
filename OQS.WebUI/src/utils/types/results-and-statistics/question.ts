import { QuestionType } from "../questions";

export type Question = {
  id: string;
  quizId: string;
  type: QuestionType;
  text: string;
  allocatedPoints: number;

  choices?: string[];
  trueFalseAnswer?: boolean;
  multipleChoiceAnswers?: string[];
  singleChoiceAnswer?: string;
  writtenAcceptedAnswers?: string[];
};
