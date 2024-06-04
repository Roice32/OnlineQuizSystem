export type QuizResultHeader = {
  resultId: string;
  quizId: string;
  userId: string;
  userName: string;
  quizName: string;
  score: number;
  submittedAtUtc: string;
  reviewPending: boolean;
};
